using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UILives : MonoBehaviour
{
    [SerializeField] private List<Image> _liveImages;

    public void SetLives(int currentLive, int maxLives)
    {
        int i = 0;

        foreach (Image image in _liveImages)
        {
            image.gameObject.SetActive(i < currentLive);
            ++i;
        }
    }
}