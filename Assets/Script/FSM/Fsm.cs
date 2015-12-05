using UnityEngine;
using System.Collections.Generic;

namespace FSM
{
    public class Fsm : MonoBehaviour
    {
        public FsmRecord Record { get; set; }

        public FsmState State { get; private set; }
        public FsmState PrevState { get; private set; }

        void Start()
        {
            FsmState idle = new FsmState(this);
            idle._actions = new FsmAction[] { new FsmAction_Idle(idle) };

            FsmState move = new FsmState(this);
            move._actions = new FsmAction[] { new FsmAction_Move(move) };

            idle._transitions = new FsmTransition[]
            {
                new FsmTransition
                {
                    NextState = move,
                    Conditions = new List<FsmCondition>
                    {
                        new FsmCondition_Input(idle) { IsPressedCondition = true }
                    }
                }
            };

            move._transitions = new FsmTransition[]
            {
                new FsmTransition
                {
                    NextState = idle,
                    Conditions = new List<FsmCondition>
                    {
                        new FsmCondition_Input(move) { IsPressedCondition = false }
                    }
                }
            };

            SetNextState(idle);
        }

        void Update()
        {
            if (State != null)
            {
                State.Update();
                FsmState nextState = State.NextState();
                if (nextState != null)
                {
                    SetNextState(nextState);
                }
            }
        }

        void SetNextState(FsmState nextState)
        {
            if (State != null)
            {
                State.Exit();
            }

            PrevState = State;
            State = nextState;
            State.Start();
        }
    }
}
