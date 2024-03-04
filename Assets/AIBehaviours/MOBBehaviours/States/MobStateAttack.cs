using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateAttack
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = false;

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateAttack)}: Enter");
            }
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateAttack)}: Update");
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateAttack)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
        }
    }
}