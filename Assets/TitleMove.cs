using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleMove : MonoBehaviour
{
    private Tween _tween;
    private Vector3 _center;
    void OnEnable()
    {
        _center = transform.position;
        _tween = GetComponent<RectTransform>().DOShakePosition(2, 5f, 0, 50, false, false, ShakeRandomnessMode.Harmonic)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutBounce);
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
