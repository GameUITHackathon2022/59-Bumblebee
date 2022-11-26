using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using static Cinemachine.DocumentationSortingAttribute;

public class SessionDriver : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private LevelSelector _levelSelector;
    [SerializeField] private int _lives = 3;

    private LevelController _currentLevel;

    public void StartSession()
    {
        _playerController.PlayerDeadEvent += OnPlayerDead;
        _playerController.PlayerDoneCollectingTrashEvent += OnPlayerDoneCollectingTrash;
        _playerController.PlayerWinEvent += OnPlayerWin;
    }

    public void EndSession()
    {
        _playerController.PlayerDeadEvent -= OnPlayerDead;
        _playerController.PlayerDoneCollectingTrashEvent -= OnPlayerDoneCollectingTrash;
        _playerController.PlayerWinEvent -= OnPlayerWin;

        _currentLevel = null;
    }

    public void StartLevel(LevelController level)
    {
        _currentLevel = level;

        _playerController.SpawnStick(_currentLevel.GetStartPosition(), _currentLevel.StartRotationDirection);
        _currentLevel.RefreshStage();
        _playerController.Indicators.Clean();
        foreach (var trash in _currentLevel.AllTrash)
        {
            _playerController.Indicators.AssignIndicator(trash.transform);
        }
        
        _playerController.StartPlaying(_lives, _currentLevel.TotalTrashCount, _currentLevel.RankTimes);
    }

    public void EndLevel()
    {
        DOVirtual.DelayedCall(3.5f, () =>
        {
            _playerController.DespawnStick();

            var stat = _playerController.GetLevelEndStatistics();
            var number = _currentLevel.LevelNumber;
            var rankInt = stat.PlayerWon ? (int)stat.Rank : -1;

            var oldTime = PlayerPrefs.GetFloat($"Level{number}Time", 0f);

            PlayerPrefs.SetInt($"Level{number}Rank", rankInt);
            PlayerPrefs.SetInt($"Level{number + 1}Locked", stat.PlayerWon ? 0 : 1);
            if (stat.TimePlayed < oldTime)
            {
                PlayerPrefs.SetFloat($"Level{number}Time", stat.TimePlayed);
            }
            PlayerPrefs.SetInt($"CurrentLevel", number + 1);

            GameManager.Instance.LoadingScreen.Transitor.TransitIn(() =>
            {
                _levelSelector.EndScreen.Setup(_currentLevel.LevelNumber, stat);
                _levelSelector.EndScreen.Show();

                Destroy(_currentLevel.gameObject);
                _currentLevel = null;

                GameManager.Instance.LoadingScreen.Transitor.TransitOut();
            });
        });
    }

    private void OnPlayerDead()
    {
        EndLevel();
    }

    private void OnPlayerDoneCollectingTrash()
    {
        _currentLevel.UnlockGoal();
        _playerController.Indicators.AssignIndicator(_currentLevel.GoalTransform);
    }

    private void OnPlayerWin()
    {
        EndLevel();
    }
}
