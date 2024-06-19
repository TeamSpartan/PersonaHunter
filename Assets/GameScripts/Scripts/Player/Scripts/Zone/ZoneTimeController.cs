using System.Collections;
using UnityEngine;

namespace Player.Zone
{
    /// <summary>SlowTime時の処理をします</summary>
    public class ZoneTimeController : MonoBehaviour
    {
        [SerializeField, Tooltip("制限時間"), Header("制限時間")]
        private float zoneTimeLimit = 10f;

        private bool _exitFlag = true; // 終了時フラグ
        private bool _finishFlag = true; // 強制終了フラグ
        private bool _isSlowTime; // SlowTime中かどうかのフラグ

        /// <summary>制限時間を取得します</summary>
        public float GetZoneTimeLimit => zoneTimeLimit;

        /// <summary>SlowTime中かどうかのフラグを取得します</summary>
        public bool GetIsSlowTime => _isSlowTime;

        /// <summary>強制終了フラグ</summary>
        public void SetFinishFlag(bool flag) => _finishFlag = flag;

        private ZoneObj _zoneObj;
        private GameLogic _gameLogic;

        private void Start()
        {
            _zoneObj = FindObjectOfType<ZoneObj>();
            _gameLogic = FindAnyObjectByType<GameLogic>();
        }

        IEnumerator ZoneTimerCountDown()
        {
            for (float i = zoneTimeLimit; i > 0; i--)
            {
                yield return new WaitForSeconds(1f);
                _zoneObj.ZoneDecreaseUpdate(i);
                Debug.Log("ゾーンゲージマイナス");
            }

            EndDull();
        }

        /// <summary>
        /// これを一度読んだらコルーチンが回るのでEndDullを態々呼ぶ必要はない。
        /// </summary>
        public void StartDull()
        {
            if (_isSlowTime || _zoneObj.ActiveZoneObjCount < 1) return;

            _exitFlag = false;
            _isSlowTime = true;
            _gameLogic.StartDiveInZone();
            _zoneObj.StartDull();
            this.StartCoroutine(this.ZoneTimerCountDown());

#if UNITY_EDITOR
            Debug.Log("EnterSlowTime");
#endif
        }

        /// <summary>
        /// トキトメの強制終了
        /// </summary>
        public void EndDull()
        {
            _exitFlag = true;
            _isSlowTime = false;
            _gameLogic.GetOutOverZone();
            _zoneObj.EndDull();

#if UNITY_EDITOR
            Debug.Log("ExitSlowTime");
#endif
        }
    }
}
