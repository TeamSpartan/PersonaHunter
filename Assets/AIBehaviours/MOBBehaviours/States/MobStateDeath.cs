using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateDeath
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = !false;

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDeath)}: Enter");
            }
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDeath)}: Update");
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateDeath)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
        }
    }
}