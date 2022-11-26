using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFixer : MonoBehaviour
{
    private Quaternion _initRotation;

    private void Start()
    {
        _initRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = _initRotation;
    }
}
