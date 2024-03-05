using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateIdle
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = !false;

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateIdle)}: Enter");
            }
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateIdle)}: Update");
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateIdle)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
        }
    }
}