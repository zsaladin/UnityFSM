using UnityEngine;
using System.Collections;

namespace FSM
{
    public class FsmAction_Idle : FsmAction
    {
        public FsmAction_Idle(FsmState state)
            : base(state)
        {

        }
        
        public override void OnStart()
        {
            State.FSM.GetComponent<Animator>().SetFloat("Speed", 0f);
        }
    }
}
