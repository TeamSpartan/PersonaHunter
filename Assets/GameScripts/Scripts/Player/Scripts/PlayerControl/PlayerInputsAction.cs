using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input
{
	#region InputEnum

	public enum PlayerInputTypes
	{
		Idle,
		Attack,
		Parry,
		Avoid
	}

	#endregion

	public enum InputType
	{
		Player,
		UI
	}

	public class PlayerInputsAction : MonoBehaviour, IInputValueReferencable, ILockOnEventFirable
	{
		private static PlayerInputsAction _instance;
		public static PlayerInputsAction Instance => _instance;

		[SerializeField, Header("インプット保存の上限")] private int maxInputCount = 5;
		private GameInputs _gameInputs;
		private Queue<PlayerInputTypes> _inputQueue = new();

		private Vector2 _inputVector = default;
		private Vector2 _inputCamera;
		private Vector2 _mouseMove;
		private Vector2 _cursorInput;

		private PlayerInputTypes _currentInput;
		private InputType _inputType;

		private bool _isLockOn;
		private bool _isZone;
		private bool _isCancel;
		private bool _isDecision;
		private bool _isConfirm;
		private bool _isDash;

		private bool _isExternalInputBlocked;
		private bool _playerControllerInputBlocked;

		private float _elapsedT;
		
		
		///<summary>InGame用とUI用の切り替え</summary>
		public void ChangeInputType(InputType inputType)
		{
			if (inputType == InputType.Player)
			{
				_inputType = InputType.UI;
				InGameDis();
				InUIInput();
			}
			else
			{
				_inputType = InputType.Player;
				InUIDis();
				InGameInput();
			}
		}

		///<summary>プレイヤーの移動入力</summary>
		public Vector2 GetMoveInput
		{
			get
			{
				if (_playerControllerInputBlocked || _isExternalInputBlocked)
					return Vector3.zero;
				return _inputVector;
			}
		}

		public Vector2 GetMoveValue()
		{
			return _inputVector;
		}


		///<summary>カメラの移動入力、ロックオン時敵の切り替え	</summary>
		public Vector2 GetCameraInput
		{
			get
			{
				if (_playerControllerInputBlocked || _isExternalInputBlocked)
					return Vector2.zero;
				return _inputCamera;
			}
		}

		public Vector2 GetCamMoveValue()
		{
			

			return _inputCamera;
		}

		public event System.Action ELockOnTriggered;
		public System.Action EvtCamLeftTarget { get; set; }
		public System.Action EvtCamRightTarget { get; set; }

		///<summary></summary>
		public Vector2 GetCursorInputInput => _cursorInput;

		///<summary>現在の入力のタイプ</summary>
		public PlayerInputTypes GetCurrentInputType => _currentInput;

		///<summary>入力の種類</summary>
		public InputType GetInputType => _inputType;

		///<summary>ロックオン</summary>
		public bool IsLockOn => _isLockOn;

		///<summary>ゾーン中か否か</summary>
		public bool IsZone => _isZone;

		///<summary>ダッシュする</summary>
		public bool IsDash => _isDash;

		void Awake()
		{
			if (_instance == null)
				_instance = this;

			_gameInputs = new();
		}

		private void OnEnable()
		{
			_gameInputs.Enable();
			InGameInput();
		}

		private void OnDisable()
		{
			_gameInputs.Disable();
			InGameDis();
		}

		private void Start()
		{
			_elapsedT = 0f;
		}

		void Update()
		{
			if (_inputType == InputType.Player)
				InputTypeUpdate();
			
			if (IsLockOn)
			{
				_elapsedT += Time.deltaTime;
			}
			if (_elapsedT > 1f)
			{
				ELockOnTriggered();
				_elapsedT = 0;
			}
		}

		///<summary>アニメーションが終わったら実行</summary>
		public void EndAction() => _currentInput = PlayerInputTypes.Idle;


		///<summary>同一の行動をキューから排除する</summary>
		public void DeleteInputQueue(PlayerInputTypes type)
		{
			while (_inputQueue.Count > 0)
			{
				if (_inputQueue.Peek() == type)
				{
					_inputQueue.Dequeue();
				}
				else
				{
					return;
				}
			}
		}

		///<summary>キューの最初の要素を確認する</summary>
		public PlayerInputTypes CheckInputQueue()
		{
			if (_inputQueue.Count <= 0)
			{
				return PlayerInputTypes.Idle;
			}

			return _inputQueue.Peek();
		}

		//------------------ここからは見なくて大丈夫----------------------------------------------------------------------------------------------

		///<summary>入力をキューに保存する</summary>
		void AddInputQueue(PlayerInputTypes playerInputType)
		{
			if (_inputQueue.Count >= maxInputCount)
			{
				_inputQueue.Dequeue();
			}

			_inputQueue.Enqueue(playerInputType);
		}

		//入力タイプの更新
		void InputTypeUpdate()
		{
			if (_currentInput == PlayerInputTypes.Idle && _inputQueue.Count >= 1)
			{
				_currentInput = _inputQueue.Dequeue();
				Debug.Log(_currentInput);
			}
		}

		void OnAttack(InputAction.CallbackContext context)
		{
			AddInputQueue(PlayerInputTypes.Attack);
			Debug.Log("WaitAttack");
		}

		void OnParry(InputAction.CallbackContext context)
		{
			AddInputQueue(PlayerInputTypes.Parry);
			Debug.Log("WaitParry");
		}

		void OnAvoid(InputAction.CallbackContext context)
		{
			AddInputQueue(PlayerInputTypes.Avoid);
			Debug.Log("WaitAvoid");
		}

		void OnZone(InputAction.CallbackContext context)
		{
			ELockOnTriggered?.Invoke();
			Debug.Log("Zone");
		}

		private void OnMove(InputAction.CallbackContext context)
		{
			_inputVector = context.ReadValue<Vector2>();
		}

		private void OnCamera(InputAction.CallbackContext context)
		{
			_inputCamera = context.ReadValue<Vector2>();
			if (_inputType == InputType.Player && _isLockOn)
			{
				if (0 < _inputCamera.x)
				{
					EvtCamRightTarget();
				}
				else if (_inputCamera.x < 0)
				{
					EvtCamLeftTarget();
				}
			}
		}

		void OnLockOn(InputAction.CallbackContext context)
		{
			_isLockOn = !_isLockOn;
		}

		void OnPause(InputAction.CallbackContext context)
		{
			if (_inputType == InputType.Player)
			{
				_inputType = InputType.UI;
			}
			else
			{
				_inputType = InputType.Player;
			}
		}

		void OnCancel(InputAction.CallbackContext context)
		{
			_isCancel = !_isCancel;
		}

		void OnDecision(InputAction.CallbackContext context)
		{
			_isDecision = !_isDecision;
		}

		void OnConfirm(InputAction.CallbackContext context)
		{
			_isConfirm = !_isConfirm;
		}

		void OnCursor(InputAction.CallbackContext context)
		{
			_cursorInput = context.ReadValue<Vector2>();
		}

		void OnDash(InputAction.CallbackContext context)
		{
			
		}

		void InGameInput()
		{
			//Move
			_gameInputs.Player.Move.started += OnMove;
			_gameInputs.Player.Move.performed += OnMove;
			_gameInputs.Player.Move.canceled += OnMove;

			//Attack
			_gameInputs.Player.Attack.started += OnAttack;

			//Parry
			_gameInputs.Player.Parry.started += OnParry;

			//Avoid
			_gameInputs.Player.Avoid.started += OnAvoid;

			//Zone
			_gameInputs.Player.Zone.started += OnZone;

			//Camera
			_gameInputs.Player.Camera.started += OnCamera;
			_gameInputs.Player.Camera.canceled += OnCamera;

			//LockOn
			_gameInputs.Player.LockOn.started += OnLockOn;

			//Pause
			_gameInputs.Player.Pause.started += OnPause;
			_gameInputs.Player.Pause.canceled += OnPause;
			
			//Dash
			_gameInputs.Player.Dash.started += OnDash;
		}

		void InGameDis()
		{
			//Move
			_gameInputs.Player.Move.started -= OnMove;
			_gameInputs.Player.Move.performed -= OnMove;
			_gameInputs.Player.Move.canceled -= OnMove;

			//Attack
			_gameInputs.Player.Attack.started -= OnAttack;

			//Parry
			_gameInputs.Player.Parry.started -= OnParry;

			//Avoid
			_gameInputs.Player.Avoid.started -= OnAvoid;

			//Zone
			_gameInputs.Player.Zone.started -= OnZone;

			//Camera
			_gameInputs.Player.Camera.started -= OnCamera;
			_gameInputs.Player.Camera.canceled -= OnCamera;

			//LockOn
			_gameInputs.Player.LockOn.started -= OnLockOn;

			//Pause
			_gameInputs.Player.Pause.started -= OnPause;
			_gameInputs.Player.Pause.canceled -= OnPause;
		}

		void InUIInput()
		{
			//Cancel
			_gameInputs.UI.Cancel.started += OnCancel;
			_gameInputs.UI.Cancel.canceled += OnCancel;

			//決定
			_gameInputs.UI.Decision.started += OnDecision;
			_gameInputs.UI.Decision.canceled += OnDecision;

			//変更確定
			_gameInputs.UI.Confirm.started += OnConfirm;
			_gameInputs.UI.Confirm.canceled += OnConfirm;

			//UI移動
			_gameInputs.UI.Cursor.started += OnCursor;
			_gameInputs.UI.Cursor.performed += OnCursor;
			_gameInputs.UI.Cursor.canceled += OnCursor;
		}

		void InUIDis()
		{
			//Cancel
			_gameInputs.UI.Cancel.started -= OnCancel;
			_gameInputs.UI.Cancel.canceled -= OnCancel;

			//決定
			_gameInputs.UI.Decision.started -= OnDecision;
			_gameInputs.UI.Decision.canceled -= OnDecision;

			//変更確定
			_gameInputs.UI.Confirm.started -= OnConfirm;
			_gameInputs.UI.Confirm.canceled -= OnConfirm;

			//UI移動
			_gameInputs.UI.Cursor.started -= OnCursor;
			_gameInputs.UI.Cursor.performed -= OnCursor;
			_gameInputs.UI.Cursor.canceled -= OnCursor;
		}
	}
}
