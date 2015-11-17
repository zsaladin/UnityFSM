using UnityEngine;
using System.Linq;
using System.Collections.Generic;


namespace FSM
{
    public partial class FsmState
    {
        public FsmAction[] _actions;
        public FsmTransition[] _transitions;

        public Fsm FSM { get; private set; }

        public FsmState(Fsm fsm)
        {
            FSM = fsm;
        }

        public void Start()
        {
            foreach (FsmAction action in _actions)
            {
                action.OnStart();
            }
        }

        public void Update()
        {
            foreach (FsmAction action in _actions)
            {
                action.OnUpdate();
            }
        }

        public void Exit()
        {
            foreach(FsmAction action in _actions)
            {
                action.OnExit();
            }
        }

        public FsmState NextState()
        {
            FsmTransition transition = _transitions.FirstOrDefault(item => item.MeetConditions());
            return transition != null ? transition.State : null;
        }
    }
}