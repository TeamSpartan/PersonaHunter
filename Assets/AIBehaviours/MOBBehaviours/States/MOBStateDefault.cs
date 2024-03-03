using SgLibUnite.StateSequencer;
using SgLibUnite.AI;
using UnityEngine;
using UnityEngine.AI;

namespace MOBState
{
    /// <summary>
    /// オモテガリ デフォルトステート
    /// </summary>
    public class MOBStateDefault
    :   EnemyStateBaseClass
        ,ISequensableState
    {
        /// <summary>
        /// パトロールの道筋
        /// </summary>
        private PatrollerPathContainer _pathContainer;

        private NavMeshAgent _agent;

        private Transform _target, _selfTransform;

        public MOBStateDefault(NavMeshAgent agent)
        {
            _agent = agent;
        }
        
        public void Entry()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void TaskOnUpdate(Transform position, Transform destination)
        {
            _selfTransform = position;
            _target = destination;
        }
    }
}
