using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;

    private void FixedUpdate()
    {
        var angle = transform.rotation.eulerAngles;
        angle.z += _speed * Time.fixedDeltaTime;
        transform.localRotation = Quaternion.Euler(angle);
    }
}
