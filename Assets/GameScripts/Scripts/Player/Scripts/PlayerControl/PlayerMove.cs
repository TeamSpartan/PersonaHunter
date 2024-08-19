using DG.Tweening;
using Player.Input;
using Player.Param;
using PlayerCam.Scripts;
using UnityEngine;

namespace Player.Action
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMove : MonoBehaviour, IPlayerCameraTrasable
	{
		[SerializeField, Header("プレイヤーの回転速度")] float _rotateSpeed = 20f;

		[SerializeField, Header("カメラの回転速度")] private float _cameraRotateSpeed = 3f;

		[SerializeField, Header("歩く速度")] float _walkSpeed = 10f;

		[SerializeField, Header("走る速度")] private float _runSpeed = 20f;
		[SerializeField, Header("走るまでの時間")] private float _runTransitionTime = 1f;

		[Header("接地判定の長さ")] [SerializeField, Tooltip("接地判定Y方向のオフセット"), Range(0, -1)]
		float _groundOffSetY = -0.14f;

		[SerializeField, Header("接地判定の半径"), Range(0, 1)]
		float _groundRadius = 0.5f;

		[SerializeField, Header("重力の大きさ")] private float _gravityValue = 30f;

		[SerializeField, Header("ノーマル判定Rayの長さ")]
		private float _rayLength = 0.3f;

		[SerializeField, Tooltip("接地判定が反応するLayer")]
		LayerMask _groundLayers;

		[SerializeField] private GameObject _player;

		Rigidbody _rb;
		Vector3 _dir;
		RaycastHit _slopeHit;
		private Animator _animator;
		private Vector3 _groundNormalVector;
		private PlayerParam _playerParam;
		private PlayerCameraBrain _playerCamera;
		private DOTween _doTween;

		private float _speed;

		public Vector3 MovementDirection
		{
			get
			{
				return _dir;
			}

			set
			{
				_dir = value;
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

			//走っていると
			if (PlayerInputsAction.Instance.IsRunning)
			{
				_rb.velocity = GetSlopeMoveDirection(_dir * _speed, NormalRay());
			}
			else
			{
				_rb.velocity = GetSlopeMoveDirection(_dir * _speed, NormalRay());
			}

			if (PlayerInputsAction.Instance.GetMoveInput() == Vector2.zero)
			{
				_rb.velocity = Vector3.zero;
			}

			_animator.SetFloat("Speed", _dir.magnitude);
		}

		///<summary>プレイヤーの向き</summary>
		public void PlayerRotate(Vector3 dir)
		{
			if (_playerCamera.LockingOn)
			{
				_player.transform.forward +=
					Vector3.Lerp(_player.transform.forward, dir, Time.deltaTime * _rotateSpeed);
			}
			else
			{
				_player.transform.forward +=
					Vector3.Lerp(_player.transform.forward, dir, Time.deltaTime * _rotateSpeed);
				if (PlayerInputsAction.Instance.GetCurrentInputType != PlayerInputTypes.Avoid)
					transform.forward += Vector3.Lerp(transform.forward, dir, Time.deltaTime * _cameraRotateSpeed);
			}
		}

		/// <summary>プレイヤーの接地判定を判定するメソッド </summary>
		/// <returns>接地判定</returns>
		bool GroundCheck()
		{
			//オフセットを計算して接地判定の球の位置を設定する
			Vector3 spherePosition =
				new Vector3(transform.position.x, transform.position.y - _groundOffSetY, transform.position.z);

			//接地判定を返す
			return Physics.CheckSphere(spherePosition, _groundRadius, _groundLayers, QueryTriggerInteraction.Ignore);
		}

		///<summary>法線ベクトルを返す</summary>
		public Vector3 NormalRay()
		{
			RaycastHit hit;
			Ray ray = new Ray(transform.position, Vector3.down * _rayLength);
			if (Physics.Raycast(ray, out hit, _rayLength, _groundLayers))
			{
				Debug.DrawRay(transform.position, Vector3.down * _rayLength, Color.cyan);
				return hit.normal;
			}

			return Vector3.zero;
		}

		/// <summary>
		/// 傾斜に合わせたベクトルに変えるメソッド
		/// </summary>
		/// <returns>傾斜に合わせたベクトル</returns>
		/// <param name="dir"></param>
		public Vector3 GetSlopeMoveDirection(Vector3 dir, Vector3 normalVector)
		{
			return Vector3.ProjectOnPlane(dir, normalVector);
		}

		public Transform GetPlayerCamTrasableTransform()
		{
			return transform;
		}

		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_animator = GetComponentInChildren<Animator>();
			_playerParam = GetComponent<PlayerParam>();
			_playerCamera = FindObjectOfType<PlayerCameraBrain>();
			_playerParam.SetIsRun(false);
			_speed = _walkSpeed;
		}

		private void FixedUpdate()
		{
			if (GroundCheck())
			{
				if (!_playerParam.GetIsAnimation)
				{
					OnMove(PlayerInputsAction.Instance.GetMoveInput());
					PlayerRotate(_dir);
				}
				else if (!_playerParam.GetIsAvoid)
				{
					_rb.velocity = Vector3.zero;
				}
			}
			else
			{
				_rb.AddForce(Vector3.down * _gravityValue);
			}
		}

		private void OnDisable()
		{
			MovementDirection = Vector3.zero;
		}

		void SpeedChange()
		{
			if (_playerParam.GetIsRun)
			{
				_speed = _runSpeed;
			}
			else
			{
				_speed = _walkSpeed;
			}
		}
	}
}
