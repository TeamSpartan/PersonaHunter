using SgLibUnite.StateSequencer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateDamaged
    :ISequensableState
    , IEnemyState
    {
        private bool _debugging = !false;

        #region Parameter Inside

        private Transform _selfTransform;
        private Transform _playerTransform;
        private NavMeshAgent _agent;

        #endregion
        
        public MobStateDamaged(){}
        
        
        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDamaged)}: Enter");
            }
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDamaged)}: Update");
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDamaged)}: Exit");
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