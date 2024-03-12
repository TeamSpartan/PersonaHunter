using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>SlowTime時の処理をします</summary>
public class ZoneTimeController : MonoBehaviour, IDulledTarget
{
	[SerializeField, Tooltip("制限時間"), Header("制限時間")]
	private float zoneTimeLimit = 10f;

	private GameLogic _gameLogic;
	private List<IDulledTarget> _zoneTimes; // ISlowTimeを継承したオブジェクト達
	private float _timer; // タイマー
	private bool _exitFlag = true; // 終了時フラグ
	private bool _finishFlag = true; // 強制終了フラグ
	private bool _isSlowTime; // SlowTime中かどうかのフラグ

	/// <summary>制限時間を取得します</summary>
	public float GetZoneTimeLimit => zoneTimeLimit;

	/// <summary>タイマーを取得します</summary>
	public float GetTimer => _timer;

	/// <summary>SlowTime中かどうかのフラグを取得します</summary>
	public bool GetIsSlowTime => _isSlowTime;

	/// <summary>強制終了フラグ</summary>
	public void SetFinishFlag(bool flag) => _finishFlag = flag;


	private void FixedUpdate()
	{
		if (GetIsSlowTime)
			_timer -= Time.fixedDeltaTime;
	}

	public void StartDull()
	{
		_timer = zoneTimeLimit;
		_exitFlag = false;
		_isSlowTime = true;

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
