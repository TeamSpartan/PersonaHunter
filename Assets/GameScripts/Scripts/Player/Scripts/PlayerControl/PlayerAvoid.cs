using Player.Input;
using Player.Param;
using UnityEngine;

namespace Player.Action
{
	[RequireComponent(typeof(PlayerParam))]
	[RequireComponent(typeof(Animator))]
	///<summary>プレイヤーの回避</summary>
	public class PlayerAvoid : MonoBehaviour
	{
		
		private int _avoidId = Animator.StringToHash("IsAvoid");

		public event System.Action OnAvoidStart,
			OnJustAvoidStart,
			OnEvading,
			OnJustEvading,
			OnEndAvoid,
			OnEndJustAvoid;


		public System.Action OnAvoidSuccess;
		public System.Action<float> OnJustAvoidSuccess;

		private PlayerParam _playerParam;
		private Animator _animator;


		private void OnEnable()
		{
			OnAvoidSuccess += () => Debug.Log("回避成功");
			OnJustAvoidSuccess += _ => Debug.Log("ジャスト回避成功");
			OnEvading += () => Debug.Log("回避中");
			OnJustEvading += ()=> Debug.Log("ジャスト回避中");
		}

		private void Start()
		{
			_animator = GetComponent<Animator>();
			_playerParam = GetComponent<PlayerParam>();
			Initialization();
		}

		void Initialization()
		{
			
		}

		private void Update()
		{
			if (PlayerInputsAction.Instance.GetCurrentInputType == PlayerInputTypes.Avoid && !_playerParam.GetIsAnimation)
			{
				Avoided();
			}
		}

		void Avoided()
		{
			if (_playerParam.GetIsAvoid)
			{
				OnEvading?.Invoke();
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
				OnEvading?.Invoke();
				return;
			}

			_playerParam.SetIsAvoid(true);
			OnAvoidStart?.Invoke();
		}

		public void JustAvoid()
		{
			if (_playerParam.GetIsJustAvoid)
			{
				OnJustEvading?.Invoke();
				return;
			}

			_playerParam.SetIsJustAvoid(true);
			OnJustAvoidStart?.Invoke();
		}

		public void EndAvoid()
		{
			if (!_playerParam.GetIsAvoid)
			{
				return;
			}
			
			OnEndAvoid?.Invoke();
			_playerParam.SetIsAvoid(false);
			
		}

		public void EndJustAvoid()
		{
			if (!_playerParam.GetIsJustAvoid)
			{
				return;
			}
			
			OnEndJustAvoid?.Invoke();
			_playerParam.SetIsJustAvoid(false);
		}

		public void EndAvoidActions()
		{
			PlayerInputsAction.Instance.DeleteInputQueue(PlayerInputTypes.Avoid);
			_playerParam.SetIsAnimation(false);
			PlayerInputsAction.Instance.EndAction();
		}
		//------------------------------------------------------------------------------------------------------------------------------------
	}
}
