using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField] private VerticalScreenTransitor _screenTransitor;

    public VerticalScreenTransitor Transitor => _screenTransitor;

    private void Start()
    {
        _screenTransitor.TransitOut();
    }

    public void LoadMainMenuScene()
    {
        _screenTransitor.TransitIn(() =>
        {
            var asyncOperation = SceneManager.LoadSceneAsync("MainMenuScene");

            asyncOperation.completed += (operation) =>
            {
                _screenTransitor.TransitOut();
            };
        });
    }

    public void LoadGameScene()
    {
        _screenTransitor.TransitIn(() =>
        {
            var asyncOperation = SceneManager.LoadSceneAsync("GameScene");

            asyncOperation.completed += (operation) =>
            {
                _screenTransitor.TransitOut();
            };
        });
    }
}