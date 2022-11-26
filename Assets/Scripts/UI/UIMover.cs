using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIMover : MonoBehaviour
{
    public enum FadeInDirection
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    [SerializeField] private bool _movedInOnStart;
    [SerializeField] private bool _playedAnimOnStart;
    [SerializeField] private FadeInDirection _direction = FadeInDirection.LEFT;
    [SerializeField] private float _time = 0.5f;
    [SerializeField] private Ease _ease = Ease.Linear;
    [SerializeField] private UnityEvent _moveInEvent;
    [SerializeField] private UnityEvent _moveOutEvent;

    private RectTransform rect => GetComponent<RectTransform>();
    private Vector2 _movedOutPosition;
    private Vector2 _movedInPosition;

    private bool _isMovedIn;

    public bool IsMovedIn => _isMovedIn;
    public bool IsMovedOut => !_isMovedIn;

    private void Awake()
    {

    }

    private void Start()
    {
        _movedOutPosition = GetMovedOutPosition();
        _movedInPosition = rect.anchoredPosition;

        if (!_movedInOnStart)
        {
            rect.anchoredPosition = _movedOutPosition;
        }

        if (_playedAnimOnStart)
        {
            if (_movedInOnStart)
            {
                MoveOut();
            }
            else
            {
                MoveIn();
            }
        }
    }

    public void MoveInWithoutCallback()
    {
        MoveIn(null);
    }

    public void MoveOutWithoutCallback()
    {
        MoveOut(null);
    }

    public void MoveIn(Action onCompleteAction = null)
    {
        rect.DOAnchorPos(_movedInPosition, _time)
            .SetEase(_ease)
            .OnComplete(() =>
            {
                _isMovedIn = true;
                _moveInEvent.Invoke();
                onCompleteAction?.Invoke();
            });
    }

    public void MoveOut(Action onCompleteAction = null)
    {
        rect.DOAnchorPos(_movedOutPosition, _time)
            .SetEase(_ease)
            .OnComplete(() =>
            {
                _isMovedIn = false;
                _moveOutEvent.Invoke();
                onCompleteAction?.Invoke();
            });
    }

    private Vector2 GetMovedOutPosition()
    {
        switch (_direction)
        {
            case FadeInDirection.LEFT:
                return rect.anchoredPosition + new Vector2(rect.rect.width, 0f);
            case FadeInDirection.RIGHT:
                return rect.anchoredPosition + new Vector2(-rect.rect.width, 0f);
            case FadeInDirection.UP:
                return rect.anchoredPosition + new Vector2(0f, -rect.rect.height);
            case FadeInDirection.DOWN:
                return rect.anchoredPosition + new Vector2(0f, rect.rect.height);
            default:
                return Vector2.zero;
        }
    }
}
