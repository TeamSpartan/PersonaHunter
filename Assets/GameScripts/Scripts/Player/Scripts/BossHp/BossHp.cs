using UnityEngine;

public class BossHp : MonoBehaviour, IDamagedComponent
{
	[SerializeField] private float initiateHp = 1000f;
	private float _currentHp;

	public float InitiateHp => initiateHp;
	public float CurrentHp => _currentHp;

	public event System.Action OnDeath,
		OnReceiveDamage,
		OnResetDamage;


	private void OnEnable()
	{
		#region initiateAction
		OnResetDamage += () => Debug.Log("ResetDamage");
		OnReceiveDamage += () => Debug.Log("ReceiveDamage");
		OnDeath += () => Debug.Log("Death");
		#endregion
	}

	public void ResetDamage()
	{
		_currentHp = initiateHp;
		OnResetDamage.Invoke();
	}

	public void AddDamage(float dmg)
	{
		_currentHp -= dmg;
		OnReceiveDamage.Invoke();

		if (_currentHp <= 0)
		{
			OnDeath.Invoke();
		}
	}

	public void Kill()
	{
		_currentHp = 0;
	}
}
