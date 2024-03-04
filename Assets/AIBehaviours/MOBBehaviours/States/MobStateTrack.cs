using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateTrack
    : ISequensableState
    , IEnemyState
    {
        public void Entry()
        {
            Debug.Log($"{nameof(MobStateTrack)}: Enter");
        }

        public void Update()
        {
            Debug.Log($"{nameof(MobStateTrack)}: Update");
        }

        public void Exit()
        {
            Debug.Log($"{nameof(MobStateTrack)}: Exit");
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
            
        }
    }
}