using UnityEngine;
using System.Collections.Generic;

namespace FSM
{
    public class Fsm : MonoBehaviour
    {
        public FsmState State { get; private set; }
        public FsmState PrevState { get; private set; }

        void Start()
        {
            
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
