// 作成者 菅沼
using System.Collections.Generic;
using System;

namespace SgLibUnite.BehaviourTree
{
    /// <summary> ステートとして登録をするクラスが継承するべきインターフェース </summary>
    public interface ISequensableState
    {
        public void Entry();
        public void Update();
        public void Exit();
    }

    /// <summary> ダミーのステートのクラス </summary>
    class DummySequensableStateClass : ISequensableState
    {
        public void Entry()
        {
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }

    /// <summary> ステート遷移のタイプ </summary>
    enum StateMachineTransitionType
    {
        StandardState, // 通常 
        AnyState, // 一フレームのみ遷移 
    }

    /// <summary> ステートの遷移機能を提供する </summary>
    public class BTTransition
    {
        private ISequensableState _from;
        public ISequensableState StateFrom => _from;
        private ISequensableState _to;
        public ISequensableState StateTo => _to;
        private string _name;
        public string Name => _name;

        public BTTransition(ISequensableState from, ISequensableState to, string name)
        {
            _from = from;
            _to = to;
            _name = name;
        }
    }

    /// <summary> ビヘイビアツリーの機能を提供する </summary>
    public class BehaviourTree
    {
        // Normal State
        private HashSet<ISequensableState> _states = new();

        // State From Any
        private HashSet<ISequensableState> _stateFromAny = new();

        // Transition
        private HashSet<BTTransition> _btTransitions = new();

        // Transition From Any
        private HashSet<BTTransition> _btTransitionsFromAny = new();

        // Current State
        private ISequensableState _cPlayingState;

        // Current Transition Name
        private string _cTranstionName;

        // The Status Pausing Or Not
        private bool _isPausing = true;

        // Delegate Exposing
        public event Action<string> OnEntered;
        public event Action<string> OnUpdate;
        public event Action<string> OnExitted;

        #region ResitingProcess

        /// <summary> ステートの登録 </summary>
        public void ResistState(ISequensableState state)
        {
            _states.Add(state);
            if (_cPlayingState == null)
            {
                _cPlayingState = state;
            }
        }

        /// <summary> どのステートからの登録 </summary>
        public void ResistStateFromAny(ISequensableState state)
        {
            _stateFromAny.Add(state);
        }

        /// <summary> 複数のステートを引数に渡す。まとめてステート登録をする </summary>
        public void ResistStates(List<ISequensableState> states)
        {
            foreach (var state in states)
            {
                _states.Add(state);
                if (_cPlayingState == null)
                {
                    _cPlayingState = state;
                }
            }
        }

        /// <summary> 複数のどのステートから遷移可能なステートを引数に渡す。まとめてステート登録をする </summary>
        public void ResistStatesFromAny(List<ISequensableState> states)
        {
            foreach (var state in states)
            {
                _states.Add(state);
            }
        }

        /// <summary> 遷移の作成 </summary>
        public void MakeTransition(ISequensableState from, ISequensableState to, string name)
        {
            var tmp = new BTTransition(from, to, name);
            _btTransitions.Add(tmp);
        }

        /// <summary> どのステートからも遷移可能なステートの遷移の作成 </summary>
        public void MakeTransitionFromAny(ISequensableState from, ISequensableState to, string name)
        {
            var tmp = new BTTransition(new DummySequensableStateClass(), to, name);
            _btTransitionsFromAny.Add(tmp);
        }

        #endregion

        #region UpdateProcess

        /// <summary> 遷移の更新 </summary>
        public void UpdateTransition(string name, ref bool condition, bool equalsTo = true, bool isTrigger = false)
        {
            if(_isPausing) return; // 一時停止中は処理しない
            foreach (var transition in _btTransitions)
            {
                if (condition == equalsTo && transition.Name == name)
                {
                    if (transition.StateFrom == _cPlayingState)
                    {
                        if (OnExitted != null) OnExitted(_cTranstionName);
                        if (isTrigger) condition = !equalsTo;
                        _cPlayingState = transition.StateTo;
                        _cPlayingState.Entry();

                        if (OnEntered != null) OnEntered(_cTranstionName);
                        _cTranstionName = name;
                    }
                }
                else if (transition.Name == name)
                {
                    _cPlayingState.Update();
                    if (OnUpdate != null) OnUpdate(_cTranstionName);
                }
            }
        }

        /// <summary> どのステートからもできる遷移の作成 </summary>
        public void UpdateTransitionFromAny(string name, ref bool condition, bool equalsTo = true,
            bool isTrigger = false)
        {
            if(_isPausing) return;
            foreach (var transition in _btTransitionsFromAny)
            {
                if (condition == equalsTo && transition.Name == name)
                {
                    _cPlayingState.Exit();
                    if (OnExitted != null) OnExitted(_cTranstionName);
                    if (isTrigger) condition = !equalsTo;
                    _cPlayingState = transition.StateTo;
                    _cPlayingState.Entry();

                    if (OnEntered != null) OnEntered(_cTranstionName);
                    _cTranstionName = name;
                }
                else if (transition.Name == name)
                {
                    _cPlayingState.Update();
                    if (OnUpdate != null) OnUpdate(_cTranstionName);
                }
            }
        }

        #endregion

        #region InitializeProcess

        /// <summary> ビヘイビアを開始 </summary>
        public void StartBehaviour()
        {
            _isPausing = false;
            _cPlayingState.Entry();
        }

        #endregion

        #region PauseProcess

        /// <summary> ビヘイビアを一時停止 </summary>
        public void PauseBehaviour()
        {
            _isPausing = true;
        }

        #endregion
    }
}
