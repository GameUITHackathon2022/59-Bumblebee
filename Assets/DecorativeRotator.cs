using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorativeRotator : MonoBehaviour
{
    void Start()
    {
        float degrees = Random.Range(0f, 360.0f);
        transform.rotation = Quaternion.Euler(Vector3.forward * degrees);
    }
}
