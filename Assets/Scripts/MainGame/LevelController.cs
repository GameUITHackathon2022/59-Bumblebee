using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelConfig _config;
    [SerializeField] private Transform _startTransform;
    [SerializeField] private Transform _goalTransform;
    [SerializeField] private Transform _trashHost;

    public int LevelNumber => _config.LevelNumber;
    public string LevelName => _config.LevelName;

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
    }
}
