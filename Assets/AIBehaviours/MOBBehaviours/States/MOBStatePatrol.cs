using System.Linq;
using SgLibUnite.StateSequencer;
using SgLibUnite.AI;
using UnityEngine;
using UnityEngine.AI;

namespace MOBState
{
    /// <summary>
    /// オモテガリ デフォルトステート
    /// </summary>
    public class MOBStatePatrol
        : EnemyStateBaseClass
            , ISequensableState
    {
        private NavMeshAgent _agent;

        private Transform _destination, _selfTransform;

        private int _currentPathIndex;

        private Vector3[] _patrollingPath;

        private float _threshordDistToGoNext;

        /// <summary>
        /// 次の目標地点へ行くと判定する距離の閾（しきい）値
        /// </summary>
        public float ThreshordToGoNext
        {
            get { return this._threshordDistToGoNext; }
            set { this._threshordDistToGoNext = value; }
        }

        public MOBStatePatrol(NavMeshAgent agent
            , PatrollerPathContainer pathContainer
            , Transform aiPosition
            , Transform playerDestination)
        {
            _agent = agent;
            _patrollingPath = null;
            _patrollingPath = new Vector3[pathContainer.GetPatrollingPath.Length];
            _patrollingPath = pathContainer.GetPatrollingPath;

            _selfTransform = aiPosition;
            _destination = playerDestination;

            _currentPathIndex = 0;
        }

        void SetDestinationAuto()
        {
            _agent.SetDestination(_patrollingPath[_currentPathIndex]);
        }

        Vector3 GetCurrentDestination()
        {
            return _patrollingPath[_currentPathIndex];
        }

        public void Entry()
        {
            // ここで一番近い目標地点を探索
            float minDis = 0f;

            for (int i = 0; i < _patrollingPath.Length; i++)
            {
                var dis = Vector3.Distance(_selfTransform.position, _patrollingPath[i]);
                if (dis < minDis)
                {
                    minDis = dis;
                    _currentPathIndex = i;
                } // update minimum distance index of array
            } // 現状の座標から一番近いポイントを探索

            SetDestinationAuto();
            
            Debug.Log($"{nameof(MOBStatePatrol)} Entry");
        }

        public void Update()
        {
            // ここでパトロール処理
            float dis = Vector3.Distance(_selfTransform.position, GetCurrentDestination());
            if (dis < this._threshordDistToGoNext)
            {
                ++_currentPathIndex;
                SetDestinationAuto();
            }
            
            Debug.Log($"{nameof(MOBStatePatrol)} Tick");
            
        }

        public void Exit()
        {
            _agent.ResetPath(); // とりあえずリセット
            
            Debug.Log($"{nameof(MOBStatePatrol)} Exit");
        }

        public override void TaskOnUpdate(Transform aiPosition, Transform playerDestination)
        {
            _selfTransform = aiPosition;
            _destination = playerDestination;
        }
    }
}
