using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using JSAM;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField] private VerticalScreenTransitor _screenTransitor;

    public VerticalScreenTransitor Transitor => _screenTransitor;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            AudioManager.PlayMusic(Music.Intro);
        } else if (scene.name == "LevelSelect")
        {
            AudioManager.PlayMusic(Music.LevelSelect);
        } else if (scene.name == "GameScene")
        {
            AudioManager.PlayMusic(Music.Game);
        }
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

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
                AudioManager.PlayMusic(Music.Game);
                Debug.Log("AUDIO MANAGER PLAY MUSIC!");
            };
        });
    }
}