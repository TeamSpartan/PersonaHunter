using System;
using Player.Action;
using Player.Param;
using UnityEngine;

namespace Player.Hp
{
	///<summary>プレイヤーHPの内部処理</summary>
	public class PlayerHp : MonoBehaviour, IDamagedComponent
	{
		[SerializeField, Header("無敵フレーム")] private int invincibilityFrame;

		public event System.Action OnDeath,
			OnReceiveDamage,
			OnReceiveHeal,
			OnHitWhileInvulnerable,
			OnBecomeVulnerable,
			OnResetDamage;

		private PlayerParam _playerParam;
		private PlayerAvoid _playerAvoid;

		private float _initialHp;
		private float _currentHp;
		private int _frameSinceLastHit;

		#region properties

		public float CurrentHp => _currentHp;
		public float InitialHp => _initialHp;

		#endregion

		private void OnEnable()
		{
			#region InitiateAction

			OnResetDamage += () => Debug.Log("ResetDamage");
			OnReceiveDamage += () => Debug.Log("ReceiveDamage");
			OnBecomeVulnerable += () => Debug.Log("BecomeVulnerable");
			OnReceiveHeal += () => Debug.Log("ReceiveHeal");
			OnHitWhileInvulnerable += () => Debug.Log("HitWhileInvulnerable");
			OnDeath += () => Debug.Log("Death");

			#endregion
		}

		public void Initialization()
		{
			_playerParam = GetComponentInParent<PlayerParam>();
			_playerAvoid = GetComponentInParent<PlayerAvoid>();
			_initialHp = _playerParam.GetInitialHp;
		}

		private void Update()
		{
			//無敵時間中
			if (_playerParam.GetIsDamage)
			{
				if (_frameSinceLastHit > invincibilityFrame)
				{
					_frameSinceLastHit = 0;
					OnBecomeVulnerable?.Invoke();
					_playerParam.SetIsDamage(false);
				}
				else
				{
					++_frameSinceLastHit;
				}
			}
		}

		///<summary>HPのリセット</summary>
		public void ResetDamage()
		{
			_currentHp = _initialHp;
			OnResetDamage?.Invoke();
		}

		public void AddDamage(float dmg)
		{
			if (_currentHp <= 0 || _playerParam.GetIsParry)
			{
				return;
			}

			if (_playerParam.GetIsAvoid && !_playerParam.GetIsJustAvoid)
			{
				_playerAvoid.OnAvoidSuccess.Invoke();
				return;
			}

			if (_playerParam.GetIsJustAvoid)
			{
				_playerAvoid.OnJustAvoidSuccess.Invoke(_playerParam.GetIncreaseValueOfJustAvoid);
				return;
			}

			//Received damage while invincible
			if (_playerParam.GetIsDamage)
			{
				OnHitWhileInvulnerable?.Invoke();
				return;
			}

			//減産処理
			_currentHp -= (int)dmg;
			if (dmg > 0f)
			{
				//Processing when HP decreases
				OnReceiveDamage?.Invoke();
				_playerParam.SetIsDamage(true);
			}
			else
			{
				//Processing when HP increases
				OnReceiveHeal?.Invoke();
			}

			//at time of death
			if (_currentHp <= 0f)
				OnDeath?.Invoke();
		}

		#region OverLoad

		public void AddDamage(int dmg)
		{
			if (_currentHp <= 0 || _playerParam.GetIsParry)
			{
				return;
			}

			if (_playerParam.GetIsAvoid && !_playerParam.GetIsJustAvoid)
			{
				_playerAvoid.OnAvoidSuccess.Invoke();
			}

			if (_playerParam.GetIsJustAvoid)
			{
				_playerAvoid.OnJustAvoidSuccess.Invoke(_playerParam.GetIncreaseValueOfJustAvoid);
			}


			//Received damage while invincible
			if (_playerParam.GetIsDamage)
			{
				OnHitWhileInvulnerable?.Invoke();
				return;
			}

			_currentHp -= dmg;
			if (dmg > 0)
			{
				//Processing when HP decreases
				OnReceiveDamage?.Invoke();
				_playerParam.SetIsDamage(true);
			}
			else
			{
				//Processing when HP increases
				OnReceiveHeal?.Invoke();
			}

			//at time of death
			if (_currentHp <= 0)
				OnDeath?.Invoke();
		}

		#endregion


		public void Kill()
		{
			_currentHp = 0;
			OnDeath?.Invoke();
		}
	}
}
