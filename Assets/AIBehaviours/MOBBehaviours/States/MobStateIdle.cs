using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateIdle
    : ISequensableState
    , IEnemyState
    {
        public void Entry()
        {
            Debug.Log($"{nameof(MobStateIdle)}: Enter");
        }

        public void Update()
        {
            Debug.Log($"{nameof(MobStateIdle)}: Update");
        }

        public void Exit()
        {
            Debug.Log($"{nameof(MobStateIdle)}: Exit");
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
            
        }
    }
}