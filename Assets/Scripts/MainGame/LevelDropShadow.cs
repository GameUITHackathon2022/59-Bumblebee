using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelDropShadow : MonoBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private float _zOffset = 2;
    [SerializeField] private float _baseOffset = 0.3f;

    private void Start()
    {
        var go = Instantiate(gameObject, transform);
        Destroy(go.GetComponent<CompositeCollider2D>());
        Destroy(go.GetComponent<TilemapCollider2D>());
        Destroy(go.GetComponent<Rigidbody2D>());
        Destroy(go.GetComponent<LevelDropShadow>());
        go.GetComponent<Tilemap>().color = _color;

        go.transform.localPosition = new Vector3(0f, -_baseOffset, _zOffset);

        go = Instantiate(go, transform);
        go.transform.localPosition = new Vector3(0f, _baseOffset, _zOffset);

        go = Instantiate(go, transform);
        go.transform.localPosition = new Vector3(-_baseOffset, 0f, _zOffset);

        go = Instantiate(go, transform);
        go.transform.localPosition = new Vector3(_baseOffset, 0f, _zOffset);
    }
}
