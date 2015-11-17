using UnityEngine;
using System.Collections;
using System.Linq;

namespace FSM
{
    public class FsmTransition
    {
        public FsmState State { get; set; }
        public FsmCondition[] Conditions { get; set; }

        public bool MeetConditions()
        {
            return Conditions.All(item => item.MeetCondition());
        }
    }
}