using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStatePatrol
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = false;

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStatePatrol)}: Enter");
            }
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStatePatrol)}: Update");
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStatePatrol)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
        }
    }
}