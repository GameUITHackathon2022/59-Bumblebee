using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class UILives : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public void SetLives(int currentLive, int maxLives)
    {
        _text.text = $"{currentLive}/{maxLives}";
    }
}