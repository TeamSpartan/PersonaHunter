using UnityEngine;

public class PlayerHp : MonoBehaviour, IDamagedComponent
{
	[SerializeField, Header("初期HP")] private int initialHp;
	[SerializeField, Header("無敵フレーム")] private int invincibilityFrame;

	private System.Action _onDeath,
		_onReceiveDamage,
		_onReceiveHeal,
		_onHitWhileInvulnerable,
		_onBecomeVulnerable,
		_onResetDamage;

	private int _currentHp;
	private bool _isInvulnerable;
	private int _frameSinceLastHit;

	private void Start()
	{
		ResetDamage();
	}

	private void OnEnable()
	{
		_onResetDamage += () => Debug.Log("ResetDamage");
		_onReceiveDamage += () => Debug.Log("ReceiveDamage");
		_onBecomeVulnerable += () => Debug.Log("BecomeVulnerable");
		_onReceiveHeal += () => Debug.Log("ReceiveHeal");
		_onHitWhileInvulnerable += () => Debug.Log("HitWhileInvulnerable");
		_onDeath+= () => Debug.Log("Death");
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		//無敵時間中
		if (_isInvulnerable)
		{
			if (_frameSinceLastHit > invincibilityFrame)
			{
				_frameSinceLastHit = 0;
				_isInvulnerable = false;
				_onBecomeVulnerable?.Invoke();
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
		_currentHp = initialHp;
		_onResetDamage?.Invoke();
	}

	public void AddDamage(float dmg)
	{
		if (_currentHp <= 0)
		{
			return;
		}

		//Received damage while invincible
		if (_isInvulnerable)
		{
			_onHitWhileInvulnerable?.Invoke();
			return;
		}

		_currentHp -= (int)dmg;
		if (dmg > 0)
		{
			//Processing when HP decreases
			_onReceiveDamage?.Invoke();
			_isInvulnerable = true;
		}
		else
		{
			//Processing when HP increases
			_onReceiveHeal?.Invoke();
		}

		//at time of death
		if (_currentHp <= 0)
			_onDeath?.Invoke();
	}

	public void Kill()
	{
		_currentHp = 0;
	}
}
