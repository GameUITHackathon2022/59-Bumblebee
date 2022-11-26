using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Squash : MonoBehaviour
{
    private TMP_Text _text;
    private Tween _tween;

    void OnEnable()
    {
        _tween = transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutBack);
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
