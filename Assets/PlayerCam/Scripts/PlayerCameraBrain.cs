using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using SgLibUnite.CodingBooster;

namespace PlayerCam.Scripts
{
    [RequireComponent(typeof(CinemachineBrain))]
    /// <summary>
    /// 作成：菅沼
    /// プレイヤーカメラの機能を提供する、
    /// 主に
    /// プレイヤ追跡とロックオンターゲットのロックオンを提供。
    /// </summary>
    public class PlayerCameraBrain
        : MonoBehaviour
    {
        #region Parameter Exposing

        [SerializeField, Header("The Radius When Locking On The Targets")]
        private float LockOnRadius;

        [SerializeField, Header("The Max Distance Capurable Lock-On Target")]
        private float MaxDistanceToCapture = 100f;

        [SerializeField, Header("The Camera Default")]
        private CinemachineVirtualCamera PlayerFollowingCam;

        [SerializeField, Header("The Camera LockingOn Target")]
        private CinemachineVirtualCamera LockOnCamera;

        /// <summary>
        /// 現在 ロックオン している ターゲットのトランスフォーム情報
        /// </summary>
        public Transform CurrentLockingOnTarget => _currentLockOnTarget;

        #endregion

        #region Parameter Inside

        /// <summary>
        /// 現在のプレイヤのトランスフォーム
        /// </summary>
        private Transform _playerCurrent;

        /// <summary>
        /// ロックオン対象の配列
        /// </summary>
        private List<Transform> _lockOnTargetsTf;

        /// <summary>
        /// ワールド座標とビューポート座標の連想配列
        /// <para>
        /// ビューポート座標 (0,0)[画面左下] ~ (1,1)[画面右上]
        /// </para>
        /// </summary>
        private Dictionary<Transform, Vector3> _vportTransformDic = new Dictionary<Transform, Vector3>();

        /// <summary>
        /// ロックオン中かどうかのフラグ
        /// </summary>
        private bool _lockingOn;

        public bool LockingOn => _lockingOn;

        /// <summary>
        /// コーディング ブースタ クラス
        /// </summary>
        CBooster boost = new CBooster();

        /// <summary>
        /// ロックオン時の半径
        /// </summary>
        private float _lockOnRadius;

        /// <summary>
        /// ロックオン対象
        /// </summary>
        private Transform _currentLockOnTarget;

        // 入力値 ー キャラ移動と視点移動入力
        private float _moveX;
        private float _moveY;
        private float _mouseX;
        private float _mouseY;
        private float _theta;

        // カメラ各種
        private CinemachineBrain _cinemachineBrain;
        private CinemachineVirtualCamera _playerFollowCam;
        private CinemachineVirtualCamera _lockOnCam;

        #endregion

        private void Start()
        {
            // 検索にひっかかった最初のオブジェクトをプレイヤとする
            this._playerCurrent = boost.GetDerivedComponents<IPlayerCameraTrasable>()
                .First().GetPlayerCamTrasableTransform();

            // ロックオンイベント発火元へのデリゲート登録をする
            boost.GetDerivedComponents<ILockOnEventFirable>()
                .ForEach(_ => _.ELockOnTriggered += this.LockOnTriggerred);

            // ロックオン対象選択イベント発火もとへデリゲート登録
            var lockOnEventHandler = boost.GetDerivedComponents<ILockOnEventFirable>()[0];
            lockOnEventHandler.EvtCamLeftTarget += LockOnToLeftTarget;
            lockOnEventHandler.EvtCamRightTarget += LockOnToRightTarget;

            // 内部パラメータ初期化
            this._lockOnRadius = LockOnRadius; // 半径
            this._playerFollowCam = PlayerFollowingCam;
            this._lockOnCam = LockOnCamera;
            this._cinemachineBrain = GetComponent<CinemachineBrain>();

            // 更新方法 ー 頻度（毎秒）を設定
            this._cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;

            // メインカメラであってほしいので
            this.gameObject.tag = "MainCamera";
        }

        public void OnApplicationQuit()
        {
            // ロックオンイベント発火元へのデリゲート登録解除をする
            boost.GetDerivedComponents<ILockOnEventFirable>()
                .ForEach(_ => _.ELockOnTriggered -= this.LockOnTriggerred);
        }

        private void FixedUpdate()
        {
            GetInputValue();
            CamBehaviour_Tick();
        }

        void CamBehaviour_Tick()
        {
            if (_lockingOn)
            {
                CamLockingOn_Tick();
            }
            else
            {
                CamDefault_Tick();
            }
        }

        void CamLockingOn_Tick()
        {
            // ターゲットを常に正面左側からソートした状態の状態を格納
            _lockOnTargetsTf = GetSortedLockOnTargets();

            // 通常カメラとは視点は大きな変化はなし、ロックオンターゲット中心に
            // 円形を描くような左右移動をする。
            // 前後（敵に対して）すると半径の値が変動

            var inputMove = new Vector2(_moveX, _moveY);
            var inputLook = new Vector2(_mouseX, _mouseY);

            // 前後移動

            #region Movements

            if (_moveY < 0 && _moveY != 0)
            {
                if (_lockOnRadius <= LockOnRadius)
                {
                    _lockOnRadius += Time.deltaTime * 10f;
                }
            }
            else if (_moveY != 0)
            {
                if (_lockOnRadius >= 1)
                {
                    _lockOnRadius -= Time.deltaTime * 10f;
                }
            }

            // 左右移動
            if (_moveX < 0 && _moveX != 0)
            {
                _theta -= Time.deltaTime;
            }
            else if (_moveX != 0)
            {
                _theta += Time.deltaTime;
            }

            // Set Up Player
            // プレイヤ のトランスフォーム情報を初期化ー設定
            var playerRight = Mathf.Cos(_theta) * _lockOnRadius;
            var playerForward = Mathf.Sin(_theta) * _lockOnRadius;
            var pDir = new Vector2(playerRight, playerForward);

            var target = _currentLockOnTarget;
            var centerPosition = target.position;

            centerPosition.x += pDir.x; // right
            centerPosition.z += pDir.y; // forward

            var dir = new Vector3(target.position.x - _playerCurrent.position.x
                , 0f
                , target.position.z - _playerCurrent.position.z).normalized;
            _playerCurrent.transform.position = centerPosition;
            _playerCurrent.transform.forward = dir;

            #endregion

            // 前フレームの連想配列は破棄。配置が換わっている可能性があるため
            _vportTransformDic.Clear();

            // ビューポート座標:ワールド座標の連想配列を作成
            foreach (var lockOnTarget in _lockOnTargetsTf)
            {
                var vportpos = Camera.main.WorldToViewportPoint(lockOnTarget.position);
                if (!_vportTransformDic.ContainsKey(lockOnTarget))
                {
                    // Debug.Log($"Dic Added");
                    _vportTransformDic.Add(lockOnTarget, vportpos);
                }
            }

            // ビューポート座標ｘ成分で昇順ソート
            _vportTransformDic = _vportTransformDic.OrderBy(i => i.Value.x).ToDictionary(k => k.Key, v => v.Value);

            // Set Up Camera
            // カメラ をセットアップ
            _lockOnCam.LookAt = target;
            _lockOnCam.Follow = _playerCurrent;
        }

        void CamDefault_Tick()
        {
            // 基本的にオービタルカメラ。 左右のみ、すこし上からプレイヤを見下ろしている視点
            _playerFollowCam.Follow = _playerCurrent;
            _playerFollowCam.LookAt = _playerCurrent;
        }

        void GetInputValue()
        {
            var iInput = boost.GetDerivedComponents<IInputValueReferencable>();

            _moveX = iInput[0].GetMoveValue().x;
            _moveY = iInput[0].GetMoveValue().y;

            _mouseX = iInput[0].GetCamMoveValue().x;
            _mouseY = iInput[0].GetCamMoveValue().y;
        }

        /// <summary>
        /// 左のロックオン対象へロックオン
        /// </summary>
        void LockOnToLeftTarget()
        {
            // 現在ロックオンしているターゲットのインデックスを取得
            var vportpos = Camera.main.WorldToViewportPoint(_currentLockOnTarget.position);
            var index = _vportTransformDic.Values.ToList().FindIndex(x => x == vportpos);
            if (index != -1 && index - 1 > -1)
            {
                index--;
            }
            
            _currentLockOnTarget = _vportTransformDic.Keys.ToList()[index];

            _theta = GetRadianValueToLookAtTarget();
        }

        /// <summary>
        /// 右のロックオン対象へロックオン
        /// </summary>
        void LockOnToRightTarget()
        {
            // 現在ロックオンしているターゲットのインデックスを取得
            var vportpos = Camera.main.WorldToViewportPoint(_currentLockOnTarget.position);
            var index = _vportTransformDic.Values.ToList().FindIndex(x => x == vportpos);
            if (index != -1 && index + 1 < _vportTransformDic.Count)
            {
                index++;
            }
            
            _currentLockOnTarget = _vportTransformDic.Keys.ToList()[index];

            _theta = GetRadianValueToLookAtTarget();
        }

        /// <summary>
        /// ロックオン入力が入った時に発火するイベントへの登録関数
        /// </summary>
        void LockOnTriggerred()
        {
            _lockingOn = !_lockingOn;
            _playerFollowCam.Priority = 0;
            _lockOnCam.Priority = 0;

            if (_lockingOn)
            {
                LockOnSetup();
            }
            else
            {
                _lockOnTargetsTf.Clear();

                _playerFollowCam.Priority = 1;
            }
        }

        private void LockOnSetup()
        {
            // Get All LockOn Target
            // マップ上のロックオン可能なターゲットを取得
            // Filter Captureable Target
            // 捕捉可能な距離圏内にいるターゲットを取得
            _lockOnTargetsTf = GetLockableTargets();

            // ロックオンターゲットのソート：左→右 ＿ 0 → Count - 1
            _lockOnTargetsTf = GetSortedLockOnTargets();

            // If LockOn Targte Was Not Found Cancel Locking On
            if (_lockOnTargetsTf.Count() < 1)
            {
                _lockingOn = false;
                return;
            }

            // とりあえず正面のターゲットへロックオン
            var len = _lockOnTargetsTf.Count;
            _currentLockOnTarget = _lockOnTargetsTf[(len - 1) / 2];
            
            _theta = GetRadianValueToLookAtTarget();

            // ロックオンカメラに切り替え
            _lockOnCam.Priority = 1;
        }

        /// <summary>
        /// 現在のターゲット対象をプレイヤの位置を変えることなく向くために必要なラジアンの値を返す。
        /// <para>
        /// ラジアンの値で対象を中心とした円の円周の位置が決まる。
        /// </para>
        /// </summary>
        private float GetRadianValueToLookAtTarget()
        {
            // ワールド座標 ｘ軸とｚ軸が作る平面のグラフにおいて、
            // 第4象限 １．５以上 ３ 未満
            // 第3証言 ０ 以上 １．５ 未満
            // 第2証言 ０ 未満 -１．５ 以上
            // 第1証言 -１．５ 未満 -３以上
            // 単位は ラジアン
            LockOnRadius = _lockOnRadius = Vector3.Distance(_currentLockOnTarget.position,_playerCurrent.position);

            // The Subtraction for calculate Vector Player position-supposed 
            // ロックオン発動時のプレイヤの位置を設定するためのベクトル
            var dx = _currentLockOnTarget.position.x
                     - _playerCurrent.position.x;
            var dy = _currentLockOnTarget.position.z
                     - _playerCurrent.position.z;

            // ま反対の方向へプレイヤの位置が初期化されてしまうのでオイラー角でいう180°を足せばよい。
            // が、ま反対のためdy,dxともに-1を掛けた値を使えπは足さずに済む
            // Atan2は弧度法の値で返してくるのでPI（弧度法）を返す
            return Mathf.Atan2(-dy, -dx);
        }

        /// <summary>
        /// 敵トランスフォームのリストの添え字を指定してプレイヤとの距離を取得
        /// </summary>
        float GetDistanceByIndex(int index) =>
            Vector3.Distance(_lockOnTargetsTf[index].position, _playerCurrent.position);

        private List<Transform> GetSortedLockOnTargets()
        {
            return _lockOnTargetsTf.OrderBy(v => (v.position - _playerCurrent.position).x).ToList();
        }

        private List<Transform> GetLockableTargets()
        {
            return boost.GetDerivedComponents<IPlayerCamLockable>()
                .Select(_ => _.GetLockableObjectTransform())
                .Where(_ =>
                    Vector3.Distance(_playerCurrent.position, _.position) <= MaxDistanceToCapture
                    || Camera.main.WorldToScreenPoint(_.position).x <= Camera.main.pixelWidth - 1
                    && Camera.main.WorldToScreenPoint(_.position).y <= Camera.main.pixelHeight - 1
                    && Camera.main.WorldToScreenPoint(_.position).z > 0)
                .ToList();
        }
    }
}
