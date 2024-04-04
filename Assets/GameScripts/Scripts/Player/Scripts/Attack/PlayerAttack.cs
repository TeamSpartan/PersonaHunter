using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //コンボアクション用
    public bool IsAttack; //攻撃用アクションフラグ
    private int _comboCount; //コンボ判定用フラグ
    private string _currentName; //モーション名
    [SerializeField, Tooltip("")]

    /// <summary>
    /// 攻撃時に呼ばれる処理
    /// </summary>
    public void Attack()
    {
        if(_comboCount < 2)
        {
            if(_currentName == null )
            {
                
            }
            else if()
        }
    }

    /// <summary>
    /// アタックフラグのリセットメソッド
    /// </summary>
    public void AttackClear()
    {
        IsAttack = false;
    }

    /// <summary>
    /// コンボフラグをリセットメソッド
    /// </summary>
    public void ClearCombo()
    {
        _comboCount = 0;
    }
}
