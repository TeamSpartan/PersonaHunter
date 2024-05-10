using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
	[SerializeField, Header("HPが減る速度")] private float _duration;
	[SerializeField, Header("赤ゲージが残る時間")] private float _waitTime = .2f;
	[SerializeField] private Image _healthImage;
	[SerializeField] private Image _burnImage;

	[SerializeField, Header("ボス初登場時のHPの増える速度")]
	private float _hpDuration;

	private float _initiateHp;
	private float saveHp;
	
	private NuweBrain _nuweBrain;
	private Tween _burnEffect;
	

	private void OnEnable()
	{
	}

	private void Start()
	{
		_nuweBrain = GetComponent<NuweBrain>();
		_healthImage.fillAmount = 1f;
		_burnImage.fillAmount = 1f;
		_initiateHp = _nuweBrain.GetMaxHP;
	}

	private void Update()
	{
		if (saveHp != _nuweBrain.GetHealthPoint)
		{
			SetGauge();
			saveHp = _nuweBrain.GetHealthPoint;
		}
	}

	public void SetGauge()
	{
		Debug.Log("ゲージ");
		_burnEffect?.Kill();
		_healthImage.DOFillAmount(_nuweBrain.GetHealthPoint / _initiateHp, _duration).OnComplete(() =>
		{
			_burnEffect = _burnImage.DOFillAmount(_nuweBrain.GetHealthPoint / _initiateHp, _duration * 0.5f).SetDelay(_waitTime);
		});
	}

	void BarEffect()
	{
		_healthImage.DOFillAmount(0, 0f);
		_burnImage.DOFillAmount(0, 0f);
		_healthImage.DOFillAmount(_initiateHp, _hpDuration);
		_burnImage.DOFillAmount(_initiateHp, _hpDuration * 0.6f);
	}
}
