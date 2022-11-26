using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;


public class HorizontalScreenTransitor : MonoBehaviour
{
	[Header("Coponents")]
	[SerializeField] private RectTransform _imageTransformLeft;
	[SerializeField] private RectTransform _imageTransformRight;

	[Header("Parameters")]
	[SerializeField] private float _time = 0.75f;
	[SerializeField] private Ease _ease = Ease.Linear;
	[SerializeField] private UnityEvent _transitInEvent;
	[SerializeField] private UnityEvent _transitOutEvent;

	private void Start()
	{

	}

	public void SetTransitIn()
	{
		_imageTransformLeft.anchorMax = new Vector2(1f, 1f);
		_imageTransformRight.anchorMin = new Vector2(0f, 0f);
	}
	public void SetTransitOut()
	{
		_imageTransformLeft.anchorMax = new Vector2(0f, 1f);
		_imageTransformRight.anchorMin = new Vector2(1f, 0f);
	}


	public void TransitIn(Action onCompleteAction = null)
	{
		var sequence = DOTween.Sequence();

		sequence.Insert(0f, _imageTransformLeft.DOAnchorMax(new Vector2(1f, 1f), _time).SetEase(_ease));
		sequence.Insert(0f, _imageTransformRight.DOAnchorMin(new Vector2(0f, 0f), _time).SetEase(_ease));

		sequence.OnComplete(() =>
		{
			_transitInEvent?.Invoke();
			onCompleteAction?.Invoke();
		});
	}

	public void TransitOut(Action onCompleteAction = null)
	{
		var sequence = DOTween.Sequence();

		sequence.Insert(0f, _imageTransformLeft.DOAnchorMax(new Vector2(0f, 1f), _time).SetEase(_ease));
		sequence.Insert(0f, _imageTransformRight.DOAnchorMin(new Vector2(1f, 0f), _time).SetEase(_ease));

		sequence.OnComplete(() =>
		{
			_transitOutEvent?.Invoke();
			onCompleteAction?.Invoke();
		});
	}
}

