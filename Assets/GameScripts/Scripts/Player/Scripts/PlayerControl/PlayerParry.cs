using Player.Input;
using Player.Param;
using UnityEngine;

namespace Player.Action
{
	[RequireComponent(typeof(PlayerParam))]
	[RequireComponent(typeof(Animator))]
	///<summary>プレイヤーのパリー</summary>
	public class PlayerParry : MonoBehaviour
	{
		public event System.Action OnParryStart,
			OnParrySuccess,
			OnEndParry,
			OnDuringParry;

		private int _parryID = Animator.StringToHash("IsParry");
		private PlayerParam _playerParam;
		private Animator _animator;

		private void Start()
		{
			_playerParam = GetComponentInParent<PlayerParam>();
			_animator = GetComponentInParent<Animator>();
		}

		private void Update()
		{
			if (PlayerInputsAction.Instance.GetCurrentInputType == PlayerInputTypes.Parry &&
			    !_playerParam.GetIsAnimation)
			{
				Parried();
			}
		}

		void Parried()
		{
			if (_playerParam.GetIsParry)
			{
				OnDuringParry?.Invoke();
				return;
			}

			_playerParam.SetIsAnimation(true);
			_animator.SetTrigger(_parryID);
		}


		///<summary>アニメーションイベントで呼び出す用</summary>------------------------------------------------------------------
		public void Parry()
		{
			if (_playerParam.GetIsParry)
			{
				OnDuringParry?.Invoke();
				return;
			}

			_playerParam.SetIsParry(true);
			OnParryStart?.Invoke();
		}

		public void EndParry()
		{
			if (!_playerParam.GetIsParry)
			{
				return;
			}

			OnEndParry?.Invoke();
			_playerParam.SetIsParry(false);
		}

		public void EndParryActions()
		{
			PlayerInputsAction.Instance.DeleteInputQueue(PlayerInputTypes.Parry);
			_playerParam.SetIsAnimation(false);
			PlayerInputsAction.Instance.EndAction();
		}
	}
}
