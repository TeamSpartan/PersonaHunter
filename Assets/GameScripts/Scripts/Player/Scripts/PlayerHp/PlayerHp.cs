using System;
using Player.Action;
using Player.Param;
using SgLibUnite.CodingBooster;
using UnityEngine;

namespace Player.Hp
{
	///<summary>プレイヤーHPの内部処理</summary>
	public class PlayerHp : MonoBehaviour, IDamagedComponent
	{
		[SerializeField, Header("無敵フレーム")] private int _invincibilityFrame;
		[SerializeField, Header("リジェネまでの時間")] private float _regeneratWaitTime;
		[SerializeField, Header("リジェネ回復量"), Range(1f,100f)] private float _regenerationSpeed;


		public event System.Action OnDeath,
			OnReceiveDamage,
			OnReceiveHeal,
			OnHitWhileInvulnerable,
			OnBecomeVulnerable,
			OnResetDamage,
			OnRegeneration;

		private PlayerParam _playerParam;
		private PlayerAvoid _playerAvoid;

		private float _initialHp;
		private float _currentHp;
		private int _frameSinceLastHit;
		private float _regenerationTimer;
		private bool _isRegeneration;

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
			OnRegeneration += () => Debug.Log("Regene");
			OnReceiveDamage += () => _regenerationTimer = 0;

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
				if (_frameSinceLastHit > _invincibilityFrame)
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

			Regeneration();
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

			//ジャスト回避
			if (_playerParam.GetIsJustAvoid)
			{
				_playerAvoid.OnJustAvoidSuccess.Invoke(_playerParam.GetIncreaseValueOfJustAvoid);
				return;
			}

			//普通の回避
			if (_playerParam.GetIsAvoid)
			{
				_playerAvoid.OnAvoidSuccess.Invoke();
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
				SetRegenerate(false);
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


		public void Kill()
		{
			_currentHp = 0;
			OnDeath?.Invoke();
		}

		void Regeneration()
		{
			//リジェネ
			if (_isRegeneration && _initialHp > _currentHp)
			{
				_currentHp += _regenerationSpeed * Time.deltaTime;
				OnRegeneration?.Invoke();
			}
			else if(_isRegeneration)
			{
				_currentHp = _initialHp;
			}

			//リジェネの待ち時間
			if (_regenerationTimer > _regeneratWaitTime && _initialHp > _currentHp)
			{
				SetRegenerate(true);
				_regenerationTimer = 0;
			}
			else
			{
				_regenerationTimer += Time.deltaTime;
			}
		}

		void SetRegenerate(bool value) { _isRegeneration = value; }
	}
}
