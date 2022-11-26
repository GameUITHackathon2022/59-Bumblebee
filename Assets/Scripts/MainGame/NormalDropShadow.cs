using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NormalDropShadow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _source;
    [SerializeField] private Color _color;
    [SerializeField] private Vector3 _offset;

    private void Start()
    {
        var go = new GameObject("Shadow");
        go.transform.SetParent(_source.transform);
        go.transform.localPosition = _offset;
        go.AddComponent<SpriteRenderer>();
        go.GetComponent<SpriteRenderer>().color = _color;
        go.GetComponent<SpriteRenderer>().sprite = _source.sprite;
    }
}
