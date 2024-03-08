using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayerCam.Scripts
{
    /// <summary>
    /// 作成：菅沼
    /// プレイヤーカメラの機能を提供する、
    /// 主に
    /// プレイヤ追跡とロックオンターゲットのロックオンを提供。
    /// </summary>
    public class PlayerCamera
        : MonoBehaviour,
            IInitializableComponent
    {
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

        /// <summary>
        /// ロックオン入力が入った時に発火するイベントへの登録関数
        /// </summary>
        void LockOnTriggerred()
        {
            _lockingOn = !_lockingOn;
            if (_lockingOn)
            {
                _lockOnTargets = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                    .Where(_ => _.GetComponent<IPlayerCamLockable>() != null)
                    .Select(_ => _.GetComponent<IPlayerCamLockable>().GetLockableObjectTransform()).ToList();
            }
            else
            {
                _lockOnTargets.Clear();
            }
        }

        void CamBehaviourDefault()
        {
            
        }

        void CamBehaviourLockingOn()
        {
            
        }
        
        void CameraBehaviourEveryFrame()
        {
            switch (_lockingOn)
            {
                case true:
                    CamBehaviourLockingOn();
                    break;
                case false:
                    CamBehaviourDefault();
                    break;
            }
        }

        public void InitializeThisComponent()
        {
            // 検索にひっかかった最初のオブジェクトをプレイヤとする
            this._player = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(_ => _.GetComponent<IPlayerCameraTrasable>() != null)
                .Select(_ => _.GetComponent<GameObject>()).First().gameObject.transform;

            // ロックオンイベント発火元へのデリゲート登録をする
            GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(_ => _.GetComponent<ILockOnEventFirable>() != null)
                .Select(_ => _.GetComponent<ILockOnEventFirable>()).ToList()
                .ForEach(_ => _.ELockOnTriggered += this.LockOnTriggerred);

        }

        private void FixedUpdate()
        {
            CameraBehaviourEveryFrame();
        }

        public void FinalizeThisComponent()
        {
            // ロックオンイベント発火元へのデリゲート登録解除をする
            GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(_ => _.GetComponent<ILockOnEventFirable>() != null)
                .Select(_ => _.GetComponent<ILockOnEventFirable>()).ToList()
                .ForEach(_ => _.ELockOnTriggered -= this.LockOnTriggerred);
        }
    }
}