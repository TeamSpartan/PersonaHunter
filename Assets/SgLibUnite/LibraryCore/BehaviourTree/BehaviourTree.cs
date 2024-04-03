// 作成者 菅沼

using System.Collections.Generic;
using System;
using System.Linq;

namespace SgLibUnite.BehaviourTree
{
    /// <summary> BTのビヘイビアのベースクラス </summary>
    public class BTBehaviour
    {
        private List<Action> _behaviours;
        private int _behaviourIndex;

        private Action OnBegin;
        private Action OnTick;
        private Action OnEnd;

        public event Action EBegin
        {
            add { OnBegin += value; }
            remove { OnBegin -= value; }
        }

        public event Action ETick
        {
            add { OnTick += value; }
            remove { OnTick -= value; }
        }

        public event Action EEnd
        {
            add { OnEnd += value; }
            remove { OnEnd -= value; }
        }

        public void AddBehaviour(Action behaviour)
        {
            _behaviours.Add(behaviour);
        }

        public BTBehaviour()
        {
            _behaviours = new();
            _behaviourIndex = 0;
        }

        public BTBehaviour(params Action[] behaviour)
        {
            _behaviours = new();
            _behaviourIndex = 0;
            var b = behaviour.ToList();
            _behaviours = b;
        }

        public void Begin()
        {
            if (OnBegin != null) OnBegin.Invoke();
            if (_behaviours.Count == 0)
            {
                throw new Exception("No State Added On This Behaviour");
            }
        }

        public void Tick()
        {
            if (OnTick != null) OnTick.Invoke();
            if (_behaviourIndex + 1 < _behaviours.Count)
            {
                _behaviourIndex++;
            }
            else
            {
                _behaviourIndex = 0;
            }

            _behaviours[_behaviourIndex].Invoke();
        }

        public void End()
        {
            if (OnEnd != null) OnEnd.Invoke();
        }
    }

    /// <summary> 遷移の情報を格納している </summary>
    public class BTTransition
    {
        private BTBehaviour _from;
        public BTBehaviour From => _from;
        private BTBehaviour _to;
        public BTBehaviour To => _to;
        private string _name;
        public string Name => _name;

        public BTTransition(BTBehaviour from, BTBehaviour to, string name)
        {
            _from = from;
            _to = to;
            _name = name;
        }
    }

    /// <summary> ビヘイビアツリーの機能を提供 </summary>
    public class BehaviourTree
    {
        private HashSet<BTBehaviour> _btBehaviours = new();
        private HashSet<BTTransition> _btTransitions = new();
        private BTBehaviour _currentBehaviour;
        private string _currentTransitionName;
        private bool _isPausing;

        public void ResistBehaviours(params BTBehaviour[] btBehaviours)
        {
            _btBehaviours = btBehaviours.ToHashSet();
            if (_currentBehaviour == null) _currentBehaviour = btBehaviours[0];
        }

        public void MakeTransition(BTBehaviour from, BTBehaviour to, string name)
        {
            var tmp = new BTTransition(from, to, name);
            _btTransitions.Add(tmp);
        }

        public void UpdateTransition(string name, ref bool condition, bool equalsTo = true, bool isTrigger = false)
        {
            if (_isPausing) return;

            foreach (var transition in _btTransitions)
            {
                if (condition == equalsTo && transition.Name == name)
                {
                    _currentBehaviour.End();
                    if (isTrigger) condition = !equalsTo;
                    _currentBehaviour = transition.To;
                    _currentBehaviour.Begin();
                    _currentTransitionName = transition.Name;
                }
                else if (transition.Name == name)
                {
                    _currentBehaviour.Tick();
                }
            }
        }

        public void JumpTo(BTBehaviour behaviour)
        {
            if (_btBehaviours.Contains(behaviour))
            {
                _currentBehaviour = behaviour;
            }
        }

        public void PauseBT()
        {
            _isPausing = true;
        }

        public void StartBT()
        {
            _isPausing = false;
            _currentBehaviour.Begin();
        }
    }
}
