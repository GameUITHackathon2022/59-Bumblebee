using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorController : MonoBehaviour
{
    [SerializeField] private RectTransform _indicatorBounds;
    [SerializeField] private Transform _centerTransform;
    [SerializeField] private GameObject _indicatorPrefab;

    private Dictionary<Transform, Indicator> _watchedTransforms = new Dictionary<Transform, Indicator>();

    public float Width => _indicatorBounds.rect.width;
    public float Height => _indicatorBounds.rect.height;

    public void AssignIndicator(Transform transformToIndicate)
    {
        var go = Instantiate(_indicatorPrefab, _indicatorBounds);
        var indicator = go.GetComponent<Indicator>();
        if (transformToIndicate.TryGetComponent<CollectableTrash>(out var trash))
        {
            indicator.SetUp(trash.GetComponent<SpriteRenderer>().sprite, false);
        }
        else if (transformToIndicate.TryGetComponent<SpriteRenderer>(out var rdr))
        {
            indicator.SetUp(rdr.sprite, true);
        }

        indicator.transform.position = transformToIndicate.position;
        _watchedTransforms.Add(transformToIndicate, indicator);
    }

    public void Clean()
    {
        foreach (var indicator in _watchedTransforms.Values)
        {
            Destroy(indicator.gameObject);
        }

        _watchedTransforms.Clear();
    }

    private void Update()
    {
        foreach (var watchedTransform in _watchedTransforms)
        {
            if (watchedTransform.Key == null || !watchedTransform.Key.gameObject.activeSelf)
            {
                watchedTransform.Value.gameObject.SetActive(false);
            }
            else
            {
                watchedTransform.Value.gameObject.SetActive(true);

                var obj = watchedTransform.Key;
                var indicator = watchedTransform.Value;
                // Update indicator location
                var delta = (Vector2)(obj.position - _centerTransform.position);
                var direction = delta.normalized;

                var alpha = Mathf.Rad2Deg * Mathf.Atan(direction.x / direction.y);

                if (delta.magnitude <= 10f)
                {
                    indicator.transform.position = obj.position;
                }
                else
                {
                    indicator.transform.position = _centerTransform.position + (Vector3)direction * 10f;
                }

                var temp = indicator.GetComponent<RectTransform>().localPosition;
                temp.z = 0f;
                indicator.GetComponent<RectTransform>().localPosition = temp;
                indicator.transform.up = direction;
            }
        }
    }
}
