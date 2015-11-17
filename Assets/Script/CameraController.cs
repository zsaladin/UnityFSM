using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _target;

	void Update ()
    {
        if (_target == null)
            return;

        transform.position = _target.position - _target.forward * 2f + _target.up * 1.7f;
        transform.LookAt(_target.position + _target.up * 1.1f);
	}
}
