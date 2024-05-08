using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

	public class PlayerInputsAction : MonoBehaviour, IInputValueReferencable
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

		private bool _isLockOn = false;
		private bool _isZone = false;
		private bool _isCancel = false;
		private bool _isDecision = false;
		private bool _isConfirm = false;

		private bool _isExternalInputBlocked;
		private bool _playerControllerInputBlocked;

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

		public float GetHorizontalMoveValue()
		{
			return GetMoveInput.x;
		}

		public float GetVerticalMoveValue()
		{
			return GetMoveInput.y;
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

		//マウスのX軸方向の位置
		public float GetHorizontalCamMoveValue()
		{
			return GetCameraInput.x;
		}

		//マウスのY軸方向の位置
		public float GetVerticalCamMoveValue()
		{
			return GetCameraInput.y;
		}

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

		void Update()
		{
			if (_inputType == InputType.Player)
				InputTypeUpdate();
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
			_isZone = !_isZone;
			Debug.Log("Zone");
		}

		private void OnMove(InputAction.CallbackContext context)
		{
			_inputVector = context.ReadValue<Vector2>();
		}

		private void OnCamera(InputAction.CallbackContext context)
		{
			_inputCamera = context.ReadValue<Vector2>();
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
			_gameInputs.Player.Zone.canceled += OnZone;

			//Camera
			_gameInputs.Player.Camera.started += OnCamera;
			_gameInputs.Player.Camera.performed += OnCamera;
			_gameInputs.Player.Camera.canceled += OnCamera;

			//LockOn
			_gameInputs.Player.LockOn.started += OnLockOn;

			//Pause
			_gameInputs.Player.Pause.started += OnPause;
			_gameInputs.Player.Pause.canceled += OnPause;
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
			_gameInputs.Player.Zone.canceled -= OnZone;

			//Camera
			_gameInputs.Player.Camera.started -= OnCamera;
			_gameInputs.Player.Camera.performed -= OnCamera;
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
