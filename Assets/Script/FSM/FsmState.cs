using UnityEngine;
using System.Linq;
using System.Collections.Generic;


namespace FSM
{
    public partial class FsmState
    {
        Action[] _actions;
        TransitionSet[] _transitionSets;

        public void Start()
        {
            foreach (Action action in _actions)
            {
                action.OnStart();
            }
        }

        public void Update()
        {
            foreach (Action action in _actions)
            {
                action.OnUpdate();
            }
        }

        public void Exit()
        {
            foreach(Action action in _actions)
            {
                action.OnExit();
            }
        }

        public FsmState NextState()
        {
            return _transitionSets.FirstOrDefault(item =>
            {
                return item.Conditions.All(item2 => item2.MeetCondition());
            }).NextState;
        }
    }

    partial class FsmState
    {
        public abstract class Action
        {
            public virtual void OnStart() { }
            public virtual void OnUpdate() { }
            public virtual void OnExit() { }
        }

        public abstract class TransitionCondition
        {
            public abstract bool MeetCondition();
        }

        public struct TransitionSet
        {
            public FsmState NextState { get; set; }
            public List<TransitionCondition> Conditions { get; set; }
        }
    }
}