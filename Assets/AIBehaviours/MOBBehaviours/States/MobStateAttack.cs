using System;
using System.Linq;
using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateAttack
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = !false;

        #region Parameter Inside

        private Transform _selfTransform;
        private Transform _playerTransform;
        private NavMeshAgent _agent;
        private LayerMask _playersLayerMask;
        private Action onEndAttack;
        private float _damage = 0;
        private float _attackRange = 0f;
        private float _elapsedTime = 0f;
        private float _attackingInterval = 0f;
        private float _deltaTime = 0;
        
        #endregion
        
        public MobStateAttack(){}

        public MobStateAttack(float attackRange, float damage, float attackingInterval, LayerMask playersLayerMask, Action taskOnEndAttack)
        {
            this._attackRange = attackRange;
            this._damage = damage;
            this._attackingInterval = attackingInterval;
            this.onEndAttack = taskOnEndAttack;
            this._playersLayerMask = playersLayerMask;
        }

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateAttack)}: Enter");
            }
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateAttack)}: Update");
            }

            // 間隔を計測
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _attackingInterval)
            {
                var colliders = Physics.OverlapSphere(_selfTransform.position, _attackRange, _playersLayerMask);
                colliders.ToList()
                    .Where(_ => _.GetComponent<IDamagedComponent>() != null)
                    .Select(_ => _.GetComponent<IDamagedComponent>()).ToList()
                    .ForEach(_ => _.AddDamage(_damage));
                _elapsedTime = 0f;
                onEndAttack();
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateAttack)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent, float deltaTime)
        {
            _selfTransform = selfTransform;
            _playerTransform = targetTransform;
            _agent = agent;
            _deltaTime = deltaTime;
        }
    }
}