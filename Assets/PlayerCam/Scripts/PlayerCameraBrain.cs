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
    public class PlayerCameraBrain
        : MonoBehaviour,
            IInitializableComponent
    {
        #region Parameter Exposing

        [SerializeField, Header("The Radius When Locking On The Targets")]
        private float LockOnRadius;

        [SerializeField, Header("The Max Distance Capurable Lock-On Target")]
        private float MaxDistanceToCapture = 100f;

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

        // 入力値 ー キャラ移動と視点移動入力
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
            _playerFollowCam.Priority = 0;
            _lockOnCam.Priority = 0;

            if (_lockingOn)
            {
                // Get All LockOn Target
                // マップ上のロックオン可能なターゲットを取得
                // Filter Captureable Target
                // 捕捉可能な距離圏内にいるターゲットを取得
                _lockOnTargets = boost.GetDerivedComponents<IPlayerCamLockable>()
                    .Select(_ => _.GetLockableObjectTransform())
                    .Where(_ => Vector3.Distance(_player.position, _.position) <= MaxDistanceToCapture
                                && Camera.main.WorldToScreenPoint(_.position).z > 0)
                    .ToList();

                // If LockOn Targte Was Not Found Cancel Locking On
                if (_lockOnTargets.Count() < 1)
                {
                    _lockingOn = false;
                    return;
                }

                // The Subtraction for calculate Vector Player position-supposed 
                // ロックオン発動時のプレイヤの位置を設定するためのベクトル
                var dx = _lockOnTargets[_lockingOnTargetIndex].position.x
                         - _player.position.x;
                var dy = _lockOnTargets[_lockingOnTargetIndex].position.z
                         - _player.position.z;

                // ま反対の方向へプレイヤの位置が初期化されてしまうのでオイラー角でいう180°を足せばよい。
                // Atan2は弧度法の値で返してくるのでPI（弧度法）を返す
                var angleForPos = Mathf.Atan2(dy, dx) + Mathf.PI;
                _theta = angleForPos;

                // List All Distancies All Target Between Player
                // 捕捉可能なターゲットとプレイヤの距離をすべて取得しておく
                var disList =
                    _lockOnTargets.Select(_ => Vector3.Distance(_player.position, _.position)).ToList();

                // とりあえずインスペクタへ公開しているフィールドも初期化しておく
                LockOnRadius = disList.Max();
                _lockOnRadius = disList.Max();

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
            Debug.Log($"{nameof(PlayerCameraBrain)} Tick");

            // 基本的にオービタルカメラ。 左右のみ、すこし上からプレイヤを見下ろしている視点
            _playerFollowCam.Follow = _player;
            _playerFollowCam.LookAt = _player;
        }

        void CamBehaviourLockingOn()
        {
            Debug.Log($"{nameof(PlayerCameraBrain)} Tick-");
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
            // プレイヤ のトランスフォーム情報を初期化ー設定
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
            // カメラ をセットアップ
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

            // 更新方法 ー 頻度（毎秒）を設定
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
