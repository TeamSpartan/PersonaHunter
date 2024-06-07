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
		private GameLogic _gameLogic = new();

		private void Start()
		{
			_zoneObj = FindObjectOfType<ZoneObj>();
		}

		IEnumerator ZoneTimerCountDown()
		{
			for (float i = zoneTimeLimit; i > 0; i--)
			{
				yield return new WaitForSeconds(1f);
				_zoneObj.ZoneDecreaseUpdate(i);
				Debug.Log("ゾーンゲージマイナス");
			}

			_gameLogic.GetOutOverZone();
		}

		public void StartDull()
		{
			_exitFlag = false;
			this.StartCoroutine(this.ZoneTimerCountDown());

#if UNITY_EDITOR
			Debug.Log("EnterSlowTime");
#endif
		}

		public void EndDull()
		{
			_exitFlag = true;
			_isSlowTime = false;

#if UNITY_EDITOR
			Debug.Log("ExitSlowTime");
#endif
		}
	}
}
