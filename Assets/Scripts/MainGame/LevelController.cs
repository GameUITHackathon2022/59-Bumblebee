using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelConfig _config;
    [SerializeField] private Transform _startTransform;
    [SerializeField] private Transform _goalTransform;
    [SerializeField] private Transform _trashHost;
    [SerializeField] private Sprite _lockedGoalSprite;
    [SerializeField] private Sprite _unlockedGoalSprite;

    public int LevelNumber => _config.LevelNumber;
    public string LevelName => _config.LevelName;
    public int StartRotationDirection => _config.StartRotateDirection;

    private List<CollectableTrash> _allTrashs;

    public IDictionary<LevelRank, float> RankTimes => _config.RankTimes;

    public int TotalTrashCount => _allTrashs.Count;

    private void Awake()
    {
        _allTrashs = new List<CollectableTrash>();
        foreach (var trash in _trashHost.GetComponentsInChildren<CollectableTrash>())
        {
            _allTrashs.Add(trash);
        }
    }

    public Vector2 GetStartPosition()
    {
        return _startTransform.position;
    }

    public Vector2 GetEndPosition()
    {
        return _goalTransform.position;
    }

    public void RefreshStage()
    {
        foreach (var trash in _allTrashs)
        {
            trash.Respawn();
        }
        LockGoal();
    }

    public void UnlockGoal()
    {
        _goalTransform.GetComponentInChildren<SpriteRenderer>().sprite = _unlockedGoalSprite;
        _goalTransform.GetComponentInChildren<Rotator>().enabled = true;
        _goalTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _goalTransform.GetComponentInChildren<BoxCollider2D>().enabled = true;
                _goalTransform.GetComponentInChildren<CircleCollider2D>().enabled = true;
                
            });
    }

    public void LockGoal()
    {
        _goalTransform.GetComponentInChildren<BoxCollider2D>().enabled = false;
        _goalTransform.GetComponentInChildren<CircleCollider2D>().enabled = false;
        _goalTransform.GetComponentInChildren<Rotator>().enabled = false;
        _goalTransform.GetComponentInChildren<SpriteRenderer>().sprite = _lockedGoalSprite;

        _goalTransform.localScale = new Vector3(0.5f, 0.5f, 0f);
    }
}
