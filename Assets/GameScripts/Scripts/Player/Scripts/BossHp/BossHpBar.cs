using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : BossHp
{
	[SerializeField, Header("HPが減る速度")] private float _duration;
	[SerializeField, Header("赤ゲージが残る時間")] private float _waitTime = .2f;
	[SerializeField] private Image _healthImage;
	[SerializeField] private Image _burnImage;
	
	private Tween _burnEffect;

	private void OnEnable()
	{
		base.OnEnable();
		OnReceiveDamage += SetGauge;
	}

	private void Start()
	{
		_healthImage.fillAmount = 1f;
		_burnImage.fillAmount = 1f;
		ResetDamage();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.P))
		{
			base.AddDamage(10);
		}
	}

	public void SetGauge()
	{
		Debug.Log("ゲージ");
		_burnEffect?.Kill();
		_healthImage.DOFillAmount(CurrentHp / InitiateHp, _duration).OnComplete(() =>
		{
			_burnEffect = _burnImage.DOFillAmount(CurrentHp / InitiateHp, _duration * 0.5f).SetDelay(_waitTime);
		});
	}

}
