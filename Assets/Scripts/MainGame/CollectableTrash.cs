using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableTrash : MonoBehaviour
{
    public void Respawn()
    {
        gameObject.SetActive(true);
    }

    public void OnCollect()
    {
        gameObject.SetActive(false);
    }
}
