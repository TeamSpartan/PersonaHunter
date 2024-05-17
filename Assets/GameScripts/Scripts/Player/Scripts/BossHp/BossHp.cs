using UnityEngine;

public class BossHp : MonoBehaviour, IDamagedComponent
{
	[SerializeField] private float _initiateHp = 1000f;
	private float _currentHp;

	public float InitiateHp => _initiateHp;
	public float CurrentHp => _currentHp;

	public event System.Action OnDeath,
		OnReceiveDamage,
		OnResetDamage;


	protected void OnEnable()
	{
		#region initiateAction
		OnResetDamage += () => Debug.Log("ResetDamage");
		OnReceiveDamage += () => Debug.Log("ReceiveDamage");
		OnDeath += () => Debug.Log("Death");
		#endregion
	}

	public void ResetDamage()
	{
		_currentHp = _initiateHp;
		OnResetDamage?.Invoke();
	}

	public void AddDamage(float dmg)
	{
		_currentHp -= dmg;
		OnReceiveDamage?.Invoke();

		if (_currentHp <= 0)
		{
			OnDeath?.Invoke();
		}
	}

	public void Kill()
	{
		_currentHp = 0;
	}
}
