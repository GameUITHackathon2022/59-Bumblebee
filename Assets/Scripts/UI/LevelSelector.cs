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

    public int CurrentSelectedLevel
    {
        get { return _currentlySelectedLevel; }
        set
        {
            int lastUnlockedLevel = Mathf.Clamp(value, 1, _items.Count);
            while (_items[lastUnlockedLevel - 1].IsLocked || lastUnlockedLevel == 1)
            {
                --lastUnlockedLevel;
            }

            if (_currentlySelectedLevel != lastUnlockedLevel)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (i == _currentlySelectedLevel - 1)
                    {
                        _items[i].Select();
                    }
                    else
                    {
                        _items[i].Deselect();
                    }
                }
            }

            _currentlySelectedLevel = value;
        }
    }

    private void Start()
    {
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

    public void Show()
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            var name = GameManager.Instance.LevelNames[i];
            _items[i].Setup(i + 1, name);
        }

        CurrentSelectedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        _blockInput = false;
    }

    public void LoadNextLevel()
    {
        
    }

    public void ReplayLevel()
    {

    }

    private void LoadLevel(int number)
    {
        var levelAsset = GameManager.Instance.Levels[number];

        GameManager.Instance.LoadingScreen.Transitor.TransitIn(() =>
        {
            gameObject.SetActive(false);
            var operation = Addressables.InstantiateAsync(levelAsset);
            operation.Completed += (handle) =>
            {
                GameManager.Instance.LoadingScreen.Transitor.TransitOut();
            };
        });
    }
}
