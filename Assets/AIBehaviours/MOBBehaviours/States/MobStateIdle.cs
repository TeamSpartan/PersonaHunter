using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateIdle
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = !false;

        #region Parameter Inside

        private Transform _selfTransform;
        private Transform _playerTransform;
        private NavMeshAgent _agent;

        #endregion

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateIdle)}: Enter");
            }
            
            // その場所にとどまる。
            if(_agent.hasPath)
            {_agent.ResetPath();}
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateIdle)}: Update");
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateIdle)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent, float detltaTime)
        {
            _selfTransform = selfTransform;
            _playerTransform = targetTransform;
            _agent = agent;
        }
    }
}