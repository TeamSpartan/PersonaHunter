using System;
using Player.Input;
using Player.Param;
using Player.Zone;
using SgLibUnite.CodingBooster;
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
		private CBooster _cBooster = new();
		private ZoneObj _zoneObj;
		private void OnEnable()
		{
			
		}

		private void Start()
		{
			_playerParam = GetComponentInParent<PlayerParam>();
			_animator = GetComponentInParent<Animator>();
			_zoneObj = GetComponent<ZoneObj>();
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

		void ParrySuccess()
		{
			_zoneObj.IncreaseGaugeValue(_playerParam.GetGiveValueOfParry);
		}

		///<summary>アニメーションイベントで呼び出す用</summary>------------------------------------------------------------------
		public void Parry()
		{
			if (_playerParam.GetIsParry)
			{
				OnDuringParry?.Invoke();
				return;
			}
			foreach (var IGuard in _cBooster.GetDerivedComponents<IAbleToParry>())
			{
				IGuard.EParrySucceed += ParrySuccess;
				IGuard.NotifyPlayerIsGuarding(this.gameObject);
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
			foreach (var IGuard in _cBooster.GetDerivedComponents<IAbleToParry>())
			{
				IGuard.EParrySucceed -= ParrySuccess;
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
		//------------------------------------------------------------------------------------------------------------------------------------------------------------

		void DebugComment(string comment)
		{
			Debug.Log(comment);
		}
	}
}
