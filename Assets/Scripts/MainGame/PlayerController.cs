using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public class LevelEndStatistics
    {
        public int TrashCollected;
        public int TotalTrash;
        public float TimePlayed;
        public LevelRank Rank;
    }

    [Header("Components")]
    [SerializeField] private GameObject _stickObjectPrefab;
    [SerializeField] private CinemachineVirtualCamera _camera;

    [Header("UIs")]
    [SerializeField] private UITimer _uiTimer;
    [SerializeField] private UILives _uiLives;
    [SerializeField] private UITrashCounter _uiTrashCounter;
    [SerializeField] private IndicatorController _indicatorController;

    private StickObject _stickObject;

    private int _maxLiveCount;
    private int _currentLiveCount;

    private int _trashCollectedCount;
    private int _totalTrashCount;

    private float _playTime;
    private Dictionary<LevelRank, float> _rankTimes;
    private LevelRank _currentRank;

    private bool _isInPlay;
    private bool _collectedAllTrash;
    private bool _touchedGoal;

    public int MaxLiveCount
    {
        get
        {
            return _maxLiveCount;
        }
        protected set
        {
            _maxLiveCount = value;
            _uiLives.SetLives(CurrentLiveCount, MaxLiveCount);
        }
    }

    public int CurrentLiveCount
    {
        get
        {
            return _currentLiveCount;
        }
        protected set
        {
            _currentLiveCount = value;
            _uiLives.SetLives(CurrentLiveCount, MaxLiveCount);
            if (CurrentLiveCount <= 0)
            {
                OnPlayerDead();
            }
        }
    }

    public int CollectedTrashCount
    {
        get
        {
            return _trashCollectedCount;
        }
        protected set
        {
            _trashCollectedCount = value;
            _uiTrashCounter.SetTrash(CollectedTrashCount, TotalTrashCount);
        }
    }

    public int TotalTrashCount
    {
        get
        {
            return _totalTrashCount;
        }
        protected set
        {
            _totalTrashCount = value;
            _uiTrashCounter.SetTrash(CollectedTrashCount, TotalTrashCount);
        }
    }

    public IndicatorController Indicators => _indicatorController;

    public bool IsDead => _currentLiveCount <= 0;
    public bool LevelConcluded => IsDead || _touchedGoal;

    public event Action PlayerDeadEvent;
    public event Action PlayerDoneCollectingTrashEvent;
    public event Action PlayerWinEvent;


    private void Start()
    {
        
    }

    private void Update()
    {
        if (_isInPlay)
        {
            _playTime += Time.deltaTime;
            _uiTimer.SetCurrentTime(_playTime);
            CompareAndUpdateRankTime();
        }
    }

    public StickObject SpawnStick(Vector2 location, int rotation=1)
    {
        var go = Instantiate(_stickObjectPrefab, (Vector3)location + new Vector3(0f, 0f, -10f), Quaternion.identity);
        _stickObject = go.GetComponent<StickObject>();
        _stickObject.Input.LockControl();
        _stickObject.ForceSetRotationDirection(rotation);

        _camera.LookAt = _stickObject.transform;
        _camera.Follow = _stickObject.transform;

        _stickObject.ReachedEndGoalEvent += OnReachEndGoal;
        _stickObject.TrashCollectedEvent += OnCollectedTrash;
        _stickObject.LoseLiveEvent += OnCollidedWithWall;
        _stickObject.HealLivesEvent += OnHeal;

        return _stickObject;
    }

    public void StartPlaying(int lives, int trashCount, IDictionary<LevelRank, float> rankTimes)
    {
        _stickObject.Input.UnlockControl();
        MaxLiveCount = lives;
        CurrentLiveCount = lives;
        TotalTrashCount = trashCount;
        CollectedTrashCount = 0;
        _rankTimes = new Dictionary<LevelRank, float>(rankTimes);

        _playTime = 0f;
        _currentRank = LevelRank.S_RANK;

        _isInPlay = true;
    }

    public void DespawnStick()
    {
        if (_stickObject != null)
        {
            _stickObject.ReachedEndGoalEvent -= OnReachEndGoal;
            _stickObject.TrashCollectedEvent -= OnCollectedTrash;
            _stickObject.LoseLiveEvent -= OnCollidedWithWall;
            _stickObject.HealLivesEvent -= OnHeal;

            Destroy(_stickObject.gameObject);
            _stickObject = null;
        }
    }

    public LevelEndStatistics GetLevelEndStatistics()
    {
        if (LevelConcluded)
        {
            return new LevelEndStatistics
            {
                TimePlayed = _playTime,
                Rank = _currentRank,
                TrashCollected = CollectedTrashCount,
                TotalTrash = TotalTrashCount
            };
        }
        else
        {
            Debug.LogError($"{name}: Level has not concluded, cannot get level end statistics.");
            return new LevelEndStatistics();
        }
    }

    private void CompareAndUpdateRankTime()
    {
        if (_rankTimes == null)
        {
            return;
        }
        else
        {
            if (_rankTimes[_currentRank] < _playTime)
            {
                if (_currentRank != LevelRank.C_RANK)
                {
                    _currentRank = (LevelRank)((int)_currentRank + 1);
                }

                _uiTimer.SetRankTimes(_currentRank, _rankTimes[_currentRank]);
            }
        }
    }

    private void OnPlayerDead()
    {
        _stickObject.SetSink();
        PlayerDeadEvent?.Invoke();
    }

    private void OnReachEndGoal()
    {
        _touchedGoal = true;
        _isInPlay = false;
        PlayerWinEvent?.Invoke();
    }

    private void OnCollectedTrash(int count)
    {
        CollectedTrashCount += count;
        if (CollectedTrashCount >= TotalTrashCount)
        {
            _stickObject.AllowToTouchGoal();
            PlayerDoneCollectingTrashEvent?.Invoke();
        }
    }

    private void OnCollidedWithWall()
    {
        CurrentLiveCount--;
    }

    private void OnHeal()
    {
        CurrentLiveCount = MaxLiveCount;
    }
}
