using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

public class LevelSelectorItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _number;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private Image _rank;
    [SerializeField] private Image _panel;
    [SerializeField] private Sprite _lockPanelSprite;
    [SerializeField] private Sprite _unselelctedPanelSprite;
    [SerializeField] private Sprite _selectedPanelSprite;
    [SerializeField] private List<Sprite> _rankSprites;

    public bool IsLocked { get; protected set; }

    public void Setup(int number, string name)
    {
        _number.text = number.ToString();

        if (number == 1)
        {
            IsLocked = false;
        }
        else
        {
            IsLocked = PlayerPrefs.GetInt($"Level{number}Locked", 1) == 1 ? true : false;
        }
        
        var rankInt = PlayerPrefs.GetInt($"Level{number}Rank", -1);
        if (IsLocked)
        {
            Lock();
        }
        else if (rankInt != -1)
        {
            var rank = (LevelRank)rankInt;
            var time = PlayerPrefs.GetFloat($"Level{number}Time", 0f);
            _time.text = FormatTime(time);
            _rank.sprite = _rankSprites[rankInt];
            _panel.sprite = _unselelctedPanelSprite;
        }
        else
        {
            _rank.sprite = null;
            _time.text = "------";
            _panel.sprite = _unselelctedPanelSprite;
        }
        
    }

    public void Select()
    {
        _panel.sprite = _selectedPanelSprite;
    }

    public void Deselect()
    {
        _panel.sprite = _unselelctedPanelSprite;
    }

    public void Lock()
    {
        _panel.sprite = _lockPanelSprite;
        _rank.sprite = null;
    }

    private static string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)time - minutes * 60;
        int miniSeconds = (int)((time - System.Math.Truncate(time)) * 100);
        return $"{minutes:00}:{seconds:00}.{miniSeconds:000}";
    }
}
