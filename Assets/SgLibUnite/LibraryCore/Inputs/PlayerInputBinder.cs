using System;
using System.Collections;
using System.Collections.Generic;
using SgLibUnite.Singleton;
using UnityEngine;
using UnityEngine.InputSystem;
namespace SgLibUnite
{
    public class PlayerInputBinder : SingletonBaseClass<PlayerInputBinder>
    {
        [SerializeField] InputActionAsset inputAction;

        private void OnEnable()
        {
            GameObject.DontDestroyOnLoad(this);
        }

        // アクション名を指定してそれに登録
        public void BindAxis(string actionMapName, string actionName
        , Action<InputAction.CallbackContext> callbackAction, ActionInvokeFaze actionInvokingFaze)
        {
            var actionMap = inputAction.FindActionMap(actionMapName);
            var action = actionMap.FindAction(actionName);
            switch (actionInvokingFaze)
            {
                case ActionInvokeFaze.Started:
                    action.started += callbackAction;
                    break;
                case ActionInvokeFaze.Performed:
                    action.performed += callbackAction;
                    break;
                case ActionInvokeFaze.Canceled:
                    action.canceled += callbackAction;
                    break;
            }
        }

        public void BindAxis(string actionMapName, string actionName
            , Action<InputAction.CallbackContext> actionOnStarted
            , Action<InputAction.CallbackContext> actionOnPerformed
            , Action<InputAction.CallbackContext> actionOnCanceled)
        {
            var actionMap = inputAction.FindActionMap(actionMapName);
            var action = actionMap.FindAction(actionName);
            action.started += actionOnStarted;
            action.performed += actionOnPerformed;
            action.canceled += actionOnCanceled;
        }

        public void BindAction(string actionMapName, string actionName
        , Action<InputAction.CallbackContext> callbackActionStarted
            , Action<InputAction.CallbackContext> callbackActionCanceled
            , Action<InputAction.CallbackContext> callBackActionOnTriggered
            , bool IsLongPress = false)
        {
            var actionMap = inputAction.FindActionMap(actionMapName);
            var action = actionMap.FindAction(actionName);
            if (!IsLongPress)
            {
                action.started += callbackActionStarted;
                action.canceled += callbackActionCanceled;
            }
            else
            {
                action.performed += callBackActionOnTriggered;
            }
        }

        public T GetActionValueAs<T>(string actionMapName, string actionName)
        {
            var actionMap = inputAction.FindActionMap(actionMapName);
            var action = actionMap.FindAction(actionName);
            T result = default;
            if (action != null)
            {
                var input = action.ReadValueAsObject();
                if (input != null)
                { result = (T)input; }
            }
            return result;
        }

        public bool GetActionValueAsButton(string actionMapName, string actionName)
        {
            var actionMap = inputAction.FindActionMap(actionMapName);
            var action = actionMap.FindAction(actionName);
            return action.IsPressed();
        }

        protected override void ToDoAtAwakeSingleton()
        {
            
        }
    }

    public enum ActionInvokeFaze
    {
        Started,
        Performed,
        Canceled
    }
}