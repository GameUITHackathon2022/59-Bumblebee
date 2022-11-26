using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private Transform _endPosition;
    [SerializeField] private float _startDelay = 0.5f;
    [SerializeField] private float _playerBlowUpDelay = 1f;

    private float _idleTimer;

    public float IdleTimer
    {
        get { return _idleTimer; }
        set
        {
            _idleTimer = value;
            gameObject.SetActive(false);
        }
    }

    public void StartMoving()
    {
        transform.position = _startPosition.position;
    }

    private void Start()
    {
        StartMoving();
    }

    private void FixedUpdate()
    {
        if (gameObject.activeSelf)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _endPosition.position, _speed * Time.fixedDeltaTime);
        
        if (Vector2.Distance(transform.position, _endPosition.position) < 0.1f)
        {
            StartMoving();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            IdleTimer = _playerBlowUpDelay;
        }
    }
}
