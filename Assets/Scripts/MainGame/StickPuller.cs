using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPuller : MonoBehaviour
{
    [SerializeField] private float _centerPullMagnitude = 1f;
    [SerializeField] private float _outerPullMagnitude = 0.1f;
    [SerializeField] private float _centerRadius = 0.1f;
    [SerializeField] private CircleCollider2D _collider;

    private float _radius;

    private void Start()
    {
        _radius = _collider.radius;
    }

    public Vector2 GetPullVector(Vector2 worldPosition)
    {
        var deltaVector = (Vector2)transform.position - worldPosition;
        var ratio = deltaVector.magnitude / _radius;
        
        if (deltaVector.magnitude <= _centerRadius)
        {
            return Vector2.zero;
        }

        if (ratio <= 1)
        {
            return deltaVector.normalized * Mathf.Lerp(_outerPullMagnitude, _centerPullMagnitude, ratio);
        }
        else
        {
            return Vector2.zero;
        }
    }
}
