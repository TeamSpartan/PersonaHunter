using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    /// <summary>
    ///  作成：菅沼
    /// オモテガリ MOBステート プレイヤの追跡
    /// </summary>
    public class MobStateTrack
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = !false;

        #region ParamInside

        private Transform _selfTransform;
        private Transform _playerTransform;
        private NavMeshAgent _agent;
        private float _attackingRange = 0;

        #endregion

        public MobStateTrack(){}
        
        public MobStateTrack(float attackingRange) => _attackingRange = attackingRange;

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateTrack)}: Enter");
            }

            _agent.SetDestination(_playerTransform.position);
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateTrack)}: Update");
            }

            var d = Vector3.Distance(_selfTransform.position, _playerTransform.position);
            if (d > _attackingRange)
            {
                _agent.SetDestination(_playerTransform.position);
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateTrack)}: Exit");
            }
            
            if(_agent.hasPath)
            {_agent.ResetPath();}
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent)
        {
            _selfTransform = selfTransform;
            _playerTransform = targetTransform;
            _agent = agent;
        }
    }
}