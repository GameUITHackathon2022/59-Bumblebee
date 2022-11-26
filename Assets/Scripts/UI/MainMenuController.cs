using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private bool _blockInput;

    private void Update()
    {
        if (_blockInput)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _blockInput = true;
            Application.Quit();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        { 
            GameManager.Instance.LoadingScreen.LoadGameScene();
            _blockInput = true;
            return;
        }
    }
}
