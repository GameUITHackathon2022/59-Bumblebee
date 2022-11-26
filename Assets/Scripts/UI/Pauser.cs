using DG.Tweening;
using System;
using UnityEngine;

public class Pauser : MonoBehaviour
{
	[SerializeField] private GameObject _overlay;

    private bool _isPaused;
    private bool _blocked;

    private bool IsPaused
    {
        get { return _isPaused; }
        set
        {
            Time.timeScale = value ? 0 : 1;
            _overlay.SetActive(value);
            _isPaused = value;
        }
    }

    private void Start()
    {
        SetOff();
    }

    public void SetOn()
    {
        _blocked = false;
    }

    public void SetOff()
    {
        IsPaused = false;
        _blocked = true;
    }

    private void Update()
    {
        if (_blocked)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
        }
    }
}