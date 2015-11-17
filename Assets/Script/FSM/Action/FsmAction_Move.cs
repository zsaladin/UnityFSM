using UnityEngine;
using System.Collections;

namespace FSM
{
    public class FsmAction_Move : FsmAction
    {
        Transform _transform;
        Rigidbody _rigidbody;
        public FsmAction_Move(FsmState state)
            : base(state)
        {
            _transform = state.FSM.transform;
            _rigidbody = state.FSM.GetComponent<Rigidbody>();
        }

        public override void OnUpdate()
        {
            float speed = GetSpeed();
            _rigidbody.MovePosition(_rigidbody.position + _transform.forward * speed * Time.deltaTime);
            State.FSM.GetComponent<Animator>().SetFloat("Speed", speed);

            float angleSpeed = speed >= 0 ? 120f : -120f;
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.AngleAxis(Input.GetAxis("Horizontal") * Time.deltaTime * angleSpeed, _transform.up));
        }


        float GetSpeed()
        {
            float speed = Input.GetAxis("Vertical");
            return speed >= 0 ? speed * 5f : speed * 1.5f;
        }
    }
}