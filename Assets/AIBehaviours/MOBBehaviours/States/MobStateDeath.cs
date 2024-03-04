using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateDeath
    : ISequensableState
    , IEnemyState
    {
        public void Entry()
        {
            Debug.Log($"{nameof(MobStateDeath)}: Enter");
        }

        public void Update()
        {
            Debug.Log($"{nameof(MobStateDeath)}: Update");
        }

        public void Exit()
        {
            Debug.Log($"{nameof(MobStateDeath)}: Exit");
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
            
        }
    }
}