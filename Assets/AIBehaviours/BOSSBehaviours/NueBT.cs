using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

// 作成 菅沼
[RequireComponent(typeof(NavMeshAgent))]
/// <summary> オモテガリ 鵺 </summary>
public class NueBT
    : MonoBehaviour
        , IMobBehaviourParameter
        , IInitializableComponent
        , IDulledTarget
        , IDamagedComponent
{
    #region Parameter Exposing

    [SerializeField, Header("Health")] private float _health;

    [SerializeField, Header("Flinch Threshold")]
    private float _flinchThreshold;

    [SerializeField, Header("Awaiting Time When Get Parry[sec]")]
    private float _awaitOnParry;

    [SerializeField, Header("Awaiting Time When Get Flinch[sec]")]
    private float _awaitOnFlinching;

    [SerializeField, Range(1f, 100f), Header("Sight Range")]
    private float _sightRange;

    [SerializeField, Range(1f, 100f), Header("Attacking Range")]
    private float _attackingRange;

    #endregion

    #region Behaviours

    private BTBehaviour _btbIdle;
    private BTBehaviour _btbGetClose;
    private BTBehaviour _btbSelectAttackType;
    private BTBehaviour _btbClaw;
    private BTBehaviour _btbTale;
    private BTBehaviour _btbRush;
    private BTBehaviour _btbDeath;
    private BTBehaviour _btbFlinch;
    private BTBehaviour _btbStumble;

    #endregion

    private BehaviourTree _behaviourTree;
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;

    #region States

    private void SelectAttackBehaviour()
    {
        
    }

    private void Claw()
    {
    }

    private void Tale()
    {
    }

    private void Rush()
    {
    }

    private void Death()
    {
    }

    private void Flinch()
    {
    }

    private void Stumble()
    {
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
    }

    private void FixedUpdate()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackingRange);
    }
}
