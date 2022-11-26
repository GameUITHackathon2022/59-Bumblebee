using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputReceiver : MonoBehaviour
{
    public Action UpEvent;
    public Action DownEvent;
    public Action LeftEvent;
    public Action RightEvent;
    public Action SelectEvent;
    public Action BackEvent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DownEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LeftEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SelectEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            BackEvent?.Invoke();
        }
    }
}
