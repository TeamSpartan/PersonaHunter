using System;
using System.Timers;
using SgLibUnite.StateSequencer;
using UnityEngine;
using UnityEngine.AI;

namespace AIBehaviours.MOBBehaviours.States
{
    public class MobStateFlinch
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
        private float _deltaTime = 0f;
        private Action onEndFlinching;

        #endregion

        public MobStateFlinch(){}

        public MobStateFlinch(float flinchingTime, Action taskOnEndFlinching)
        {
            _flinchingTime = flinchingTime;
            onEndFlinching = taskOnEndFlinching;
        }

        public void Entry()
        {
            if (_debuggging)
            {
                Debug.Log($"{nameof(MobStateFlinch)}: Entry");
            }

            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
        }

        public void Update()
        {
            if (_debuggging)
            {
                Debug.Log($"{nameof(MobStateFlinch)}: Update {_deltaTime}");
            }

            _elapsedTime += _deltaTime;
            if (_elapsedTime >= _flinchingTime)
            {
                onEndFlinching();
                _elapsedTime = 0f;
            }
        }

        public void Exit()
        {
            if (_debuggging)
            {
                Debug.Log($"{nameof(MobStateFlinch)}: Exit");
            }
        }

        public void UpdateState(Transform selfTransform, Transform targetTransform, NavMeshAgent agent, float detltaTime)
        {
            _selfTransform = selfTransform;
            _playerTransform = targetTransform;
            _agent = agent;
            _deltaTime = detltaTime;
        }
    }
}