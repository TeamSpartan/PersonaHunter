using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    /// <summary>
    ///  作成：菅沼
    /// オモテガリ MOBステート その場にとどまる
    /// </summary>
    public class MobSMStateIdle
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
                Debug.Log($"{nameof(MobSMStateIdle)}: Enter");
            }
            
            // その場所にとどまる。
            if(_agent.hasPath)
            {_agent.ResetPath();}
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobSMStateIdle)}: Update");
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobSMStateIdle)}: Exit");
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