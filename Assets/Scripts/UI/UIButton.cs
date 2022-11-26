using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Events;

public class UIButton : MonoBehaviour
{
    [SerializeField] private UnityEvent _event;

    public void Press()
    {
        _event.Invoke();
    }
}
