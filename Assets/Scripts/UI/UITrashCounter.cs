using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class UITrashCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public void SetTrash(int currentCollected, int totalCount)
    {
        _text.text = $"{currentCollected}/{totalCount}";
    }
}