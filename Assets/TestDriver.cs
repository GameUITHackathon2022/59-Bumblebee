using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class TestDriver : MonoBehaviour
{
    [SerializeField] private LevelController _testLevelObject;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private int _lives = 3;

    private void Start()
    {
        _playerController.PlayerDeadEvent += OnPlayerDead;
        _playerController.PlayerDoneCollectingTrashEvent += OnPlayerDoneCollectingTrash;
        _playerController.PlayerWinEvent += OnPlayerWin;

        StartGame();
    }

    private void OnDestroy()
    {
        _playerController.PlayerDeadEvent -= OnPlayerDead;
        _playerController.PlayerDoneCollectingTrashEvent -= OnPlayerDoneCollectingTrash;
        _playerController.PlayerWinEvent -= OnPlayerWin;
    }

    private void StartGame()
    {
        _playerController.SpawnStick(_testLevelObject.GetStartPosition(), _testLevelObject.StartRotationDirection);
        _testLevelObject.RefreshStage();
        _playerController.Indicators.Clean();
        foreach (var trash in _testLevelObject.AllTrash)
        {
            _playerController.Indicators.AssignIndicator(trash.transform);
        }
        // _playerController.Indicators.AssignIndicator(_testLevelObject.GoalTransform);
        _playerController.StartPlaying(_lives, _testLevelObject.TotalTrashCount, _testLevelObject.RankTimes);
    }

    private void OnPlayerDead()
    {
        DOVirtual.DelayedCall(3.5f, () =>
        {
            _playerController.DespawnStick();
            StartGame();
        });
    }

    private void OnPlayerDoneCollectingTrash()
    {
        _testLevelObject.UnlockGoal();
    }

    private void OnPlayerWin()
    {
        DOVirtual.DelayedCall(3.5f, () =>
        {
            _playerController.DespawnStick();
            StartGame();
        });
    }
}
