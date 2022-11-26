using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorController : MonoBehaviour
{
    [SerializeField] private RectTransform _indicatorBounds;

    private Dictionary<Transform, Indicator> _watchedTransforms = new Dictionary<Transform, Indicator>();

    public void AssignIndicator(Transform transformToIndicate)
    {

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

            }
        }
    }
}
