using SgLibUnite.StateSequencer;
using UnityEngine;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateTrack
        : ISequensableState
            , IEnemyState
    {
        private bool _debugging = !false;

        public void Entry()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateTrack)}: Enter");
            }
        }

        public void Update()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateTrack)}: Update");
            }
        }

        public void Exit()
        {
            if (_debugging)
            {
                Debug.Log($"{nameof(MobStateTrack)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform)
        {
        }
    }
}