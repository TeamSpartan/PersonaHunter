using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //右側からの攻撃なら１、左側からの攻撃なら２
    private int _currentName; //モーション名
    /// <summary>入力フラグ </summary>
    private bool IsInput;
    /// <summary>入力フラグ </summary>
    private bool IsStart;
    [SerializeField] Animator _animator;

    /// <summary>
    /// 攻撃時に呼ばれる処理
    /// </summary>
    public void Attack()
    {
        //モーション中の入力が２回目なら処理を行わない
        if (IsInput)
            return;

        if(_currentName != 1)
        {
            //右からの攻撃
            _animator.SetTrigger("RightAttack");
            _currentName = 1;
            
            if(!IsStart)
            {
                IsStart = true;
            }//最初の攻撃の場合
            else
            {
                IsInput = true;
            }
        }
        else 
        {
            //左からの攻撃
            _animator.SetTrigger("LeftAttack");
            _currentName = 2;
            IsInput = true;
        }
    }

    /// <summary>
    /// アタックフラグのリセットメソッド
    /// </summary>
    public void AttackClear()
    {
        _currentName = 0;
        IsStart = false;
    }

    /// <summary>
    /// モーションが終わった後に呼び出す処理
    /// </summary>
    public void InputReset()
    {
        
        IsStart = false;
    }
}
