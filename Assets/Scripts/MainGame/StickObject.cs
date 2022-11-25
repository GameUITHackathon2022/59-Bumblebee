using ATL.AudioData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StickObject : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private StickInputReceiver _inputReceiver;
    [SerializeField] private Animator _animator;

    [Header("ContactPoints")]
    [SerializeField] private List<Collider2D> _positiveContactPoints;
    [SerializeField] private List<Collider2D> _negativeContactPoints;

    [Header("Parameters - Movement")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _sprintMultiplier = 1.5f;
    [SerializeField] private float _doubleSprintMultiplier = 2f;

    [Header("Parameters - Spin")]
    [SerializeField] private float _rotateSpeed = 30f;
    [Tooltip("1 means counter-clockwise, -1 is clockwise")]
    [SerializeField] private int _defaultRotation = 1;
    [SerializeField] private float _bounceTime = 0.2f;
    [SerializeField] private float _invincibilityTime = 0.5f;
    [SerializeField] private float _bounceRotation = 35f;

    private int _rotateDirection;
    private float _bounceTimer;
    private int _bounceContactCode;
    private float _invincibilityTimer;

    public bool IsStunned => _bounceTimer > 0;
    public bool IsInvincible => _invincibilityTimer > 0;

    private void Start()
    {
        _rotateDirection = _defaultRotation;
    }

    private void FixedUpdate()
    {
        var inputState = _inputReceiver.QueryInputState();
        ControlPosition(inputState);
        ControlRotation();

        if (_invincibilityTimer > 0)
        {
            _invincibilityTimer = Mathf.Max(0f, _invincibilityTimer - Time.fixedDeltaTime);
        }
    }

    private void ControlPosition(StickInputReceiver.InputState inputState)
    {
        if (IsStunned)
        {
            _rigidbody.velocity = Vector2.zero;
            return;
        }
        var speedMod = 1f;
        if (inputState.SprintModifierMode == 1)
        {
            speedMod = _sprintMultiplier;
        }
        else if (inputState.SprintModifierMode == 2)
        {
            speedMod = _doubleSprintMultiplier;
        }

        var movementVector = inputState.MovementVector * _speed * speedMod * Time.fixedDeltaTime;

        _rigidbody.velocity = movementVector;
    }

    private void ControlRotation()
    {
        if (_bounceTimer > 0)
        {
            _rigidbody.SetRotation(_rigidbody.rotation + _bounceContactCode * (_bounceRotation / _bounceTime) * Time.fixedDeltaTime);
            _bounceTimer = Mathf.Max(0f, _bounceTimer - Time.fixedDeltaTime);
        }
        else
        {
            _rigidbody.SetRotation(_rigidbody.rotation + _rotateDirection * _rotateSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            _bounceTimer = _bounceTime;
            _bounceContactCode = GetContactRotationOnColliding(collision.GetContact(0).point);
            _invincibilityTimer = _invincibilityTime;

            Debug.Log(collision.GetContact(0).normal);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Goal"))
        {

        }
        else if (collider.CompareTag("Puller"))
        {

        }
    }

    private int GetContactRotationOnColliding(Vector2 point)
    {
        int contactCode = 0;

        foreach (var collider in _positiveContactPoints)
        {
            if (collider.bounds.min.x <= point.x && point.x <= collider.bounds.max.x
                && collider.bounds.min.y <= point.y && point.y <= collider.bounds.max.y)
            {
                contactCode = -1;
                break;
            }
        }

        if (contactCode == 0)
        {
            foreach (var collider in _negativeContactPoints)
            {
                if (collider.bounds.min.x <= point.x && point.x <= collider.bounds.max.x
                    && collider.bounds.min.y <= point.y && point.y <= collider.bounds.max.y)
                {
                    contactCode = 1;
                    break;
                }
            }
        }

        if (contactCode == 0)
        {
            Debug.LogWarning($"{name}: Your contact detection code is faulty, contactCode is 0, it should not be 0, please check");
        }

        if (_rotateDirection == -1)
        {
            contactCode = -contactCode;
        }

        return contactCode;
    }
}
