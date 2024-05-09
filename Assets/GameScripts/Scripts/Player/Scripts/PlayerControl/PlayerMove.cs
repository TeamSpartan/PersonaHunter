using System;
using DG.Tweening;
using Player.Input;
using Player.Param;
using PlayerCam.Scripts;
using Sound;
using Sound.PlayOption;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Action
{
	//Rigidbodyを強制的にアタッチする
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMove : MonoBehaviour, IPlayerCameraTrasable, IInitializableComponent
	{
		/// <summary>プレイヤーのRigidbody </summary>
		Rigidbody _rb;

		private ReactiveProperty<Vector3> saveDir = new();

		/// <summary>プレイヤーの移動方向 </summary>
		Vector3 _dir;

		/// <summary>プレイヤーの進行方向を保存する </summary>
		Quaternion _targetRotation;

		[SerializeField, Tooltip("プレイヤーの回転速度")]
		float rotateSpeed = 10f;

		[SerializeField, Tooltip("プレイヤーの移動速度")]
		float moveSpeed = 10f;

		[Header("接地判定")] [SerializeField, Tooltip("接地判定Y方向のオフセット"), Range(0, -1)]
		float groundOffSetY = -0.14f;

		[SerializeField, Tooltip("接地判定の半径"), Range(0, 1)]
		float groundRadius = 0.5f;

		[SerializeField, Header("重力の大きさ")] private float _gravityvalue = 10f;

		[SerializeField, Header("レイの長さ")] private float _raylength = 0.3f;

		[SerializeField, Tooltip("接地判定が反応するLayer")]
		LayerMask groundLayers;


		RaycastHit _slopeHit;
		private Animator _animator;
		private Vector3 _groundNormalVector;
		private PlayerParam _playerParam;
		private PlayerCameraBrain _playerCamera;
		private DOTween _doTween;


		void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_animator = GetComponentInChildren<Animator>();
			_playerParam = GetComponent<PlayerParam>();
			_playerCamera = FindObjectOfType<PlayerCameraBrain>();
		}

		private void FixedUpdate()
		{
			if (!_playerParam.GetIsAnimation)
			{
				OnMove(PlayerInputsAction.Instance.GetMoveInput);
				PlayerRotate(PlayerInputsAction.Instance.GetMoveInput);
			}
			else
			{
				_dir = Vector3.zero;
				_targetRotation = this.transform.rotation;
			}
		}

		/// <summary>プレイヤーの移動処理のメソッド </summary>
		private void OnMove(Vector2 inputValue)
		{
			//プレイヤーの移動方向を決める
			_dir = new Vector3(inputValue.x, 0, inputValue.y);
			_dir = Camera.main.transform.TransformDirection(_dir);
			_dir.y = 0;
			_dir = _dir.normalized;


			//加速
			_rb.AddForce(GetSlopeMoveDirection(_dir * moveSpeed,
				NormalRay()), ForceMode.Force);
			if (GroundCheck())
			{
				SpeedControl();
			}
			else
			{
				_rb.AddForce(new Vector3(0, -_gravityvalue));
			}


			_animator.SetFloat("Speed", _dir.magnitude);
		}

		///<summary>プレイヤーの向き</summary>
		void PlayerRotate(Vector2 inputValue)
		{
			// if (_playerCamera.LockingOn)
			// {
			//transform.LookAt(_playerCamera.CurrentLockingOnTarget);
			// }
			// else
			// {

			if (_dir.magnitude > 0)
			{
			}

			transform.forward += Vector3.Lerp(transform.forward, _dir, Time.deltaTime * rotateSpeed);
			//}
		}

		/// <summary>プレイヤーの接地判定を判定するメソッド </summary>
		/// <returns>接地判定</returns>	
		bool GroundCheck()
		{
			//オフセットを計算して接地判定の球の位置を設定する
			Vector3 spherePosition =
				new Vector3(transform.position.x, transform.position.y - groundOffSetY, transform.position.z);

			//接地判定を返す
			return Physics.CheckSphere(spherePosition, groundRadius, groundLayers, QueryTriggerInteraction.Ignore);
		}

		///<summary>法線ベクトルを返す</summary>
		Vector3 NormalRay()
		{
			RaycastHit hit;
			Ray ray = new Ray(transform.position, Vector3.down * _raylength);
			if (Physics.Raycast(ray, out hit, _raylength, groundLayers))
			{
				Debug.DrawRay(transform.position, Vector3.down * _raylength, Color.cyan);
				return hit.normal;
			}

			return Vector3.zero;
		}

		/// <summary>プレイヤーの速度制限のメソッド </summary>
		void SpeedControl()
		{
			if (_rb.velocity.magnitude > moveSpeed)
			{
				_rb.velocity = GetSlopeMoveDirection(_dir * moveSpeed, NormalRay());
			}
		}

		/// <summary>
		/// 傾斜に合わせたベクトルに変えるメソッド
		/// </summary>
		/// <returns>傾斜に合わせたベクトル</returns>
		/// <param name="dir"></param>
		Vector3 GetSlopeMoveDirection(Vector3 dir, Vector3 normalVector)
		{
			return Vector3.ProjectOnPlane(dir, normalVector);
		}

		public Transform GetPlayerCamTrasableTransform()
		{
			return transform;
		}

		public void InitializeThisComponent()
		{
		}

		public void FixedTickThisComponent()
		{
			//throw new NotImplementedException();
		}

		public void TickThisComponent()
		{
			//throw new NotImplementedException();
		}
		

		public void FinalizeThisComponent()
		{
			//throw new NotImplementedException();
		}
	}
}
