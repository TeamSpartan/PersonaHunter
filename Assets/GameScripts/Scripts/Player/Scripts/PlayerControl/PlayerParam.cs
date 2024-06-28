using System;
using SgLibUnite.Singleton;
using UnityEngine;

namespace Player.Param
{
    ///<summary>PlayerParam</summary>
    public class PlayerParam : MonoBehaviour
    {
        [SerializeField, Header("初期HP")] private float initialHp = 5f;
        [SerializeField, Header("初期攻撃力")] private float initialAtk = 10f;

        [SerializeField, Header("ジャスト回避で増加するゾーンゲージの量")]
        private float increaseValueOfJustAvoid = 1f;

        [SerializeField, Header("パリィで与える体感値")] private float giveValueOfParry = 5f;

        private bool _isAttack;
        private bool _isAvoid;
        private bool _isJustAvoid;
        private bool _isDamage;
        private bool _isParry;
        private bool _isAnimation;
        private bool _isRun;

        //=================参照用==========================================================
        ///<summary>初期HP(参照用)</summary>
        public float GetInitialHp => initialHp;

        ///<summary>初期攻撃力（参照用）</summary>
        public float GetInitialAtk => initialAtk;

        ///<summary>ジャスト回避で増加するゾーンゲージの量</summary>
        public float GetIncreaseValueOfJustAvoid => increaseValueOfJustAvoid;

        ///<summary>パリィで増加する体幹値の量</summary>
        public float GetGiveValueOfParry => giveValueOfParry;

        ///<summary>攻撃中の判定</summary>
        public bool GetIsAttack => _isAttack;

        ///<summary>回避中の判定</summary>
        public bool GetIsAvoid => _isAvoid;

        ///<summary>ジャスト回避中の判定</summary>
        public bool GetIsJustAvoid => _isJustAvoid;

        ///<summary>受ダメージ中の判定</summary>
        public bool GetIsDamage => _isDamage;

        ///<summary>パリィ中の判定</summary>
        public bool GetIsParry => _isParry;

        ///<summary>アニメーション中の判定</summary>
        public bool GetIsAnimation => _isAnimation;

        ///<summary>走り状態の判定</summary>
        public bool GetIsRun => _isRun;


        //================変更用============================================
        ///<summary>攻撃中（変更用）</summary>
        public bool SetIsAttack(bool value) => _isAttack = value;

        ///<summary>回避中（変更用）</summary>
        public bool SetIsAvoid(bool value) => _isAvoid = value;

        ///<summary>回避中（変更用）</summary>
        public bool SetIsJustAvoid(bool value) => _isJustAvoid = value;

        ///<summary>受ダメージ中（変更用）</summary>
        public bool SetIsDamage(bool value) => _isDamage = value;

        ///<summary>パリィ中（変更用）</summary>
        public bool SetIsParry(bool value) => _isParry = value;

        ///<summary>アニメーション中の判定</summary>
        public bool SetIsAnimation(bool value) => _isAnimation = value;

        ///<summary>走る判定（変更用）</summary>
        public bool SetIsRun(bool value) => _isRun = value;
    }
}
