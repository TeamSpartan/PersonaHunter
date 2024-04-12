using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Action
{
	//Rigidbodyを強制的にアタッチする
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMove : MonoBehaviour
	{
		/// <summary>プレイヤーのRigidbody </summary>
		Rigidbody _rb;

		/// <summary>接地判定 </summary>
		bool _IsGround;

		/// <summary>プレイヤーの移動方向 </summary>
		Vector3 _dir;

		/// <summary>プレイヤーの進行方向を保存する </summary>
		Quaternion _targetRotation;

		[SerializeField, Tooltip("プレイヤーの回転速度")]
		float rotateSpeed = 10f;

		[SerializeField, Tooltip("プレイヤーの移動速度")]
		float moveSpeed = 10f;

		[SerializeField, Tooltip("プレイヤーの高さ")] float playerHeight = 1f;

		[Header("接地判定")] [SerializeField, Tooltip("接地判定Y方向のオフセット"), Range(0, -1)]
		float groundOffSetY = -0.14f;

		[SerializeField, Tooltip("接地判定の半径"), Range(0, 1)]
		float groundRadius = 0.5f;

		[SerializeField, Tooltip("接地判定が反応するLayer")]
		LayerMask groundLayers;

		[Header("傾斜")] [SerializeField, Tooltip("最大傾斜角度")]
		float maxSlopeAngle;

		RaycastHit _slopeHit;
		private GameInputs _gameInputs;
		private Animator _animator;
		private Vector3 _groundNormalVector;

		private void OnEnable()
		{
			_gameInputs = new();
			_gameInputs.Enable();
		}

		private void OnDisable()
		{
			_gameInputs.Disable();
		}

		void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_animator = GetComponentInChildren<Animator>();
		}

		void Update()
		{
			//速度制限をする
			//SpeedControl();

			Debug.DrawRay(transform.position, Vector3.down, Color.red, playerHeight * 0.5f + 5f);
		}

		private void FixedUpdate()
		{
			OnMove(_gameInputs.Player.Move);
			//接地判定
			_IsGround = GroundCheck();
		}

		/// <summary>プレイヤーの移動処理のメソッド </summary>
		private void OnMove(InputAction context)
		{
			//プレイヤーの移動方向を決める
			Vector2 inputValue = context.ReadValue<Vector2>();
			_dir = new Vector3(inputValue.x, 0, inputValue.y);
			_dir = Camera.main.transform.TransformDirection(_dir);
			_dir.y = 0;
			_dir = _dir.normalized;

			//滑らかに進行方向に回転させる
			if (_dir.magnitude > 0)
			{
				_targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
			}

			transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotateSpeed);


			//傾斜を登っているのかを判定する
			if (OnSlope())
			{
				_rb.AddForce(_dir * moveSpeed * 20f, ForceMode.Force);
				//傾斜で浮かないための処理
				if (_rb.velocity.y > 0)
					_rb.AddForce(Vector3.down * 80f, ForceMode.Force);
			}
			else
			{
				_rb.AddForce(_dir * moveSpeed * 10f, ForceMode.Force);
			}

			SpeedControl();
			_animator.SetFloat("Speed", _dir.magnitude);
			//傾斜なら重力を無くす
			_rb.useGravity = !OnSlope();
		}

		/// <summary>プレイヤーの接地判定を判定するメソッド </summary>
		/// <returns>接地判定</returns>	
		bool GroundCheck()
		{
			//オフセットを計算して接地判定の球の位置を設定する
			Vector3 spherePosition =
				new Vector3(transform.position.x, transform.position.y - groundOffSetY, transform.position.z);

			Debug.DrawRay(spherePosition, Vector3.down, Color.green, playerHeight * 0.5f + 5f);
			//接地判定を返す
			return Physics.CheckSphere(spherePosition, groundRadius, groundLayers, QueryTriggerInteraction.Ignore);
		}

		/// <summary>プレイヤーの速度制限のメソッド </summary>
		void SpeedControl()
		{
			//傾斜での速度制限
			if (OnSlope())
			{
				if (_rb.velocity.magnitude > moveSpeed)
					_rb.velocity = _rb.velocity.normalized * moveSpeed;
			}
			else
			{
				Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
				//速度制限をする
				if (flatVel.magnitude > moveSpeed)
				{
					Vector3 limitVel = flatVel.normalized * moveSpeed;
					_rb.velocity = new Vector3(limitVel.x, _rb.velocity.y, limitVel.z);
				}
			}
		}

		/// <summary>
		/// 傾斜の判定をするメソッド
		/// </summary>
		/// <returns>地面が登れる傾斜であるのかを判定</returns>
		bool OnSlope()
		{
			//接地していたら地面の角度を求める
			if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f,
				    groundLayers))
			{
				float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
				return angle < maxSlopeAngle && angle != 0;
			}

			return false;
		}

		/// <summary>
		/// 傾斜に合わせたベクトルに変えるメソッド
		/// </summary>
		/// <returns>傾斜に合わせたベクトル</returns>
		Vector3 GetSlopeMoveDirection()
		{
			return Vector3.ProjectOnPlane(_dir, _slopeHit.normal).normalized;
		}
	}
}
