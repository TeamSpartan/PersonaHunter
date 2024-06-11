using System;
using UnityEngine;

///<summary>summary</summary>
public class PlayerHpPresenter : MonoBehaviour
{
    private PlayerHpModel _playerHpModel;
    private PlayerHpView _playerHpView;

    private void Start()
    {
        _playerHpModel = GetComponent<PlayerHpModel>();
        _playerHpView = GetComponent<PlayerHpView>();

        _playerHpModel.OnReceiveDamage += _playerHpView.SetGauge;
        _playerHpModel.OnRegeneration += _playerHpView.SetRegenerate;
    }
}
