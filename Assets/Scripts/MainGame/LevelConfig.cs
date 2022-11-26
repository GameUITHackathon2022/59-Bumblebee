using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

public enum LevelRank
{
    S_RANK = 0,
    A_RANK = 1,
    B_RANK = 2,
    C_RANK = 3,
}

[Serializable]
public class RankToTimeDictionary : SerializableDictionaryBase<LevelRank, float> { }

[Serializable]
public class LevelConfig
{
    [SerializeField] public int LevelNumber;
    [SerializeField] public string LevelName;
    [SerializeField] public RankToTimeDictionary RankTimes;
    [SerializeField] public int StartRotateDirection = 1;
}
