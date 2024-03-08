using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SgLibUnite.CodingBooster;

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
            var boost = new CBooster();
            if (_lockingOn)
            {
                _lockOnTargets = boost.GetDerivedComponents<IPlayerCamLockable>()
                    .Select(_ => _.GetLockableObjectTransform()).ToList();
            }
            else
            {
                _lockOnTargets.Clear();
            }
        }

        void CamBehaviourDefault()
        {
            Debug.Log($"{nameof(PlayerCamera)} Tick");
        }

        void CamBehaviourLockingOn()
        {
            Debug.Log($"{nameof(PlayerCamera)} Tick-");
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
            var booster = new CBooster();
            this._player = booster.GetDerivedComponents<IPlayerCameraTrasable>()
                .First().GetPlayerCamTrasableTransform();

            // ロックオンイベント発火元へのデリゲート登録をする
            booster.GetDerivedComponents<ILockOnEventFirable>()
                .ForEach(_ => _.ELockOnTriggered += this.LockOnTriggerred);
        }

        private void Update()
        {
            CameraBehaviourEveryFrame();
        }

        public void FinalizeThisComponent()
        {
            // ロックオンイベント発火元へのデリゲート登録解除をする
            var booster = new CBooster();
            booster.GetDerivedComponents<ILockOnEventFirable>()
                .ForEach(_ => _.ELockOnTriggered -= this.LockOnTriggerred);
        }
    }
}
