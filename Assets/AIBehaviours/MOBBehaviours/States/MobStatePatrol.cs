using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStatePatrol
    : ISequensableState
    , IEnemyState
    {
        public void Entry()
        {
            Debug.Log($"{nameof(MobStatePatrol)}: Enter");
        }

        public void Update()
        {
            Debug.Log($"{nameof(MobStatePatrol)}: Update");
        }

        public void Exit()
        {
            Debug.Log($"{nameof(MobStatePatrol)}: Exit");
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
            
        }
    }
}