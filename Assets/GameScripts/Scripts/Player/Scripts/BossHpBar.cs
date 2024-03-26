using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : BossHp
{
	[SerializeField, Header("HPが減る速度")] private float _duration;
	[SerializeField, Header("赤ゲージが残る時間")] private float _waitTime = .2f;
	[SerializeField] private Image healthImage;
	[SerializeField] private Image burnImage;
	private float _debugDamageRate = 0.1f;
	private float _currentRate = 1.0f;
	
	private Tween _burnEffect;

	private void Start()
	{
		healthImage.fillAmount = 1f;
		burnImage.fillAmount = 1f;
		OnReceiveDamage += SetGauge;
		ResetDamage();
	}

	public void SetGauge()
	{
		_burnEffect?.Kill();
		healthImage.DOFillAmount(CurrentHp / InitiateHp, _duration).OnComplete(() =>
		{
			_burnEffect = burnImage.DOFillAmount(CurrentHp / InitiateHp, _duration * 0.5f).SetDelay(_waitTime);
		});
	}

}
