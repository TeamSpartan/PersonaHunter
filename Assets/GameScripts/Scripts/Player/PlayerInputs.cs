using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputs : MonoBehaviour
{
	static PlayerInputs _instance;
	public static PlayerInputs Instance
	{
		get { return _instance; }
	}
	
	private GameInputs _gameInputs;


	Vector2 _inputVector = default;
	Vector2 _inputCamera;
	private bool _isAttack;
	private bool _isParry;
	private bool _isAvoid;
	private bool _isZone;
	private bool _isPause;
	bool _isExternalInputBlocked;
	public bool playerControllerInputBlocked;

	public Vector2 GetMoveInput
	{
		get
		{
			if (playerControllerInputBlocked || _isExternalInputBlocked)
				return Vector3.zero;
			return _inputVector;
		}
	}

	public Vector2 GetCameraInput
	{
		get
		{
			if (playerControllerInputBlocked || _isExternalInputBlocked)
				return Vector2.zero;
			return _inputCamera;
		}
	}

	///<summary>攻撃入力</summary>
	public bool GetIsAttack
	{
		get { return _isAttack; }
	}

	///<summary>パリィ入力</summary>
	public bool GetIsParry
	{
		get { return _isParry; }
	}

	///<summary>回避入力</summary>
	public bool GetIsAvoid
	{
		get { return _isAvoid; }
	}

	///<summary>ゾーン入力</summary>
	public bool GetIsZone
	{
		get { return _isZone; }
	}

	///<summary>ポーズの入力</summary>
	public bool GetIsPause
	{
		get { return _isPause; }
	}


	void Awake()
	{
		_gameInputs = new();
		if (_instance == null)
			_instance = this;
		else if (_instance != this)
			throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " +
			                         _instance.name + " and " + name + ".");
	}

	private void OnEnable()
	{
		InGameInputInitialization();
	}

	private void OnDisable()
	{
		_gameInputs.Disable();
		_gameInputs.Player.Move.started += OnMove;
		_gameInputs.Player.Move.performed += OnMove;
		_gameInputs.Player.Move.canceled += OnMove;
	}

	void InGameInputInitialization()
	{
		_gameInputs.Enable();
		
		//Move
		_gameInputs.Player.Move.started += OnMove;
		_gameInputs.Player.Move.performed += OnMove;
		_gameInputs.Player.Move.canceled += OnMove;
		
		//Attack
		_gameInputs.Player.Move.started += OnAttack;
		
		//Parry
		_gameInputs.Player.Parry.started += OnParry;
		
		//Avoid
		_gameInputs.Player.Avoid.started += OnAvoid;
		
		//Zone
		_gameInputs.Player.Zone.started += OnZone;
	}

	private void Start()
	{
	}

	void Update()
	{
		if (Gamepad.current == null)
		{
			return;
		}

		_isAttack = _gameInputs.Player.Attack.triggered;
		_isParry = _gameInputs.Player.Parry.triggered;
		_isAvoid = _gameInputs.Player.Avoid.triggered;
		_isZone = _gameInputs.Player.Zone.triggered;
	}

	void OnAttack(InputAction.CallbackContext context)
	{
		Debug.Log("Attack");
	}
	
	void OnParry(InputAction.CallbackContext context)
	{
		Debug.Log("Parry");
	}
	
	void OnAvoid(InputAction.CallbackContext context)
	{
		Debug.Log("Avoid");
	}
	
	void OnZone(InputAction.CallbackContext context)
	{
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
}
