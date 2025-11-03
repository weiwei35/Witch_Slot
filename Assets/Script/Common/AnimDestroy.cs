using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimDestroy : MonoBehaviour
{
    public void HideObj()
    {
        gameObject.SetActive(false);
    }

    public void DestroyObj()
    {
        Destroy(gameObject);
    }
}
