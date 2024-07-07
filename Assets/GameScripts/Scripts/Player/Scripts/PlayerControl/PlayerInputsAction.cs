using System;
using System.Collections.Generic;
using Player.Zone;
using PlayerCam.Scripts;
using SgLibUnite.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

#region 設計

// シングルトンはご法度で運用する

#endregion

// コードクリーン実施 【６／２２ ： 菅沼】
// 低レベル入力イベント発火クラスへのデリゲート登録メソッドの
// シグネイチャを変更
// プロパティの追加。
// 公開フィールドの変数名をらファクタ。単語のニュアンスを変更

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

    /// <summary>
    /// 入力バッファクラス
    /// </summary>
    public class PlayerInputsAction : MonoBehaviour, IInputValueReferencable,
        ILockOnEventFirable
    {
        private static PlayerInputsAction _instance;
        public static PlayerInputsAction Instance => _instance;

        [SerializeField, Header("インプット保存の上限")] private int maxEnqueueCount = 5;
        private Queue<PlayerInputTypes> _inputQueue = new();
        private GameInputs _gameInputs; // 低レベルAPIクラス   InputSystemのイベント提供クラス

        private MainGameLoop mainGameLoop;
        private ZoneTimeController _zoneTimeController;

        private Vector2 _inputVector;
        private Vector2 _inputCamera;
        private Vector2 _mouseMove;
        private Vector2 _cursorInput;

        private PlayerInputTypes _currentInput;
        private InputType _inputType;

        private bool _isLockingOn;
        private bool _isInZone;
        private bool _isCancel;
        private bool _isDecision;
        private bool _isConfirm;
        private bool _isRunning;

        private bool _isExternalInputBlocked;
        private bool _playerControllerInputBlocked;

        private PlayerCameraBrain _cameraBrain;

        /// <summary> コントローラ以外の入力がブロックされてるか </summary>
        public bool ExternalInputBlocked
        {
            get { return _isExternalInputBlocked; }
            set { _isExternalInputBlocked = value; }
        }

        /// <summary> コントローラの入力がブロックされてるか </summary>
        public bool ControllerInputBlocked
        {
            get { return _playerControllerInputBlocked; }
            set { _playerControllerInputBlocked = value; }
        }

        /// <summary> ポーズの入力がブロックされてるか </summary>
        public bool PauseInputBlocked { get; set; }

        public System.Action ELockOnTriggered { get; set; }
        public System.Action EvtCamLeftTarget { get; set; }
        public System.Action EvtCamRightTarget { get; set; }

        ///<summary>カーソルの位置</summary>
        public Vector2 GetCursorPos => _cursorInput;

        ///<summary>現在の入力のタイプ</summary>
        public PlayerInputTypes GetCurrentInputType => _currentInput;

        ///<summary>入力の種類</summary>
        public InputType InputType
        {
            get { return _inputType; }

            set { _inputType = value; }
        }

        /* フラグだったらIsから始める */

        ///<summary>ロックオン</summary>
        public bool IsLockingOn => _isLockingOn;

        ///<summary>ゾーン中か否か</summary>
        public bool IsInZone => _isInZone;

        ///<summary>ダッシュする</summary>
        public bool IsRunning => _isRunning;

        private void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            _gameInputs = new();

            _gameInputs.Enable();
            InGameInput_AddingDelegate();
        }

        private void OnDisable()
        {
            InGameInput_RemoveDelegate();
            _gameInputs.Disable();
            _gameInputs.Dispose(); // 有効化されたらインスタンス化されるのでここで破棄しておく
            _gameInputs = null;
        }

        private void Start()
        {
            mainGameLoop = GameObject.FindAnyObjectByType<MainGameLoop>();
            mainGameLoop.EPause += () => { _inputQueue.Clear(); };
            mainGameLoop.EResume += () => { _inputQueue.Clear(); };

            _zoneTimeController = MainGameLoop.FindAnyObjectByType<ZoneTimeController>();

            _cameraBrain = GameObject.FindAnyObjectByType<PlayerCameraBrain>(FindObjectsInactive.Include);
            if (_cameraBrain is not null && !_cameraBrain.gameObject.activeSelf)
            {
                _cameraBrain.gameObject.SetActive(true);
            }
        }

        private void OnGUI() // デバッグ表示
        {
            GUI.Box(new Rect(0f, 0f, 700f, 600f)
                ,
                @$"
                Input Type : {_inputType.ToString()}
                Input Blocked : {_isExternalInputBlocked} , {_playerControllerInputBlocked}
				Move Input : {GetMoveInput().ToString()}
				");
        }

        void Update()
        {
            if (_playerControllerInputBlocked
                || _isExternalInputBlocked)
            {
                return;
            }

            // 入力モードがインゲームのものなら
            if (_inputType == InputType.Player)
            {
                InputTypeUpdate();
            }
        }

        ///<summary>InGame用とUI用の切り替え</summary>
        public void ChangeInputType(InputType inputType)
        {
            if (inputType == InputType.Player)
            {
                _inputType = InputType.UI;
                InGameInput_RemoveDelegate();
                UIInput_AddDelegate();
            }
            else
            {
                _inputType = InputType.Player;
                UIInput_RemoveDelegate();
                InGameInput_AddingDelegate();
            }
        }

        ///<summary>プレイヤーの移動入力</summary>
        public Vector2 GetMoveInput()
        {
            if (_playerControllerInputBlocked
                || _isExternalInputBlocked)
            {
                return Vector3.zero;
            }

            return _inputVector;
        }

        public Vector2 GetMoveValue()
        {
            if (_playerControllerInputBlocked
                || _isExternalInputBlocked)
            {
                return Vector3.zero;
            }

            return _inputVector;
        }


        ///<summary>カメラの移動入力、ロックオン時敵の切り替え	</summary>
        public Vector2 GetCameraInput
        {
            get
            {
                if (_playerControllerInputBlocked
                    || _isExternalInputBlocked)
                {
                    return Vector2.zero;
                }

                return _inputCamera;
            }
        }

        public Vector2 GetCamMoveValue()
        {
            return _inputCamera;
        }

        ///<summary>アニメーションが終わったら実行</summary>
        public void EndAction()
        {
            _currentInput = PlayerInputTypes.Idle;
        }

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
                // return する必要はない希ガス
            }
        }

        public void ClearInputBuffer()
        {
            _inputQueue.Clear();
            _inputCamera = _inputVector = Vector2.zero;
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

        ///<summary>攻撃後歩かせる</summary>
        public void RunCancel()
        {
            _isRunning = false;
        }

        //------------------ここからは見なくて大丈夫publicじゃないから----------------------------------------------------------------------------------------------

        ///<summary>入力をキューに保存する</summary>
        void AddInputQueue(PlayerInputTypes playerInputType)
        {
            if (_inputQueue.Count >= maxEnqueueCount)
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
                Debug.Log(_inputQueue.Count);
            }
        }

        void OnAttack(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                if (!(_playerControllerInputBlocked
                      || _isExternalInputBlocked))
                {
                    AddInputQueue(PlayerInputTypes.Attack);
                }

                Debug.Log("WaitAttack");
            }
        }

        void OnParry(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                if (!(_playerControllerInputBlocked
                      || _isExternalInputBlocked))
                {
                    AddInputQueue(PlayerInputTypes.Parry);
                }

                Debug.Log("WaitParry");
            }
        }

        void OnAvoid(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                if (!(_playerControllerInputBlocked
                      || _isExternalInputBlocked))
                {
                    AddInputQueue(PlayerInputTypes.Avoid);
                }

                Debug.Log("WaitAvoid");
            }
        }

        void OnZone(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                if ((_playerControllerInputBlocked
                     || _isExternalInputBlocked))
                {
                    return;
                }

                Debug.Log("ゾーン 入力あり");
                if (_zoneTimeController is null)
                {
                    _zoneTimeController = GameObject.FindAnyObjectByType<ZoneTimeController>();
                }

                if (!_zoneTimeController.GetIsSlowTime)
                {
                    _zoneTimeController.StartDull();
                }
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (!(_playerControllerInputBlocked
                  || _isExternalInputBlocked))
            {
                _inputVector = context.ReadValue<Vector2>();
            }
            else
            {
                _inputVector = Vector2.zero;
            }
        }

        private void OnCamera(InputAction.CallbackContext context)
        {
            if (!(_playerControllerInputBlocked
                  || _isExternalInputBlocked))
            {
                _inputCamera = context.ReadValue<Vector2>();

                if (_inputType == InputType.Player)
                {
                    // 左右入力が入ればロックオン対象切り替えイベントの発火
                    if (0 < _inputCamera.x)
                    {
                        EvtCamRightTarget.Invoke();
                    }
                    else if (_inputCamera.x < 0)
                    {
                        EvtCamLeftTarget.Invoke();
                    }
                }
            }
            else
            {
                _inputVector = Vector2.zero;
            }
        }

        void OnLockOn(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                if (!(_playerControllerInputBlocked
                      || _isExternalInputBlocked))
                {
                    ELockOnTriggered?.Invoke();
                }
            }
        }

        void OnPause(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                mainGameLoop.PauseResumeInputFired();

                if (!PauseInputBlocked && mainGameLoop.IsPausing)
                {
                    _inputType = InputType.UI;
                }
                else
                {
                    _inputType = InputType.Player;
                }
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

        void OnRun(InputAction.CallbackContext context)
        {
            _isRunning = !_isRunning;
        }

        void InGameInput_AddingDelegate()
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
            _gameInputs.Player.Camera.performed += OnCamera;
            _gameInputs.Player.Camera.canceled += OnCamera;

            //LockOn
            _gameInputs.Player.LockOn.started += OnLockOn;

            //Pause
            _gameInputs.Player.Pause.started += OnPause;

            //Dash
            _gameInputs.Player.Dash.started += OnRun;
        }

        void InGameInput_RemoveDelegate()
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
            _gameInputs.Player.Camera.performed -= OnCamera;
            _gameInputs.Player.Camera.canceled -= OnCamera;

            //LockOn
            _gameInputs.Player.LockOn.started -= OnLockOn;
            //Pause
            _gameInputs.Player.Pause.started -= OnPause;
        }

        void UIInput_AddDelegate()
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

        void UIInput_RemoveDelegate()
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
