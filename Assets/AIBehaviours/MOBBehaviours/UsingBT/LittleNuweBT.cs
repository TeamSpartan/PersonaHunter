using System;
using System.Collections;
using System.Collections.Generic;
using AIBehaviours.MOBBehaviours.UsingBT.BTBehaviour;
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
    }
    
    private void StayAtCurrentPoint()
    {
        _agent.SetDestination(transform.position);
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
    }
    
    private void ChasePlayer()
    {
        _agent.SetDestination(_player.position);
    }
    
    private void Death()
    {
        _behaviourTree.PauseBT();
        GameObject.Destroy(gameObject);
    }

    #endregion

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
    }

    public void FinalizeThisComponent()
    {
        throw new System.NotImplementedException();
    }

    public void StartDull()
    {
        throw new System.NotImplementedException();
    }

    public void EndDull()
    {
        throw new System.NotImplementedException();
    }

    public void AddDamage(float dmg)
    {
        _health -= dmg;
    }

    public void Kill()
    {
        _health = 0;
    }

    private void Awake()
    {
        throw new NotImplementedException();
    }

    private void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackingRange);
    }
}
