using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickObject : MonoBehaviour
{
    [System.Serializable]
    public class CornerPointPair
    {
        public Transform PositivePoint;
        public Transform NegativePoint;

        public void GetDistances(Vector2 point, out float posDistance, out float negDistance)
        {
            posDistance = Vector2.Distance(PositivePoint.position, point);
            negDistance = Vector2.Distance(NegativePoint.position, point);
        }
    }
    [Header("Components")]
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private StickInputReceiver _inputReceiver;
    [SerializeField] private Animator _animator;

    [Header("Contact Points")]
    [SerializeField] private CornerPointPair _leftCornerPoint;
    [SerializeField] private CornerPointPair _rightCornerPoint;

    [Header("Parameters - Movement")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _sprintMultiplier = 1.5f;
    [SerializeField] private float _doubleSprintMultiplier = 2f;
    [SerializeField] private float _bounceDistance = 0.5f;

    [Header("Parameters - Spin")]
    [SerializeField] private float _rotateSpeed = 30f;
    [Tooltip("1 means counter-clockwise, -1 is clockwise")]
    [SerializeField] private int _defaultRotation = 1;
    [SerializeField] private float _bounceTime = 0.2f;
    [SerializeField] private float _invincibilityTime = 0.5f;
    [SerializeField] private float _bounceRotation = 35f;
    [SerializeField] private float _touchGoalSpeedIncrease = 5f;

    private int _rotateDirection;
    private float _bounceTimer;
    private int _bounceContactCode;
    private float _invincibilityTimer;
    private Vector2 _influenceVector;

    private float _rotationSpeedClone;
    private bool _hasTouchedGoal;

    public bool IsStunned => _bounceTimer > 0;
    public bool IsInvincible => _invincibilityTimer > 0;

    private void Start()
    {
        _rotateDirection = _defaultRotation;
        _rotationSpeedClone = _rotateSpeed;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Wall"), LayerMask.NameToLayer("Player"), false);
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

    public void AddBounce(float bounceAngle, float bounceDistance)
    {

    }

    private void ControlPosition(StickInputReceiver.InputState inputState)
    {
        var movementVector = Vector2.zero;

        if (IsStunned)
        {
            movementVector = Vector2.zero;
            return;
        }
        else
        {
            if (_hasTouchedGoal)
            {
                movementVector = Vector2.zero;
            }
            else
            {
                var speedMod = 1f;
                if (inputState.SprintModifierMode == 1)
                {
                    speedMod = _sprintMultiplier;
                }
                else if (inputState.SprintModifierMode == 2)
                {
                    speedMod = _doubleSprintMultiplier;
                }

                movementVector = inputState.MovementVector * _speed * speedMod * Time.fixedDeltaTime;
            }
        }

        // Add influence
        movementVector += _influenceVector;
        _influenceVector = Vector2.zero;

        _rigidbody.velocity = movementVector;
    }

    private void ControlRotation()
    {
        if (_hasTouchedGoal)
        {
            _rotationSpeedClone += _touchGoalSpeedIncrease * Time.fixedDeltaTime;
        }

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
            DoPositionBounce(collision.GetContact(0).normal);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Goal"))
        {
            _hasTouchedGoal = true;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Wall"), LayerMask.NameToLayer("Player"), true);
            Debug.Log("Touched goal, please end level.");
        }
        else if (collider.CompareTag("Puller"))
        {
            var puller = collider.gameObject.GetComponentInChildren<StickPuller>();
            if (puller != null)
            {
                var pullVect = puller.GetPullVector(transform.position);
                _influenceVector = pullVect;
            }
        }
    }

    private int GetContactRotationOnColliding(Vector2 contactPoint)
    {
        int contactCode = 0;

        CornerPointPair closestPair;

        _leftCornerPoint.GetDistances(contactPoint, out float leftPosDist, out float leftNegDist);
        _rightCornerPoint.GetDistances(contactPoint, out float rightPosDist, out float rightNegDist);

        var minLeftDist = Mathf.Min(leftPosDist, leftNegDist);
        var minRightDist = Mathf.Min(rightPosDist, rightNegDist);

        if (minLeftDist < minRightDist)
        {
            closestPair = _leftCornerPoint;
        }
        else
        {
            closestPair = _rightCornerPoint;
        }

        closestPair.GetDistances(contactPoint, out float posDist, out float negDist);

        if (posDist < negDist)
        {
            contactCode = -1;
        }
        else
        {
            contactCode = 1;
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

    private void DoPositionBounce(Vector2 normal)
    {
        _rigidbody.MovePosition((Vector2)transform.position + normal * _bounceDistance);
    }
}
