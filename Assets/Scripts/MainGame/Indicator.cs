using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private Image _arrow;
    [SerializeField] private Sprite _smallArrow;
    [SerializeField] private Sprite _bigArrow;

    public void SetUp(Sprite itemSprite, bool bigArrow=false)
    {
        _itemSprite.sprite = itemSprite;
        _arrow.sprite = bigArrow ? _bigArrow : _smallArrow;
    }

    private void Start()
    {
        transform.DOShakeScale(1.5f, 1.5f, 10, 0, false, ShakeRandomnessMode.Harmonic)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
