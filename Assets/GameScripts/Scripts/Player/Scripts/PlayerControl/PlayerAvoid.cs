using System;
using Player.Input;
using Player.Param;
using UnityEngine;

namespace Player.Action
{
    [RequireComponent(typeof(PlayerParam))]
    ///<summary>プレイヤーの回避</summary>
    public class PlayerAvoid : MonoBehaviour, IInitializableComponent
    {
        [SerializeField] private Animator _animator;

        private int _avoidId = Animator.StringToHash("IsAvoid");


        public System.Action OnAvoidSuccess;
        public System.Action<float> OnJustAvoidSuccess;

        private PlayerParam _playerParam;


        private void OnEnable()
        {
            OnAvoidSuccess += () => Debug.Log("回避成功");
            OnJustAvoidSuccess += _ => Debug.Log("ジャスト回避成功");
        }

        private void Start()
        {
            _playerParam = GetComponent<PlayerParam>();
        }

        private void Update()
        {
            if (PlayerInputsAction.Instance.GetCurrentInputType == PlayerInputTypes.Avoid &&
                !_playerParam.GetIsAnimation)
            {
                Avoided();
            }
        }

        void Avoided()
        {
            if (_playerParam.GetIsAvoid)
            {
                return;
            }

            _playerParam.SetIsAnimation(true);
            _animator.SetTrigger(_avoidId);
        }

        ///<summary>アニメーションイベントで呼び出す用</summary>------------------------------------------------------------------
        public void Avoid()
        {
            if (_playerParam.GetIsAvoid)
            {
                return;
            }

            _playerParam.SetIsAvoid(true);
        }

        public void JustAvoid()
        {
            if (_playerParam.GetIsJustAvoid)
            {
                return;
            }

            _playerParam.SetIsJustAvoid(true);
        }

        public void EndAvoid()
        {
            if (!_playerParam.GetIsAvoid)
            {
                return;
            }

            _playerParam.SetIsAvoid(false);
        }

        public void EndJustAvoid()
        {
            if (!_playerParam.GetIsJustAvoid)
            {
                return;
            }

            _playerParam.SetIsJustAvoid(false);
        }

        public void EndAvoidActions()
        {
            PlayerInputsAction.Instance.DeleteInputQueue(PlayerInputTypes.Avoid);
            _playerParam.SetIsAnimation(false);
            PlayerInputsAction.Instance.EndAction();
        }

        //------------------------------------------------------------------------------------------------------------------------------------
        public void InitializeThisComponent()
        {
        }

        public void FixedTickThisComponent()
        {
            //throw new System.NotImplementedException();
        }

        public void TickThisComponent()
        {
        }

        public void FinalizeThisComponent()
        {
            //throw new System.NotImplementedException();
        }

        public void PauseThisComponent()
        {
            throw new NotImplementedException();
        }

        public void ResumeThisComponent()
        {
            throw new NotImplementedException();
        }
    }
}
