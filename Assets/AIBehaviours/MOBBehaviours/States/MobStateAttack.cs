using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateAttack
    : ISequensableState
    , IEnemyState
    {
        public void Entry()
        {
            Debug.Log($"{nameof(MobStateAttack)}: Enter");
        }

        public void Update()
        {
            Debug.Log($"{nameof(MobStateAttack)}: Update");
        }

        public void Exit()
        {
            Debug.Log($"{nameof(MobStateAttack)}: Exit");
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
            
        }
    }
}