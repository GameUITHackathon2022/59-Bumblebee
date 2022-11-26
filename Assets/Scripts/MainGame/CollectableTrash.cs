using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableTrash : MonoBehaviour
{
    [SerializeField] private List<RuntimeAnimatorController> _controllers;
    [SerializeField] private Animator _animator;

    public void Respawn()
    {
        var rand = UnityEngine.Random.Range(0, _controllers.Count);
        _animator.runtimeAnimatorController = _controllers[rand];
        _animator.Rebind();
        gameObject.SetActive(true);
    }

    public void OnCollect()
    {
        gameObject.SetActive(false);
    }
}
