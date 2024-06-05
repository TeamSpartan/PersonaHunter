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
        private float _moveV;
        private float _mouseV;

        public event Action ELockOnTriggered;
        public Action EvtCamLeftTarget { get; set; }
        public Action EvtCamRightTarget { get; set; }

        public void InitializeThisComponent()
        {
            _elapsedT = 0f;
        }

        public void FixedTickThisComponent()
        {
            
        }

        public void TickThisComponent()
        {
            if (UnityEngine.Input.GetMouseButton(2)  || UnityEngine.Input.GetKey(KeyCode.L))
            {
                _elapsedT += Time.deltaTime;
            }

            _moveH = UnityEngine.Input.GetAxis("Horizontal");
            _moveV = UnityEngine.Input.GetAxis("Vertical");
            _mouseH = UnityEngine.Input.GetAxis("Mouse X");
            _mouseV = UnityEngine.Input.GetAxis("Mouse Y"); 
            
            // 左 ターゲット ロックオン
            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Look L");
                EvtCamLeftTarget();
            }
            // 右 ターゲット ロックオン
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log($"Look R");
                EvtCamRightTarget();
            }

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

        public void PauseThisComponent()
        {
            throw new NotImplementedException();
        }

        public void ResumeThisComponent()
        {
            throw new NotImplementedException();
        }

        public Vector2 GetMoveValue()
        {
            return new Vector2(_moveH, _moveV);
        }

        public Vector2 GetCamMoveValue()
        {
            return new Vector2(_mouseH, _mouseV);
        }
    }
}
