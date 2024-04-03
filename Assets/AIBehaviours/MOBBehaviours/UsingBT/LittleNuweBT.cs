using System;
using System.Collections;
using System.Collections.Generic;
using SgLibUnite.AI;
using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
/// <summary> オモテガリ 敵モブ BT </summary>
public class LittleNuweBT
    : MonoBehaviour
        , IMobBehaviourParameter
        , IInitializableComponent
        , IDulledTarget
        , IDamagedComponent
{
    #region Component Parameter

    [SerializeField, Header("Patrolling Path")]
    private PatrollerPathContainer _path;

    [SerializeField, Header("Health")] private float _health;

    [SerializeField, Header("Player Tag")] private string _playerTag;

    [SerializeField, Range(1f, 100f)] private float _sightRange;

    [SerializeField, Range(1f, 50f)] private float _attackingRange;

    [SerializeField, Header("Base Damage")]
    private float _baseDamage;

    [SerializeField, Header("Interval To Attack[sec]")]
    private float _attackInterval;

    [SerializeField, Header("LayerMask Of Player")]
    private LayerMask _playerLayerMask;

    [SerializeField, Header("PatrollingTime To Take A Break")]
    private float _timeToBreak;

    [SerializeField, Header("Breaking Time To Back Patrolling")]
    private float _timeToPatrol;
    
    #endregion

    #region Behaviours

    private BTBehaviour _patrolBehaviour;
    private BTBehaviour _idleBehaviour;
    private BTBehaviour _chaseBehaviour;
    private BTBehaviour _attackBehaviour;
    private BTBehaviour _deathBehaviour;

    #endregion

    private BehaviourTree _behaviourTree;
    private Animator _animator;
    private Transform _player;
    private NavMeshAgent _agent;
    private int _pathIndex;

    private string _btTPatToChase = "btt1";
    private string _btTPatToIdle = "btt2";
    private string _btTChaseToAtk = "btt3";

    [SerializeField]
    private bool _foundPlayer;
    [SerializeField]
    private bool _takeABreak;
    [SerializeField]
    private bool _playerInsideAttackRange;

    [SerializeField]
    private float _elapsedTimePatroling = 0f;
    [SerializeField]
    private float _elapsedTimeBreaking = 0f;

    #region BehavioursStateFunctions

    private void Patrol()
    {
        var Path = _path.GetPatrollingPath;
        var destination = Path[_pathIndex];
        if (Vector3.Distance(transform.position, destination) < 2f)
        {
            if (_pathIndex + 1 < Path.Length) _pathIndex++;
            else _pathIndex = 0;
            destination = Path[_pathIndex];
        }

        _agent.SetDestination(destination);

        _elapsedTimePatroling += Time.deltaTime;

        if (_elapsedTimePatroling > _timeToBreak)
        {
            _elapsedTimePatroling = 0f;
            _takeABreak = true;
        }
        else
        {
            _takeABreak = false;
        }
        
        Debug.Log($"Patrolling");
    }

    private void StayAtCurrentPoint()
    {
        _agent.SetDestination(transform.position);
        _elapsedTimeBreaking += Time.deltaTime;

        if (_elapsedTimeBreaking > _timeToPatrol)
        {
            _elapsedTimeBreaking = 0f;
            _behaviourTree.JumpTo(_patrolBehaviour);
        }
        
        Debug.Log("StayAtHere");
    }

    private void AttackToPlayer()
    {
        var condition = Physics.CheckSphere(transform.position, _attackingRange, _playerLayerMask);
        if (condition)
        {
            var player = _player.gameObject;
            if (player.GetComponent<IDamagedComponent>() != null)
            {
                player.GetComponent<IDamagedComponent>().AddDamage(_baseDamage);
            }
        }
        else
        {
            _behaviourTree.JumpTo(_chaseBehaviour);
        }
        
        Debug.Log("AttackToPlayer");
    }

    private void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, _player.position) < _sightRange)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            _behaviourTree.JumpTo(_patrolBehaviour);
        }
        
        Debug.Log("Chasing Player");
    }

    private void Death()
    {
        _behaviourTree.PauseBT();
        GameObject.Destroy(gameObject);
        
        Debug.Log($"Death");
    }

    #endregion

    private void SetupBehaviours()
    {
        _patrolBehaviour = new();
        _patrolBehaviour.AddBehaviour(Patrol);

        _idleBehaviour = new();
        _idleBehaviour.AddBehaviour(StayAtCurrentPoint);

        _chaseBehaviour = new();
        _chaseBehaviour.AddBehaviour(ChasePlayer);

        _attackBehaviour = new();
        _attackBehaviour.AddBehaviour(AttackToPlayer);

        _deathBehaviour = new();
        _deathBehaviour.AddBehaviour(Death);
    }

    private void SetupTransitions()
    {
        _behaviourTree.MakeTransition(_patrolBehaviour, _chaseBehaviour, _btTPatToChase);
        _behaviourTree.MakeTransition(_patrolBehaviour, _idleBehaviour, _btTPatToIdle);
        _behaviourTree.MakeTransition(_chaseBehaviour, _attackBehaviour, _btTChaseToAtk);
    }

    private void UpdateConditions()
    {
        
    }

    public float GetHealth()
    {
        return _health;
    }

    public void SetHealth(float val)
    {
        _health = val;
    }

    public void InitializeThisComponent()
    {
        _behaviourTree = new BehaviourTree();

        _agent = GetComponent<NavMeshAgent>();

        if (gameObject.GetComponent<Animator>() != null)
        {
            _animator = GetComponent<Animator>();
        }
        else if (gameObject.GetComponentInChildren<Animator>() != null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        SetupBehaviours();

        _behaviourTree.ResistBehaviours(new[]
        {
            _patrolBehaviour, _idleBehaviour, _chaseBehaviour, _attackBehaviour, _deathBehaviour
        });
        
        SetupTransitions();
        
        _behaviourTree.StartBT();
    }

    public void FinalizeThisComponent()
    {
    }

    public void StartDull()
    {
    }

    public void EndDull()
    {
    }

    public void AddDamage(float dmg)
    {
        _health -= dmg;
    }

    public void Kill()
    {
        _health = 0;
        _behaviourTree.JumpTo(_deathBehaviour);
    }

    private void FixedUpdate()
    {
        if (_health <= 0)
        {
            _behaviourTree.JumpTo(_deathBehaviour);
        }
        
        _behaviourTree.UpdateTransition(_btTPatToChase, ref _foundPlayer);
        _behaviourTree.UpdateTransition(_btTChaseToAtk, ref _playerInsideAttackRange);
        _behaviourTree.UpdateTransition(_btTPatToIdle, ref _takeABreak);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackingRange);
    }
}
