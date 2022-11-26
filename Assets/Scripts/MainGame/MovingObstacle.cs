using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private Transform _endPosition;

    public void StartMoving()
    {

    }

    private void Start()
    {
        StartMoving();
    }

    private void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
