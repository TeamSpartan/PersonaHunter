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
		Avoid,
		Zone
	}
	#endregion

	public class PlayerInputsAction : MonoBehaviour
	{
		static PlayerInputsAction _instance;

		public static PlayerInputsAction Instance => _instance;

		[SerializeField, Header("インプット保存の上限")] private int maxInputCount = 5;
		private GameInputs _gameInputs;
		private Queue<PlayerInputTypes> _inputQueue = new();

		Vector2 _inputVector = default;
		Vector2 _inputCamera;
		private PlayerInputTypes _currentInput;

		bool _isExternalInputBlocked;
		bool _playerControllerInputBlocked;


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

		///<summary>カメラの移動入力</summary>
		public Vector2 GetCameraInput
		{
			get
			{
				if (_playerControllerInputBlocked || _isExternalInputBlocked)
					return Vector2.zero;
				return _inputCamera;
			}
		}

		///<summary>現在の入力のタイプ</summary>
		public PlayerInputTypes GetCurrentInputType => _currentInput;

		void Awake()
		{
			if (_instance == null)
				_instance = this;

			_gameInputs = new();
		}

		private void OnEnable()
		{
			_gameInputs.Enable();
			InGameInputInitialization();
		}

		private void OnDisable()
		{
			_gameInputs.Disable();

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
		}
		

		void Update()
		{
			InputTypeUpdate();
		}


		void AddInputQueue(PlayerInputTypes playerInputType)
		{
			if (_inputQueue.Count >= maxInputCount)
			{
				_inputQueue.Dequeue();
			}

			_inputQueue.Enqueue(playerInputType);
		}

		///<summary>アニメーションが終わったら実行</summary>
		public void EndAction() => _currentInput = PlayerInputTypes.Idle;

		void InputTypeUpdate()
		{
			if (_currentInput == PlayerInputTypes.Idle && _inputQueue.Count >= 1)
			{
				_currentInput = _inputQueue.Dequeue();
				Debug.Log(_currentInput);
			}
		}

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

		public PlayerInputTypes CheckInputQueue()
		{
			return _inputQueue.Peek();
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
			AddInputQueue(PlayerInputTypes.Zone);
			Debug.Log("WaitZone");
		}

		private void OnMove(InputAction.CallbackContext context)
		{
			_inputVector = context.ReadValue<Vector2>();
		}

		private void OnCamera(InputAction.CallbackContext context)
		{
			_inputCamera = context.ReadValue<Vector2>();
		}

		void InGameInputInitialization()
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
		}
	}
}
