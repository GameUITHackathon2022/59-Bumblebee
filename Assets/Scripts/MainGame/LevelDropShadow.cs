using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelDropShadow : MonoBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private Vector3 _offset;

    private void Start()
    {
        var go = Instantiate(gameObject, transform);
        Destroy(go.GetComponent<CompositeCollider2D>());
        Destroy(go.GetComponent<TilemapCollider2D>());
        Destroy(go.GetComponent<Rigidbody2D>());
        Destroy(go.GetComponent<LevelDropShadow>());
        go.GetComponent<Tilemap>().color = _color;
        go.transform.localPosition += _offset;
    }
}
