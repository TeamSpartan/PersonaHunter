using System;
using System.Collections;
using System.Collections.Generic;
using SgLibUnite.AI;
using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

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

    private BehaviourTree _behaviourTree;
    private Animator _animator;
    private Transform _player;
    private NavMeshAgent _agent;

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
        throw new System.NotImplementedException();
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
