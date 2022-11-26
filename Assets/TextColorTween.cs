using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class TextColorTween : MonoBehaviour
{
    private Tween _tween;
    private Vector3 _center;
    void OnEnable()
    {
        Color color = GetComponent<TMP_Text>().color;
        _tween = DOVirtual.Color(color, new Color(color.r, color.g, color.a, 0), 2f, color_ => { GetComponent<TMP_Text>().color = color_; })
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    void Start()
    {
        
    }

    void Update()
    {

    }

    void OnDisable()
    {
        _tween.Kill();
    }
}
