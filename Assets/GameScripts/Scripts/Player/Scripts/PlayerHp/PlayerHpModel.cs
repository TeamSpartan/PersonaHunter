using Player.Action;
using Player.Param;
using UniRx;
using UnityEngine;

namespace name
{
    public class PlayerHpModel : MonoBehaviour
    {
        private float _playerCurrentHp = new();
        public float PlayerCurrentHp => _playerCurrentHp;

        [SerializeField, Header("無敵フレーム")] private int _invincibilityFrame;
        [SerializeField, Header("リジェネまでの時間")] private float _regeneratWaitTime = 5f;

        [SerializeField, Header("リジェネ回復量"), Range(1f, 100f)]
        private float _regenerationSpeed;


        public event System.Action<float, float> OnReceiveDamage,
            OnRegeneration;

        private PlayerParam _playerParam;
        private PlayerAvoid _playerAvoid;

        private float _initialHp;
        private int _frameSinceLastHit;
        private float _regenerationTimer;
        private bool _isRegeneration;

        private NuweBrain _nue;
        private KomashiraBrain _mob;

        #region properties

        public float InitialHp => _initialHp;

        #endregion

        private void OnEnable()
        {
            #region InitiateAction

            #endregion
        }

        public void Start()
        {
            _playerParam = GetComponentInParent<PlayerParam>();
            _playerAvoid = GetComponentInParent<PlayerAvoid>();
            _initialHp = _playerParam.GetInitialHp;
            _nue = GameObject.FindFirstObjectByType<NuweBrain>();
            _mob = GameObject.FindFirstObjectByType<KomashiraBrain>();
            ResetDamage();
        }

        private void Update()
        {
            //無敵時間中
            if (_playerParam.GetIsDamage)
            {
                if (_frameSinceLastHit > _invincibilityFrame)
                {
                    _frameSinceLastHit = 0;
                    _playerParam.SetIsDamage(false);
                }
                else
                {
                    ++_frameSinceLastHit;
                }
            }

            Regeneration();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_nue != null)
            {
                switch (_nue.GetAttackType(other.transform))
                {
                    case NuweBrain.NueAttackType.Claw:
                        AddDamage(_nue.GetBaseDamage);
                        break;
                    case NuweBrain.NueAttackType.Rush:
                        AddDamage(_nue.GetBaseDamage);
                        break;
                    case NuweBrain.NueAttackType.Tail:
                        AddDamage(_nue.GetBaseDamage);
                        break;
                }
            }
            else
            {
                Debug.Log("null");
            }

            if (_mob != null)
            {
                AddDamage(_mob.GetBaseDamage);
            }
        }

        ///<summary>HPのリセット</summary>
        public void ResetDamage()
        {
            _regenerationTimer = 0;
            _playerCurrentHp = _initialHp;
        }

        public void AddDamage(float dmg)
        {
            if (_playerCurrentHp <= 0f || _playerParam.GetIsParry)
            {
                return;
            }

            //ジャスト回避
            if (_playerParam.GetIsJustAvoid)
            {
                _playerAvoid.OnJustAvoidSuccess.Invoke(_playerParam.GetIncreaseValueOfJustAvoid);
                return;
            }

            //普通の回避
            if (_playerParam.GetIsAvoid)
            {
                _playerAvoid.OnAvoidSuccess.Invoke();
                return;
            }

            //Received damage while invincible
            if (_playerParam.GetIsDamage)
            {
                return;
            }

            //減算処理
            if (dmg > 0f)
            {
                _playerCurrentHp -= (int)dmg;
                //Processing when HP decreases
                OnReceiveDamage?.Invoke(InitialHp, _playerCurrentHp);
                _regenerationTimer = 0;
                SetRegenerate(false);
                _playerParam.SetIsDamage(true);
            }

            //at time of death
            if (_playerCurrentHp <= 0f)
            {
            }
        }


        public void Kill()
        {
            _playerCurrentHp = 0;
        }

        void Regeneration()
        {
            //リジェネ
            if (_isRegeneration && _initialHp > _playerCurrentHp)
            {
                _playerCurrentHp += _regenerationSpeed * Time.deltaTime;
                OnRegeneration?.Invoke(InitialHp, _playerCurrentHp);
            }
            else if (_isRegeneration)
            {
                _playerCurrentHp = _initialHp;
            }

            //リジェネの待ち時間
            if (_regenerationTimer > _regeneratWaitTime && _initialHp > _playerCurrentHp)
            {
                SetRegenerate(true);
                _regenerationTimer = 0;
            }
            else if (_initialHp > _playerCurrentHp)
            {
                _regenerationTimer += Time.deltaTime;
            }
        }

        void SetRegenerate(bool value)
        {
            _isRegeneration = value;
        }
    }
}
