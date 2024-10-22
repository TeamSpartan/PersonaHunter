using System;
using Player.Input;
using Player.Param;
using Player.Zone;
using SgLibUnite.CodingBooster;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Action
{
	[RequireComponent(typeof(PlayerParam))]
	///<summary>プレイヤーのパリー</summary>
	public class PlayerParry : MonoBehaviour, IAbleToParry
	{
		[SerializeField] private ParticleSystem _parrySucceedEffect;

		private int _parryID = Animator.StringToHash("IsParry");
		private PlayerParam _playerParam;
		private Animator _animator;
		private CBooster _cBooster = new();
		private ZoneObj _zoneObj;

		private void Start()
		{
			_playerParam = GetComponent<PlayerParam>();
			_animator = GetComponent<Animator>();
			_zoneObj = GetComponentInChildren<ZoneObj>();
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
			if (_playerParam.GetIsAnimation)
			{
				return;
			}

			PlayerInputsAction.Instance.RunCancel();
			_playerParam.BoolInitialize();
			_playerParam.AnimatorInitialize();
			_playerParam.SetIsAnimation(true);
			_animator.SetTrigger(_parryID);
		}

		//パリィの成功
		public void ParrySuccess()
		{
			_zoneObj.IncreaseGaugeValue(_playerParam.GetGiveValueOfParry);
			_parrySucceedEffect.Play();
			_playerParam.PlayParrySE();
		}

		///<summary>アニメーションイベントで呼び出す用</summary>------------------------------------------------------------------
		public void Parry()
		{
			if (_playerParam.GetIsParry)
			{
				_playerParam.SetIsParry(false);
				return;
			}

			_playerParam.BoolInitialize();
			_playerParam.SetIsParry(true);
			_playerParam.SetIsAnimation(true);
		}

		public void EndParry()
		{
			if (!_playerParam.GetIsParry)
			{
				return;
			}

			_playerParam.SetIsParry(false);
		}

		public void EndParryActions()
		{
			PlayerInputsAction.Instance.DeleteInputQueue(PlayerInputTypes.Parry);
			_playerParam.SetIsAnimation(false);
			PlayerInputsAction.Instance.EndAction();
		}

		public bool NotifyPlayerIsGuarding()
		{
			return _playerParam.GetIsParry;
		}
	}
}
