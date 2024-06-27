using System;
using Player.Param;
using Player.Input;
using SgLibUnite.CodingBooster;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField, Range(0f, 10f)] private float _attackingRange;
	[Header("攻撃倍率の最大値、最小値")]
	[SerializeField, Range(0f, 2f)] private float _maxAttackScale = 0.8f;
	[SerializeField, Range(0f, 2f)] private float _minAttackScale = 1.2f;
	[SerializeField, Header("敵のレイヤー")] LayerMask _enemyLayerMask;
	[SerializeField, Header("右からの攻撃")] private ParticleSystem _rightAttack;
	[SerializeField, Header("左からの攻撃")] private ParticleSystem _leftAttack;
	
	//右側からの攻撃なら１、左側からの攻撃なら２
	private int _currentName; //モーション名
	Animator _animator;
	private PlayerParam _playerParam;
	private float _attackScale;

	private bool _isGiveDamage = false;

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
		var condition = Physics.OverlapSphere(transform.position+ Vector3.up , _attackingRange, _enemyLayerMask);
		if (condition != null)
		{
			IDamagedComponent iDamaged = null;
			foreach (var collider in condition)
			{
				if (collider.GetComponent<IDamagedComponent>() != null)
				{
					Debug.DrawRay(transform.position + Vector3.up, transform.forward, Color.green);
					_attackScale = Random.Range(_minAttackScale, _maxAttackScale);
					collider.GetComponent<IDamagedComponent>().AddDamage(_playerParam.GetInitialAtk * _attackScale);
					_isGiveDamage = true;
				}
				else if (collider.GetComponentInChildren<IDamagedComponent>() != null)
				{
					Debug.DrawRay(transform.position + Vector3.up, transform.forward, Color.green);
					_attackScale = Random.Range(_minAttackScale, _maxAttackScale);
					collider.GetComponentInChildren<IDamagedComponent>().AddDamage(_playerParam.GetInitialAtk * _attackScale);
					_isGiveDamage = true;
				}
				else if (collider.GetComponentInParent<IDamagedComponent>() != null)
				{
					Debug.DrawRay(transform.position + Vector3.up, transform.forward, Color.green);
					_attackScale = Random.Range(_minAttackScale, _maxAttackScale);
					collider.GetComponentInParent<IDamagedComponent>().AddDamage(_playerParam.GetInitialAtk * _attackScale);
					_isGiveDamage = true;
				}
			}
		}
		else
		{
			Debug.DrawRay(transform.position + Vector3.up, transform.forward, Color.red);
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
			_rightAttack.Play();
			_currentName = 1;
			_playerParam.SetIsAnimation(true);
			_rightAttack.Play();
		}
		else
		{
			//左からの攻撃
			_animator.SetTrigger("LeftAttack");
			_leftAttack.Play();
			_currentName = 2;
			_playerParam.SetIsAnimation(true);
			_leftAttack.Play();
		}
		PlayerInputsAction.Instance.RunCancel();
	}

	/// <summary>
	/// アタックフラグのリセットメソッド
	/// </summary>
	public void AttackClear()
	{
		_currentName = 0;
	}

	///<summary>アニメーションイベントで呼び出す用</summary>------------------------------------------------------------------
	public void AttackStart()
	{
		if (_playerParam.GetIsAttack)
		{
			return;
		}

		_playerParam.SetIsAttack(true);
	}

	public void EndAttack()
	{
		if (!_playerParam.GetIsAttack)
		{
			return;
		}
		_playerParam.SetIsAttack(false);
		_isGiveDamage = false;
		
		_rightAttack.Stop();
		_leftAttack.Stop();
	}

	///<summary>アニメーションの終わり</summary>
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

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, _attackingRange);
	}

}
