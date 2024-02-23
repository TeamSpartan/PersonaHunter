// 管理者 菅沼
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SgLibUnite
{
    namespace StateSequencer
    {
        /// <summary> ステートマシンの機能を提供する </summary>
        public class StateSequencer
        {
            // 通常ステート
            HashSet<ISequensableState> _states = new HashSet<ISequensableState>();
            // Anyステートからのステート
            HashSet<ISequensableState> _statesFromAnyState = new HashSet<ISequensableState>();
            // トランジション
            HashSet<StateMachineTransition> _transitions = new HashSet<StateMachineTransition>();
            // Anyからのトランジション
            HashSet<StateMachineTransition> _transitionsFromAny = new HashSet<StateMachineTransition>();
            // 現在突入しているステート
            ISequensableState _currentPlayingSequensableState;
            // 現在突入しているトランジション名
            string _currentTransitionName;
            // ステートマシンが一時停止中かのフラグ
            bool _bIsPausing = true;
            // デリゲート公開部
            public event Action<string> OnEntered;
            public event Action<string> OnUpdated;
            public event Action<string> OnExited;

            #region 登録処理
            /// <summary> ステートの登録 </summary>
            /// <param name="sequensableState"></param>
            public void ResistState(ISequensableState sequensableState)
            {
                _states.Add(sequensableState);
                if (_currentPlayingSequensableState == null) { _currentPlayingSequensableState = sequensableState; }
            }

            /// <summary> Anyからのステートの登録 </summary>
            /// <param name="sequensableState"></param>
            public void ResistStateFromAny(ISequensableState sequensableState)
            {
                _statesFromAnyState.Add(sequensableState);
            }

            /// <summary> 複数のステートを引数に渡してすべての渡されたステートを登録 </summary>
            /// <param name="states"></param>
            public void ResistStates(List<ISequensableState> states)
            {
                foreach (ISequensableState state in states)
                {
                    _states.Add(state);
                    if (_currentPlayingSequensableState == null) { _currentPlayingSequensableState = state; }
                }
            }

            /// <summary> 複数のステートを引数に渡してすべての渡されたAnyからのステートを登録 </summary>
            /// <param name="states"></param>
            public void ResistStatesFromAny(List<ISequensableState> states)
            {
                foreach (ISequensableState state in _statesFromAnyState)
                {
                    _states.Add(state);
                }
            }

            /// <summary> ステート間の遷移の登録 </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <param name="name"></param>
            public void MakeTransition(ISequensableState from, ISequensableState to, string name)
            {
                var tmp = new StateMachineTransition(from, to, name);
                _transitions.Add(tmp);
            }

            /// <summary> Anyステートからの遷移の登録 </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <param name="name"></param>
            public void MakeTransitionFromAny(ISequensableState to, string name)
            {
                var tmp = new StateMachineTransition(new DummySequensableStateClass(), to, name);
                _transitionsFromAny.Add(tmp);
            }

            #endregion

            #region 更新処理
            /// <summary> 任意のステート間遷移の遷移の状況を更新する。 </summary>
            /// <param name="name"></param>
            /// <param name="condition2transist"></param>
            /// <param name="tType"></param>
            /// <param name="equalsTo"></param>
            public void UpdateTransition(string name, ref bool condition2transist, bool equalsTo = true, bool isTrigger = false)
            {
                if (_bIsPausing) return; // もし一時停止中なら更新処理はしない。
                foreach (var t in _transitions)
                {
                    // 遷移する場合 // * 条件を満たしているなら前トランジションを無視してしまうのでその判定処理をはさむこと *
                    // もし遷移条件を満たしていて遷移名が一致するなら
                    if ((condition2transist == equalsTo) && t.Name == name)
                    {
                        if (t.SFrom == _currentPlayingSequensableState) // 現在左ステートなら
                        {
                            _currentPlayingSequensableState.Exit(); // 右ステートへの遷移条件を満たしたので抜ける
                            if (OnExited != null)
                                OnExited(_currentTransitionName);
                            if (isTrigger) condition2transist = !equalsTo; // IsTrigger が trueなら
                            _currentPlayingSequensableState = t.STo; // 現在のステートを右ステートに更新、遷移はそのまま
                            _currentPlayingSequensableState.Entry(); // 現在のステートの初回起動処理を呼ぶ
                            if (OnEntered != null)
                                OnEntered(_currentTransitionName);
                            _currentTransitionName = name; // 現在の遷移ネームを更新
                        }
                    }
                    // 遷移の条件を満たしてはいないが、遷移ネームが一致（更新されていないなら）現在のステートの更新処理を呼ぶ
                    else if (t.Name == name)
                    {
                        _currentPlayingSequensableState.Update();
                        if (OnUpdated != null)
                            OnUpdated(_currentTransitionName);
                    }
                } // 全遷移を検索。
            }

            /// <summary> ANYステートからの遷移の条件を更新 </summary>
            /// <param name="name"></param>
            /// <param name="condition2transist"></param>
            /// <param name="equalsTo"></param>
            public void UpdateTransitionFromAnyState(string name, ref bool condition2transist, bool equalsTo = true, bool isTrigger = false)
            {
                if (_bIsPausing) return; // もし一時停止中なら更新処理はしない。
                foreach (var t in _transitionsFromAny)
                {
                    // もし遷移条件を満たしていて遷移名が一致するなら
                    if ((condition2transist == equalsTo) && t.Name == name)
                    {
                        _currentPlayingSequensableState.Exit(); // 右ステートへの遷移条件を満たしたので抜ける
                        if (OnExited != null)
                            OnExited(_currentTransitionName);
                        if (isTrigger) condition2transist = !equalsTo; // 遷移条件を初期化
                        _currentPlayingSequensableState = t.STo; // 現在のステートを右ステートに更新、遷移はそのまま
                        _currentPlayingSequensableState.Entry(); // 現在のステートの初回起動処理を呼ぶ
                        if (OnEntered != null)
                            OnEntered(_currentTransitionName);
                        _currentTransitionName = name; // 現在の遷移ネームを更新
                    }
                    // 遷移の条件を満たしてはいないが、遷移ネームが一致（更新されていないなら）現在のステートの更新処理を呼ぶ
                    else if (t.Name == name)
                    {
                        _currentPlayingSequensableState.Update();
                        if (OnUpdated != null)
                            OnUpdated(_currentTransitionName);
                    }
                } // 全遷移を検索。
            }
            #endregion

            #region 起動処理
            /// <summary> ステートマシンを起動する </summary>
            public void PopStateMachine()
            {
                _bIsPausing = false;
                _currentPlayingSequensableState.Entry();
            }
            #endregion

            #region 一時停止処理
            /// <summary> ステートマシンの処理を一時停止 </summary>
            public void PushStateMachine()
            {
                _bIsPausing = true;
            }
            #endregion
        }
        // 各トランジションは名前を割り当てている
        /// <summary> ステート間遷移の情報を格納している </summary>
        public class StateMachineTransition
        {
            ISequensableState _from;
            public ISequensableState SFrom => _from;
            ISequensableState _to;
            public ISequensableState STo => _to;
            string _name;
            public string Name => _name;
            public StateMachineTransition(ISequensableState from, ISequensableState to, string name)
            {
                _from = from;
                _to = to;
                _name = name;
            }
        }

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
            StandardState,      // 通常 
            AnyState,           // 一フレームのみ遷移 
        }

        #region ステートマシン、利用部構想
        // イニシャライズ処理
        // ステートマシンインスタンス化ステートの登録
        // トランジションの登録
        // ステートマシンの更新

        // 毎フレーム処理
        // トランジションの状態の更新
        #endregion
    }
}