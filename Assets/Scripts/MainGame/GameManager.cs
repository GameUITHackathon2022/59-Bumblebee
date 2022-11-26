using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private List<AssetReference> _levelObjects;
    [SerializeField] private List<string> _levelNames;

    public List<AssetReference> Levels => _levelObjects;
    public List<string> LevelNames => _levelNames;
    public LoadingScreen LoadingScreen => _loadingScreen;
}