using UnityEngine;
using System.Collections;

namespace FSM
{
    [System.Serializable]
    public class FsmAction_Idle : FsmAction
    {
        int _idleSpeed;

        public FsmAction_Idle(FsmState state)
            : base(state)
        {

        }
        
        public override void OnStart()
        {
            State.FSM.GetComponent<Animator>().SetFloat("Speed", (float)_idleSpeed);
        }
    }
}
