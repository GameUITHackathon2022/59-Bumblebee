using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    [SerializeField] private Transform _spriteHost;
    [SerializeField] private Image _arrow;
    [SerializeField] private Sprite _smallArrow;
    [SerializeField] private Sprite _bigArrow;

    private Tween _animTween;

    public void SetUp(bool bigArrow=false)
    {
        _arrow.sprite = bigArrow ? _bigArrow : _smallArrow;
    }

    private void Start()
    {
        _animTween = _spriteHost.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.75f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    private void OnDestroy()
    {
        _animTween.Kill();
    }
}
