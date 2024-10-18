using System;
using SgLibUnite.Singleton;
using Unity.VisualScripting;
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
        
        private Animator _animator;
        private AudioManager _audioManager;
        
        private int _parryID = Animator.StringToHash("IsParry");
        private int _avoidID = Animator.StringToHash("IsAvoid");
        private int _rightAttackID = Animator.StringToHash("RightAttack");
        private int _leftAttackID = Animator.StringToHash("LeftAttack");
        private int _dieID = Animator.StringToHash("IsDie");
        private int _takeDamageID = Animator.StringToHash("IsTakeDamage");
        private int _runID = Animator.StringToHash("Speed");

        private bool _isAttack;
        private bool _isAvoid;
        private bool _isJustAvoid;
        private bool _isDamage;
        private bool _isParry;
        private bool _isAnimation;
        private bool _isRun;
        private bool _isDie;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _audioManager = FindAnyObjectByType<AudioManager>();
        }

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

        /// <summary>死亡判定</summary>
        public bool GetIsDie => _isDie;


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
        
        /// <summary>死亡判定(変更用)</summary>
        public bool SetIsDie(bool value) => _isDie = value;
        
        //初期化用
        public void BoolInitialize()
        {
            _isAttack = false;
            _isAvoid = false;
            _isJustAvoid = false;
            _isDamage = false;
            _isParry = false;
            _isAnimation = false;
            _isRun = false;
            _isDie = false;
        }

        ///<summary>アニメータートリガーのリセット</summary>
        public void AnimatorInitialize()
        {
            _animator.ResetTrigger(_parryID);
            _animator.ResetTrigger(_avoidID);
            _animator.ResetTrigger(_rightAttackID);
            _animator.ResetTrigger(_leftAttackID);
            _animator.ResetTrigger(_dieID);
            _animator.ResetTrigger(_takeDamageID);
            _animator.SetFloat(_runID, 0f);
        }

        /// <summary>
        /// 歩行SE再生
        /// </summary>
        public void PlayWalkSE()
        {
            if(!_audioManager) return;
            _audioManager.PlaySE("PlayerWalk");
        }
        /// <summary>
        /// 走行SE再生
        /// </summary>
        public void PlayRunSE()
        {
            if(!_audioManager) return;
            _audioManager.PlaySE("PlayerRun");
        }
        /// <summary>
        /// ダメージSE再生
        /// </summary>
        public void PlayDamageSE()
        {
            if(!_audioManager) return;
            _audioManager.PlaySE("PLayerDamage");
        }
        /// <summary>
        /// 回避SE再生
        /// </summary>
        public void PlayAvoidSE()
        {
            if(!_audioManager) return;
            _audioManager.PlaySE("PlayerAvoid");
        }
        /// <summary>
        /// 右攻撃SE再生
        /// </summary>
        public void PlayAttack1SE()
        {
            if(!_audioManager) return;
            _audioManager.PlaySE("PlayerAttack1");
        }
        /// <summary>
        /// 左攻撃SE再生
        /// </summary>
        public void PlayAttack2SE()
        {
            if(!_audioManager) return;
            _audioManager.PlaySE("PlayerAttack2");
        }
        /// <summary>
        /// パリィSE再生
        /// </summary>
        public void PlayParrySE()
        {
            if(!_audioManager) return;
            _audioManager.PlaySE("PlayerParry");
        }
        /// <summary>
        /// ダメージVoice再生
        /// </summary>
        public void PlayDamageVoice()
        {
            if(!_audioManager) return;
            _audioManager.PlayVoice("PlayerDamageVoice");
        }
        /// <summary>
        /// ダウンVoice再生
        /// </summary>
        public void PlayDownVoice()
        {
            if(!_audioManager) return;
            _audioManager.PlayVoice("PlayerDown");
        }
        /// <summary>
        /// 右攻撃Voice再生
        /// </summary>
        public void PlayAttack1Voice()
        {
            if(!_audioManager) return;
            _audioManager.PlayVoice("PlayerAttack1Voice");
        }
        /// <summary>
        /// 左攻撃Voice再生
        /// </summary>
        public void PlayAttack2Voice()
        {
            if(!_audioManager) return;
            _audioManager.PlayVoice("PlayerAttack2Voice");
        }
    }
}
