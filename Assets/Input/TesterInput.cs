using System;
using UnityEngine;

namespace Input
{
    public class TesterInput
        : MonoBehaviour
            , IInitializableComponent
            , ILockOnEventFirable
            , IInputValueReferencable
    {
        private float _elapsedT;
        private float _moveH;
        private float _mouseH;

        public event Action ELockOnTriggered;

        public void InitializeThisComponent()
        {
            _elapsedT = 0f;
        }

        private void FixedUpdate()
        {
            if (UnityEngine.Input.GetMouseButton(2))
            {
                _elapsedT += Time.deltaTime;
            }

            _moveH = UnityEngine.Input.GetAxis("Horizontal");
            _mouseH = UnityEngine.Input.GetAxis("Mouse X");

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

        public float GetHorizontalMoveValue()
        {
            return this._moveH;
        }

        public float GetHorizontalMouseMoveValue()
        {
            return this._mouseH;
        }
    }
}
