using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Hp
{
	///<summary>HP表示用のスクリプト</summary>
	public class PlayerHpIndication : PlayerHp
	{
		[SerializeField, Header("HPが減る速度")] private float _duration;
		[SerializeField, Header("赤ゲージが残る時間")] private float _waitTime = .2f;
		[SerializeField] private Image healthImage;
		[SerializeField] private Image burnImage;

		private Tween _burnEffect;

		private void Start()
		{
			healthImage.fillAmount = 1f;
			burnImage.fillAmount = 1f;
			OnReceiveDamage += SetGauge;
			Initialization();
			ResetDamage();
		}

		public void SetGauge()
		{
			_burnEffect?.Kill();
			healthImage.DOFillAmount(CurrentHp / InitialHp, _duration).OnComplete(() =>
			{
				_burnEffect = burnImage.DOFillAmount(CurrentHp / InitialHp, _duration * 0.5f).SetDelay(_waitTime);
			});
		}
	}
}
