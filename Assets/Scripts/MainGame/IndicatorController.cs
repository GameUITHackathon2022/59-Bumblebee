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
                var delta = (Vector2)(obj.position - transform.position);
                var direction = delta.normalized;
                var screenPos = Camera.main.WorldToScreenPoint(obj.position);

                var theta = Mathf.Rad2Deg * Mathf.Atan(Width / Height);
                var alpha = Mathf.Rad2Deg * Mathf.Atan(direction.x / direction.y);

                var hit = Physics2D.Raycast(_centerTransform.position, direction, delta.magnitude, LayerMask.NameToLayer("UI"));
                if (hit.collider == null)
                {
                    indicator.transform.position = hit.point;
                }
                else
                {
                    indicator.transform.position = obj.position;
                }
                indicator.transform.up = direction;
            }
        }
    }
}
