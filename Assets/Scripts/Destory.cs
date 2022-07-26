using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Destory : MonoBehaviour
{
    public float t = 2f;
    void Start()
    {
        Invoke("DestoryFunc",t);
        
    }
    void DestoryFunc()
    {
        //if (!Addressables.ReleaseInstance(gameObject))
            Destroy(gameObject);
    }
}
