using UnityEngine;
using System.Collections;
using System;

namespace FSM
{
    public class FsmCondition_Input : FsmCondition
    {
        public bool IsPressedCondition { get; set; }

        public FsmCondition_Input(FsmState state)
            : base(state)
        {

        }

        public override bool MeetCondition()
        {
            bool isPressed = Mathf.Abs(Input.GetAxis("Horizontal")) > float.Epsilon || Mathf.Abs(Input.GetAxis("Vertical")) > float.Epsilon;
            return !(IsPressedCondition ^ isPressed);
        }
    }
}