using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using DG.Tweening;
using System.Linq;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private List<LevelSelectorItem> _items;
    [SerializeField] private SessionDriver _sessionDriver;
    [SerializeField] private EndScreen _endScreen;

    private int _currentlySelectedLevel;
    private bool _blockInput;

    public EndScreen EndScreen => _endScreen;

    public int CurrentSelectedLevel
    {
        get { return _currentlySelectedLevel; }
        set
        {
            value = Mathf.Clamp(value, 1, _items.Count);
            while (value > 1 && _items[value - 1].IsLocked)
            {
                --value;
            }

            if (_currentlySelectedLevel != value)
            {
                for (int i = 1; i <= _items.Count; i++)
                {
                    if (i == value)
                    {
                        _items[i - 1].Select();
                    }
                    else if (!_items[i - 1].IsLocked)
                    {
                        _items[i - 1].Deselect();
                    }
                }
            }

            _currentlySelectedLevel = value;
            PlayerPrefs.SetInt("CurrentLevel", _currentlySelectedLevel);
            Debug.Log(_currentlySelectedLevel);
        }
    }

    private void Start()
    {
        _currentlySelectedLevel = 0;
        _sessionDriver.StartSession();
        Show();
    }

    private void OnDestroy()
    {
        _sessionDriver.EndSession();
    }

    private void Update()
    {
        if (_blockInput)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _blockInput = true;
            GameManager.Instance.LoadingScreen.LoadMainMenuScene();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            LoadLevel(CurrentSelectedLevel);
            _blockInput = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CurrentSelectedLevel--;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CurrentSelectedLevel++;
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var name = GameManager.Instance.LevelNames[i];
            _items[i].Setup(i + 1, name);
        }

        CurrentSelectedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        _blockInput = false;
    }

    public void Show()
    {
        Refresh();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void LoadNextLevel()
    {
        Refresh();
        if (CurrentSelectedLevel != _items.Count)
        {
            ++CurrentSelectedLevel;
            LoadLevel(CurrentSelectedLevel);
        }
    }

    public void ReplayLevel()
    {
        Refresh();
        LoadLevel(CurrentSelectedLevel);
    }

    private void LoadLevel(int number)
    {
        var levelAsset = GameManager.Instance.Levels[number - 1];

        GameManager.Instance.LoadingScreen.Transitor.TransitIn(() =>
        {
            Hide();
            _endScreen.Hide();
            var operation = Addressables.InstantiateAsync(levelAsset);
            operation.Completed += (handle) =>
            {
                _sessionDriver.StartLevel(handle.Result.GetComponent<LevelController>());
               DOVirtual.DelayedCall(0.5f, () => GameManager.Instance.LoadingScreen.Transitor.TransitOut());
            };
        });
    }
}
