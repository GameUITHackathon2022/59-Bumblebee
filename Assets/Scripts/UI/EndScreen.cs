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

    public void Setup(int level, PlayerController.LevelEndStatistics statistics)
	{

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
}