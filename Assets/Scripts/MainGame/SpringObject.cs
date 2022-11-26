using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringObject : MonoBehaviour
{
    [SerializeField] private float _springMagnitude = 1f;
    [SerializeField] private float _springBounceAngle = 45f;

    public float SpringBounceAngle => _springBounceAngle;
    public float SpringMagnitude => _springMagnitude;
}
