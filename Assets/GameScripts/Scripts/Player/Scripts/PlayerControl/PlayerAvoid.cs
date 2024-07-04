using System;
using DG.Tweening;
using Player.Input;
using Player.Param;
using UnityEngine;

namespace Player.Action
{
	[RequireComponent(typeof(PlayerParam))]
	///<summary>プレイヤーの回避</summary>
	public class PlayerAvoid : MonoBehaviour
	{
		[SerializeField, Header("回避距離")] private float _avoidRange = 5f;
		[SerializeField] private float _maxDrag = 20f;

		[SerializeField, Header("空気抵抗をMaxまでする時間")]
		private float _duration = 1.5f;
		public System.Action OnAvoidSuccess;
		public System.Action<float> OnJustAvoidSuccess;

		private PlayerParam _playerParam;
		private Animator _animator;
		private Vector3 _dir;
		private Vector2 _saveInputValue;
		private Rigidbody _rb;
		private PlayerMove _playerMove;
		private Tween _tween;
		private AfterImageSkinnedMeshController _afterImageController;
		private int _avoidId = Animator.StringToHash("IsAvoid");
		private float _drag;
		
		private void Start()
		{
			_animator = GetComponent<Animator>();
			_playerParam = GetComponent<PlayerParam>();
			_playerMove = GetComponent<PlayerMove>();
			_rb = GetComponent<Rigidbody>();
			_afterImageController = GetComponent<AfterImageSkinnedMeshController>();
			_drag = _rb.drag;
		}

		private void Update()
		{
			if (PlayerInputsAction.Instance.GetCurrentInputType == PlayerInputTypes.Avoid &&
			    !_playerParam.GetIsAnimation)
			{
				Avoided();
			}

			//移動の処理
			if (PlayerInputsAction.Instance.GetCurrentInputType == PlayerInputTypes.Avoid)
			{
				if (_saveInputValue == Vector2.zero)
					_dir = transform.forward;
				else
				{
					_dir = new Vector3(_saveInputValue.x, 0, _saveInputValue.y);
					_dir = Camera.main.transform.TransformDirection(_dir);
				}
				_dir.y = 0;
				_dir = _dir.normalized;
				//_rb.velocity = _playerMove.GetSlopeMoveDirection(_dir * _avoidRange, _playerMove.NormalRay());
				_rb.AddForce(_playerMove.GetSlopeMoveDirection(_dir * _avoidRange, _playerMove.NormalRay()),
					ForceMode.Impulse);
				_playerMove.PlayerRotate(_dir);
			}
		}

		void Avoided()
		{
			if (_playerParam.GetIsAnimation)
			{
				return;
			}

			_saveInputValue = PlayerInputsAction.Instance.GetMoveInput();
			_playerParam.BoolInitialize();
			_playerParam.AnimatorInitialize();
			_playerParam.SetIsAnimation(true);
			_animator.SetTrigger(_avoidId);
			_afterImageController.IsCreate = true;

			 _tween = DOTween.To(() => _rb.drag,
				x => _rb.drag = x,
				_maxDrag,
				_duration).OnComplete(() => _rb.drag = 10f);
		}

		///<summary>アニメーションイベントで呼び出す用</summary>------------------------------------------------------------------
		public void Avoid()
		{
			if (_playerParam.GetIsAvoid)
			{
				return;
			}

			_playerParam.BoolInitialize();
			_playerParam.SetIsAnimation(true);
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
			_tween.Kill();
			_afterImageController.IsCreate = false;
			_rb.drag = _drag;
			PlayerInputsAction.Instance.DeleteInputQueue(PlayerInputTypes.Avoid);
			_playerParam.SetIsAnimation(false);
			PlayerInputsAction.Instance.EndAction();
			
		}

		//------------------------------------------------------------------------------------------------------------------------------------

	}
}
