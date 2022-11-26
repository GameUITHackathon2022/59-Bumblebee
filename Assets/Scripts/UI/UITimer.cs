using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UITimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentTimerText;
    [SerializeField] private Image _rankImage;
    [SerializeField] private TMP_Text _timeForRankText;
    [SerializeField] private List<Sprite> _rankSprites;


    public void SetCurrentTime(float time)
    {
        _currentTimerText.text = FormatTime(time);
    }

    public void SetRankTimes(LevelRank rank, float rankTime)
    {
        _timeForRankText.text = FormatTime(rankTime);
        _rankImage.sprite = _rankSprites[(int)rank];
    }

    private string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)time - minutes * 60;
        int miniSeconds = (int)((time - System.Math.Truncate(time)) * 100);
        return $"{minutes:00}:{seconds:00}.{miniSeconds:000}";
    }
}
