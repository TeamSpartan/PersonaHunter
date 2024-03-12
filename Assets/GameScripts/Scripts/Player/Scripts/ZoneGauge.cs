using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>SlowTime用のゲージ</summary>
public class ZoneGauge : MonoBehaviour, IDulledTarget
{
	[SerializeField, Tooltip("ゾーンゲージ用のSlider"), Header("ゾーンゲージ用のSlider")]
	private Slider zoneSlider;
	[SerializeField, Header("ウェーブUI")] GameObject FocusWaveUI;

	[SerializeField, Tooltip("ゲージのアニメーション速度"), Header("ゲージのアニメーション速度"), Range(0, 1f)]
	private float animationTime = 0.1f;

	private ZoneTimeController _slowTimeController;
	private bool _canIncreaseGaugeValue = true; // ゲージのValueを増加できるかのフラグ
	private float _maxZoneGaugeValue;
	private float _currentZoneGaugeValue;

	/// <summary>ゲージのValueを増加できるかのフラグを変更します</summary>
	public void SetCanIncreaseGaugeValue(bool flag) => _canIncreaseGaugeValue = flag;
	
	/// <summary>SlowTimeに入れるかどうかを取得します</summary>
	public bool CanSlowTime => _currentZoneGaugeValue >= _maxZoneGaugeValue;

	///<summary>Slider'sMaxValue</summary>
	public float GetMaxZoneGaugeValue => _maxZoneGaugeValue;

	///<summary>Slider'sCurrentGaugeValue</summary>
	public float GetCurrentZoneGaugeValue => _currentZoneGaugeValue;


	private void Start()
	{
		_slowTimeController = FindAnyObjectByType<ZoneTimeController>();
		InitializedGauge();
	}

	/// <summary>呼ばれたときにゲージのValueを増加させます</summary>
	public void IncreaseGaugeValue(float value)
	{
		if (!_canIncreaseGaugeValue) return;
		if (_currentZoneGaugeValue + value >= _maxZoneGaugeValue)
		{
			FocusWaveUI.SetActive(false);
			zoneSlider.gameObject.SetActive(true);
		}
		else
		{
			FocusWaveUI.SetActive(true);
			zoneSlider.gameObject.SetActive(false);
		}
		
		// 滑らかに増加
		DOTween.To(() => _currentZoneGaugeValue,
			(x) => _currentZoneGaugeValue = x,
			_currentZoneGaugeValue + value,
			animationTime);
		//zoneSlider.DOValue(_currentZoneGaugeValue + value, animationTime);
	}

	void InitializedGauge()
	{
		_maxZoneGaugeValue = _slowTimeController.GetZoneTimeLimit;
		_currentZoneGaugeValue = 0;
		FocusWaveUI.SetActive(true);
		zoneSlider.gameObject.SetActive(false);
	}

	public void FixedUpdate()
	{
		if (_slowTimeController.GetIsSlowTime)
			zoneSlider.value = _slowTimeController.GetTimer / _maxZoneGaugeValue;
	}

	public void StartDull()
	{
		SetCanIncreaseGaugeValue(false);
		// ゲージの最大値を制限時間の最大値と同じにする
		_maxZoneGaugeValue = _slowTimeController.GetZoneTimeLimit;
		zoneSlider.value = _slowTimeController.GetTimer;
	}

	public void EndDull()
	{
		// ゲージの最大値を元に戻す
		_maxZoneGaugeValue = _slowTimeController.GetZoneTimeLimit;
		SetCanIncreaseGaugeValue(true);
	}
}
