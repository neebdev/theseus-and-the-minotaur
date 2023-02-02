using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Exit : MonoBehaviour
{
    public GameObject square1;
    public GameObject square2;
    public float inDuration;
    public float outDuration;

    private void Start()
    {
        square1.transform.DOScale(0, inDuration).OnComplete(() => square1.transform.DOScale(1, outDuration)).SetLoops(-1);
        square2.transform.DOScale(1, outDuration).OnComplete(() => square2.transform.DOScale(0, inDuration)).SetLoops(-1);
    }
}
