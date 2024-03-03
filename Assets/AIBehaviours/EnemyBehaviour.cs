using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgLibUnite.StateSequencer;

public abstract class EnemyBehaviour : MonoBehaviour
{
   /// <summary>
   /// 体力値を返す
   /// </summary>
   /// <returns></returns>
   public abstract float GetHealth();
   /// <summary>
   /// 体力値を初期化する
   /// </summary>
   /// <param name="val"></param>
   public abstract void SetHealth(float val);
   /// <summary>
   /// ひるみ値を返す
   /// </summary>
   /// <returns></returns>
   public abstract float GetFlinchValue();
   /// <summary>
   /// ひるみ値を初期化する
   /// </summary>
   /// <param name="val"></param>
   public abstract void SetFlinchValue(float val);
}
