using System;
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
        /// ロックオン中かどうかのフラグ
        /// </summary>
        private bool _lockingOn;

        public void InitializeThisComponent()
        {
            this._player = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(_ => _.GetComponent<IPlayerCameraTrasable>() != null)
                .Select(_ => _.GetComponent<GameObject>()).First().gameObject.transform;
        }

        private void FixedUpdate()
        {
            
        }

        public void FinalizeThisComponent()
        {
        }
    }
}