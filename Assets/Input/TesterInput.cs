using System;
using UnityEngine;

namespace Input
{
    public class TesterInput
        : MonoBehaviour
            , IInitializableComponent
    ,ILockOnEventFirable
    {
        private float _elapsedT;
        public event Action ELockOnTriggered;

        public void InitializeThisComponent()
        {
            _elapsedT = 0f;
        }

        private void FixedUpdate()
        {
            if(UnityEngine.Input.GetMouseButton(2))
            _elapsedT += Time.deltaTime;

            if (_elapsedT > 1)
            {
                ELockOnTriggered();
                _elapsedT = 0;
            }
        }

        public void FinalizeThisComponent()
        {
            _elapsedT = 0;
        }

    }
}
