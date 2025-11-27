using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DiceFlyAnim : MonoBehaviour
{
    public Vector3 target;
    public IEnumerator DiceFly(GameObject diceObj)
    {
        yield return transform.DOLocalMove(target, 0.5f).OnComplete(() =>
        {
            diceObj.SetActive(true);
            Destroy(gameObject);
        }).WaitForCompletion();
    }
}
