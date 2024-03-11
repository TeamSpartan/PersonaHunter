using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using SgLibUnite.CodingBooster;

namespace PlayerCam.Scripts
{
    [RequireComponent(typeof(CinemachineBrain))]
    /// <summary>
    /// 作成：菅沼
    /// プレイヤーカメラの機能を提供する、
    /// 主に
    /// プレイヤ追跡とロックオンターゲットのロックオンを提供。
    /// </summary>
    public class SoulPlayerCameraBrain
        : MonoBehaviour,
            IInitializableComponent
    {
        #region Parameter Exposing

        [SerializeField, Header("The Radius When Locking On The Targets"), Range(1f, 10f)]
        private float LockOnRadius;

        [SerializeField, Header("The Camera Default")]
        private CinemachineVirtualCamera PlayerFollowingCam;

        [SerializeField, Header("The Camera LockingOn Target")]
        private CinemachineVirtualCamera LockOnCamera;

        #endregion

        #region Parameter Inside

        /// <summary>
        /// プレイヤ
        /// </summary>
        private Transform _player;

        /// <summary>
        /// ロックオン対象の配列
        /// </summary>
        private List<Transform> _lockOnTargets;

        /// <summary>
        /// ロックオン中かどうかのフラグ
        /// </summary>
        private bool _lockingOn;
        
        public bool LockingOn
        {
            get { return this._lockingOn; }
        }

        /// <summary>
        /// コーディング ブースタ クラス
        /// </summary>
        CBooster boost = new CBooster();

        /// <summary>
        /// ロックオン時の半径
        /// </summary>
        private float _lockOnRadius;

        /// <summary>
        /// ロックオン対象のインデックス
        /// </summary>
        private int _lockingOnTargetIndex = 0;
        
        // 入力値
        private float _moveX;
        private float _moveY;
        private float _mouseX;
        private float _mouseY;
        private float _theta;

        private CinemachineBrain _cinemachineBrain;
        private CinemachineVirtualCamera _playerFollowCam;
        private CinemachineVirtualCamera _lockOnCam;

        #endregion

        /// <summary>
        /// ロックオン入力が入った時に発火するイベントへの登録関数
        /// </summary>
        void LockOnTriggerred()
        {
            _lockingOn = !_lockingOn;
            _lockOnCam.Priority = 0;
            _playerFollowCam.Priority = 0;

            if (_lockingOn)
            {
                _lockOnTargets = boost.GetDerivedComponents<IPlayerCamLockable>()
                    .Select(_ => _.GetLockableObjectTransform()).ToList();
                _theta = 0f; // *
                _lockOnRadius = _lockOnTargets.Max(_ => Vector3.Distance(_player.position, _.position)); // *

                _lockOnCam.Priority = 1;
            }
            else
            {
                _lockOnTargets.Clear();

                _playerFollowCam.Priority = 1;
            }
        }

        void GetInputValue()
        {
            var iInput = boost.GetDerivedComponents<IInputValueReferencable>();

            _moveX = iInput.First().GetHorizontalMoveValue();
            _moveY = iInput.First().GetVerticalMoveValue();

            _mouseX = iInput.First().GetHorizontalMouseMoveValue();
            _mouseY = iInput.First().GetVerticalMouseMoveValue();
        }

        void CamBehaviourDefault()
        {
            Debug.Log($"{nameof(SoulPlayerCameraBrain)} Tick");

            // 基本的にオービタルカメラ。 左右のみ、すこし上からプレイヤを見下ろしている視点
            _playerFollowCam.Follow = _player;
            _playerFollowCam.LookAt = _player;
        }

        void CamBehaviourLockingOn()
        {
            Debug.Log($"{nameof(SoulPlayerCameraBrain)} Tick-");
            // 通常カメラとは視点は大きな変化はなし、ロックオンターゲット中心に
            // 円形を描くような左右移動をする。
            // 前後（敵に対して）すると半径の値が変動

            var inputMove = new Vector2(_moveX, _moveY);
            var inputLook = new Vector2(_mouseX, _mouseY);

            // 前後移動
            if (_moveY < 0 && _moveY != 0)
            {
                if (_lockOnRadius <= LockOnRadius)
                {
                    _lockOnRadius += Time.deltaTime * 10f;
                }
            }
            else if (_moveY != 0)
            {
                if (_lockOnRadius >= 1)
                {
                    _lockOnRadius -= Time.deltaTime * 10f;
                }
            }

            // 左右移動
            if (_moveX < 0 && _moveX != 0)
            {
                _theta -= Time.deltaTime;
            }
            else if (_moveX != 0)
            {
                _theta += Time.deltaTime;
            }

            // Set Up Player 
            var playerDirRight = Mathf.Cos(_theta) * _lockOnRadius;
            var playerDirForward = Mathf.Sin(_theta) * _lockOnRadius;
            var pDir = new Vector2(playerDirRight, playerDirForward);

            var target = _lockOnTargets[_lockingOnTargetIndex].transform;
            var pos = target.position;
            pos.x += pDir.x; // right
            pos.z += pDir.y; // forward

            var dir = new Vector3(target.position.x - _player.position.x
                , 0f
                , target.position.z - _player.position.z).normalized;
            _player.transform.position = pos;
            _player.transform.forward = dir;

            // Set Up Camera
            _lockOnCam.LookAt = target;
            _lockOnCam.Follow = _player;
        }

        void CameraBehaviourEveryFrame()
        {
            if (_lockingOn)
            {
                CamBehaviourLockingOn();
            }
            else
            {
                CamBehaviourDefault();
            }
        }

        public void InitializeThisComponent()
        {
            // 検索にひっかかった最初のオブジェクトをプレイヤとする
            this._player = boost.GetDerivedComponents<IPlayerCameraTrasable>()
                .First().GetPlayerCamTrasableTransform();

            // ロックオンイベント発火元へのデリゲート登録をする
            boost.GetDerivedComponents<ILockOnEventFirable>()
                .ForEach(_ => _.ELockOnTriggered += this.LockOnTriggerred);

            // 内部パラメータ初期化
            this._lockOnRadius = LockOnRadius; // 半径
            this._playerFollowCam = PlayerFollowingCam;
            this._lockOnCam = LockOnCamera;
            this._cinemachineBrain = GetComponent<CinemachineBrain>();

            this._cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
        }

        private void FixedUpdate()
        {
            GetInputValue();
            CameraBehaviourEveryFrame();
        }

        public void FinalizeThisComponent()
        {
            // ロックオンイベント発火元へのデリゲート登録解除をする
            boost.GetDerivedComponents<ILockOnEventFirable>()
                .ForEach(_ => _.ELockOnTriggered -= this.LockOnTriggerred);
        }
    }
}
