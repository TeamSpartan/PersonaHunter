using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateFlinch
    : ISequensableState
    , IEnemyState
    {
        private bool _debuggging = !false;
        
        public void Entry()
        {
            Debug.Log($"{nameof(MobStateFlinch)}: Entry");
        }

        public void Update()
        {
            Debug.Log($"{nameof(MobStateFlinch)}: Update");
        }

        public void Exit()
        {
            Debug.Log($"{nameof(MobStateFlinch)}: Exit");
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent)
        {
        }
    }
}