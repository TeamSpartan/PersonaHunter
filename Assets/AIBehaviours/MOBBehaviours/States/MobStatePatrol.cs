using SgLibUnite.AI;
using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStatePatrol
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = !false;

        #region Parameter Inside

        private PatrollerPathContainer _patrolPath;
        private Transform _selfTransform;
        private NavMeshAgent _agent;
        private int _currentPathIndex = 0;

        #endregion

        public MobStatePatrol()
        {
        }

        public MobStatePatrol(PatrollerPathContainer pathContainer)
        {
            _patrolPath = pathContainer;
        }
        
        // どのポイントが一番近いか探索
        Vector3 FindNearestPoint(Vector3[] points, ref int currentIndexFeild)
        {
            var min = Vector3.Distance(_selfTransform.position, points[0]);
            var index = 0;
            for (int i = 0; i < points.Length; i++)
            {
                var d = Vector3.Distance(_selfTransform.position, points[i]);
                if (min > d)
                {
                    min = d;
                    index = i;
                }
            }

            currentIndexFeild = index;
            return points[index];
        }

        // 次のポイントへ行くか思考
        void ThinkTogoNextPoint()
        {
            var d = Vector3.Distance(this._selfTransform.position, _patrolPath.GetPatrollingPath[_currentPathIndex]);
            if (d <= .75f)
            {
                if (_currentPathIndex + 1 < _patrolPath.GetPatrollingPath.Length)
                {
                    _currentPathIndex++;
                }
                else
                {
                    _currentPathIndex = 0;
                }

                _agent.SetDestination(_patrolPath.GetPatrollingPath[_currentPathIndex]);
            }
        }

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStatePatrol)}: Enter");
            }

            var dest = FindNearestPoint(_patrolPath.GetPatrollingPath, ref _currentPathIndex);
            _agent.SetDestination(dest);
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStatePatrol)}: Update");
            }

            ThinkTogoNextPoint();
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStatePatrol)}: Exit");
            }

            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
        }
        
        public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent)
        {
            _selfTransform = selfTransform;
            _agent = agent;
        }
    }
}