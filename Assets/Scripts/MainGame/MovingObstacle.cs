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

    private SpriteRenderer _renderer;
    private Collider2D _collider;

    private float _idleTimer;

    public float IdleTimer
    {
        get { return _idleTimer; }
        set
        {
            _idleTimer = value;

            if (value <= 0)
            {
                Enable();
                StartMoving();
            }
            else
            {
                Disable();
            }
        }
    }

    public void StartMoving()
    {
        transform.position = _startPosition.position;
    }

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        IdleTimer = _startDelay;
    }

    private void FixedUpdate()
    {
        if (IdleTimer > 0f)
        {
            IdleTimer -= Time.fixedDeltaTime;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _endPosition.position, _speed * Time.fixedDeltaTime);
        
        if (Vector2.Distance(transform.position, _endPosition.position) < 0.1f)
        {
            StartMoving();
        }

    }

    private void Disable()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
    }

    private void Enable()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            IdleTimer = _playerBlowUpDelay;
        }
    }
}
