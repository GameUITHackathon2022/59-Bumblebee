using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<LevelConfig> _allLevels;

    public LevelConfig GetLevel(int levelNumber)
    {
        return _allLevels.FirstOrDefault(x => x.LevelNumber == levelNumber);
    }

    public LevelConfig GetLevel(string levelName)
    {
        return _allLevels.FirstOrDefault(x => x.LevelName == levelName);
    }
}