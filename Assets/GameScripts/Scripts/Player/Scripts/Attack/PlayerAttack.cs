using System;
using System.Linq;
using Player.Param;
using Player.Input;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	//右側からの攻撃なら１、左側からの攻撃なら２
	private int _currentName; //モーション名
	Animator _animator;

	private PlayerParam _playerParam;
	[SerializeField, Range(1f, 50f)] private float _attackingRange;

	[SerializeField, Header("LayerMask Of Enemy")]
	private LayerMask _enemyLayerMask;

	private bool _isGiveDamage = false;

	public event System.Action OnAttackStart,
		OnAttackSuccess,
		OnEndAttack,
		OnDuringAttack;

	private void Start()
	{
		_animator = GetComponent<Animator>();
		_playerParam = GetComponent<PlayerParam>();
	}

	private void Update()
	{
		if (PlayerInputsAction.Instance.GetCurrentInputType == PlayerInputTypes.Attack && !_playerParam.GetIsAnimation)
		{
			Attack();
		}
	}

	private void FixedUpdate()
	{
		if (_playerParam.GetIsAttack && !_isGiveDamage)
		{
			AttackToEnemy();
		}
	}

	private void AttackToEnemy()
	{
		var condition = Physics.OverlapSphere(transform.position, _attackingRange, _enemyLayerMask);
		if (condition != null)
		{
			IDamagedComponent iDamaged = null;
			foreach (var collider in condition)
			{
				if (collider.gameObject.TryGetComponent(out iDamaged))
				{
					Debug.Log("Damage");
					collider.GetComponent<IDamagedComponent>().AddDamage(_playerParam.GetInitialAtk);
					_isGiveDamage = true;
				}
			}
		}
		else
		{
			Debug.DrawRay(transform.position + Vector3.up, transform.forward * 100, Color.red);
		}
	}

	/// <summary>
	/// 攻撃時に呼ばれる処理
	/// </summary>
	public void Attack()
	{
		if (_currentName != 1)
		{
			//右からの攻撃
			_animator.SetTrigger("RightAttack");
			_currentName = 1;
			_playerParam.SetIsAnimation(true);
		}
		else
		{
			//左からの攻撃
			_animator.SetTrigger("LeftAttack");
			_currentName = 2;
			_playerParam.SetIsAnimation(true);
		}
	}

	/// <summary>
	/// アタックフラグのリセットメソッド
	/// </summary>
	public void AttackClear()
	{
		_currentName = 0;
	}

	// private void OnDrawGizmos()
	// {
	// 	Gizmos.color = Color.blue;
	// 	Gizmos.DrawWireSphere(transform.position, _attackingRange);
	// }

	///<summary>アニメーションイベントで呼び出す用</summary>------------------------------------------------------------------
	public void AttackStart()
	{
		if (_playerParam.GetIsAttack)
		{
			OnDuringAttack?.Invoke();
			return;
		}

		_playerParam.SetIsAttack(true);
		OnAttackStart?.Invoke();
	}

	public void EndAttack()
	{
		if (!_playerParam.GetIsAttack)
		{
			return;
		}

		OnEndAttack?.Invoke();
		_playerParam.SetIsAttack(false);
		_isGiveDamage = false;
	}

	public void EndAttackActions()
	{
		//連続攻撃
		if (PlayerInputsAction.Instance.CheckInputQueue() == PlayerInputTypes.Attack)
		{
			Attack();
			PlayerInputsAction.Instance.DeleteInputQueue(PlayerInputTypes.Attack);
		}
		else
		{
			PlayerInputsAction.Instance.DeleteInputQueue(PlayerInputTypes.Attack);
			_playerParam.SetIsAnimation(false);
			AttackClear();
			PlayerInputsAction.Instance.EndAction();
		}
	}
}
