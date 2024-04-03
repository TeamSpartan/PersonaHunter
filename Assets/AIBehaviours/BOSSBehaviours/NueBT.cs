using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

// 作成 菅沼
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

    [SerializeField, Header("Awaiting Time When Get Parry")]
    private float _awaitOnParry;

    [SerializeField, Header("Awaiting Time When Get Flinch")]
    private float _awaitOnFlinching;

    [SerializeField, Range(1f, 100f), Header("Sight Range")]
    private float _sightRange;

    [SerializeField, Range(1f, 100f), Header("Attacking Range")]
    private float _attackingRange;

    #endregion

    #region States

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

    private void GetParry()
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
}
