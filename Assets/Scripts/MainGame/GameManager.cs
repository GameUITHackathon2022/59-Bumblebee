using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<GameObject> _levelObjects;

}