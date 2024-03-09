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
        /// コーディング ブースタ クラス
        /// </summary>
        CBooster boost = new CBooster();

        /// <summary>
        /// ロックオン入力が入った時に発火するイベントへの登録関数
        /// </summary>
        void LockOnTriggerred()
        {
            _lockingOn = !_lockingOn;
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

            // 基本的にオービタルカメラ。 左右のみ、すこし上からプレイヤを見下ろしている視点
        }

        void CamBehaviourLockingOn()
        {
            Debug.Log($"{nameof(PlayerCamera)} Tick-");

            // 通常カメラとは視点は大きな変化はなし、ロックオンターゲット中心に
            // 円形を描くような左右移動をする。

            // 前後（敵に対して）すると半径の値が変動
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
        }

        private void Update()
        {
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
