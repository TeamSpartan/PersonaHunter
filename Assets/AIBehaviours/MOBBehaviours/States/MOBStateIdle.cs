using SgLibUnite.StateSequencer;
using UnityEngine;

namespace MOBState
{
    public class MOBStateIdle
    : EnemyStateBaseClass
    , ISequensableState
    {
        public override void TaskOnUpdate(Transform aiPosition, Transform playerDestination)
        {
        }

        public void Entry()
        {
            Debug.Log($"{nameof(MOBStateIdle)} Entry");
        }

        public void Update()
        {
            Debug.Log($"{nameof(MOBStateIdle)} Tick");
        }

        public void Exit()
        {
            Debug.Log($"{nameof(MOBStateIdle)} Exit");
        }
    }
}