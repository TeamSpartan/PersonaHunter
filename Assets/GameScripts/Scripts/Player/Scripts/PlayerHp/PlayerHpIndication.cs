using DG.Tweening;
using SgLibUnite.CodingBooster;
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
		private CBooster _cBooster;

		private void Start()
		{
			healthImage.fillAmount = 1f;
			burnImage.fillAmount = 1f;
			_cBooster = new();
			OnReceiveDamage += SetGauge;
			OnRegeneration += SetRegenerate;
			base.Initialization();
			ResetDamage();
		}

		private void OnTriggerEnter(Collider other)
		{
			NuweBrain nue = other.GetComponentInParent<NuweBrain>();
			NuweJuvenile mob = other.GetComponentInParent<NuweJuvenile>();
			
			if (nue != null)
			{
				switch (nue.GetAttackType(other.transform))
				{
					case NuweBrain.NueAttackType.Claw:
						AddDamage(nue.GetBaseDamage);
						break;
					case NuweBrain.NueAttackType.Rush:
						AddDamage(nue.GetBaseDamage);
						break;
					case NuweBrain.NueAttackType.Tail:
						AddDamage(nue.GetBaseDamage);
						break;
				}
			}
			else
			{
				Debug.Log("null");
			}

			if (mob != null)
			{
				AddDamage(mob.GetBaseDamage);
			}
		}

		public void SetGauge()
		{
			_burnEffect?.Kill();
			healthImage.DOFillAmount(CurrentHp / InitialHp, _duration).OnComplete(() =>
			{
				_burnEffect = burnImage.DOFillAmount(CurrentHp / InitialHp, _duration * 0.5f).SetDelay(_waitTime);
			});
		}

		public void SetRegenerate()
		{
			healthImage.fillAmount = CurrentHp / InitialHp;
			burnImage.fillAmount = CurrentHp / InitialHp;
		}
	}
}
