using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

public class VerticalScreenTransitor : MonoBehaviour
{
	[Header("Coponents")]
	[SerializeField] private RectTransform _imageTransform;

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
		_imageTransform.anchorMax = new Vector2(1f, 1f);
	}
	public void SetTransitOut()
	{
		_imageTransform.anchorMax = new Vector2(1f, 0f);
	}


	public void TransitIn(Action onCompleteAction = null)
	{
		_imageTransform.DOAnchorMax(new Vector2(1f, 1f), _time)
			.SetEase(_ease)
			.OnComplete(() =>
			{
				_transitInEvent?.Invoke();
				onCompleteAction?.Invoke();
			});
	}

	public void TransitOut(Action onCompleteAction = null)
	{
		_imageTransform.DOAnchorMax(new Vector2(1f, 0f), _time)
			.SetEase(_ease)
			.OnComplete(() =>
			{
				_transitOutEvent?.Invoke();
				onCompleteAction?.Invoke();
			});
	}
}