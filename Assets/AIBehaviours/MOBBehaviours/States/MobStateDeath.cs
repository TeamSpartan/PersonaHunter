using System;
using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    /// <summary>
    ///  作成：菅沼
    /// オモテガリ MOBステート 死亡
    /// </summary>
    public class MobStateDeath
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = !false;

        #region Parameter Inside

        private Transform _selfTransform;
        private Transform _playerTransform;
        private NavMeshAgent _agent;
        private Action onDeath;
        private float _timeToDispose = 0f;
        private float _elapsedTIme = 0f;

        #endregion

        public MobStateDeath()
        {
        }

        public MobStateDeath(float timeToDispose, Action taskOnDeath)
        {
            _timeToDispose = timeToDispose;
            onDeath = taskOnDeath;
        }

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDeath)}: Enter");
            }

            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDeath)}: Update");
            }

            _elapsedTIme += Time.deltaTime;
            if (_elapsedTIme >= _timeToDispose)
            {
                onDeath();
                GameObject.Destroy(_selfTransform.gameObject);
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDeath)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent)
        {
            _selfTransform = selfTransform;
            _playerTransform = targetTransform;
            _agent = agent;
        }
    }
}
