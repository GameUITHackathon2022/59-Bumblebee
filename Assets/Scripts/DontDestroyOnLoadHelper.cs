using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadHelper : MonoBehaviour
{   
    private void Awake()
    {
		DontDestroyOnLoad(this);
	}
}
