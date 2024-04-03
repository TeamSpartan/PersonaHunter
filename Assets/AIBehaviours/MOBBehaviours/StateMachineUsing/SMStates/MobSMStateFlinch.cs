using System;
using System.Timers;
using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    /// <summary>
    ///  作成：菅沼
    /// オモテガリ MOBステート ひるみ（ダウン）
    /// </summary>
    public class MobSMStateFlinch
        : ISequensableState
            , IEnemyState
    {
        private bool _debuggging = !false;

        #region Parameter Inside

        private Transform _selfTransform;
        private Transform _playerTransform;
        private NavMeshAgent _agent;
        private float _elapsedTime = 0f;
        private float _flinchingTime = 0f;
        private Action onEndFlinching;

        #endregion

        public MobSMStateFlinch(){}

        public MobSMStateFlinch(float flinchingTime, Action taskOnEndFlinching)
        {
            _flinchingTime = flinchingTime;
            onEndFlinching = taskOnEndFlinching;
        }

        public void Entry()
        {
            if (_debuggging)
            {
                Debug.Log($"{nameof(MobSMStateFlinch)}: Entry");
            }

            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
            
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _flinchingTime)
            {
                onEndFlinching();
                _elapsedTime = 0f;
            }
        }

        public void Update()
        {
            if (_debuggging)
            {
                Debug.Log($"{nameof(MobSMStateFlinch)}: Update");
            }
        }

        public void Exit()
        {
            if (_debuggging)
            {
                Debug.Log($"{nameof(MobSMStateFlinch)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent)
        {
            _selfTransform = selfTransform;
            _playerTransform = targetTransform;
            _agent = agent;
        }
    }
}