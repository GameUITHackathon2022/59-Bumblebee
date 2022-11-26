using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

public class EndScreen : MonoBehaviour
{
	[SerializeField] private LevelSelector _levelSelector;
	[SerializeField] private TMP_Text _statTitles;
	[SerializeField] private TMP_Text _statValues;
	[SerializeField] private Image _winLossImage;
	[SerializeField] private Sprite _winSprite;
	[SerializeField] private Sprite _lossSprite;
	[SerializeField] private Image _rankImage;
    [SerializeField] private List<Sprite> _rankSprites;

    private void Start()
    {
        Hide();
    }

    public void Setup(int level, PlayerController.LevelEndStatistics stat)
    {
        var won = stat.PlayerWon;
        if (won)
        {
            _winLossImage.sprite = _winSprite;
        }
        else
        {
            _winLossImage.sprite = _lossSprite;
        }

        _statTitles.text = $"Time:\nBest Time:\nCollected:";
        _statValues.text = $"{FormatTime(stat.TimePlayed)}\n{FormatTime(PlayerPrefs.GetFloat($"Level{level}Time", 0))}\n{stat.TrashCollected}/{stat.TotalTrash}";

        if (won)
        {
            _rankImage.sprite = _rankSprites[(int)stat.Rank];
            _rankImage.color = Color.white;
        }
        else
        {
            _rankImage.sprite = null;
            _rankImage.color = Color.clear;
        }

    }

	public void Show()
	{
        gameObject.SetActive(true);
        _blockInput = false;
	}

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private bool _blockInput;
    private void Update()
    {
        if (_blockInput)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameManager.Instance.LoadingScreen.Transitor.TransitIn(() =>
            {
                _levelSelector.LoadNextLevel();
                GameManager.Instance.LoadingScreen.Transitor.TransitOut();
            });
            _blockInput = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            GameManager.Instance.LoadingScreen.Transitor.TransitIn(() =>
            {
                _levelSelector.Show();
                GameManager.Instance.LoadingScreen.Transitor.TransitOut();
            });
            _blockInput = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            GameManager.Instance.LoadingScreen.Transitor.TransitIn(() =>
            {
                _levelSelector.ReplayLevel();
                GameManager.Instance.LoadingScreen.Transitor.TransitOut();
            });
            _blockInput = true;
            return;
        }
    }

    private static string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)time - minutes * 60;
        int miniSeconds = (int)((time - System.Math.Truncate(time)) * 100);
        return $"{minutes:00}:{seconds:00}.{miniSeconds:000}";
    }
}