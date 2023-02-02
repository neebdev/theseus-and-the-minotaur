using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskInteraction : MonoBehaviour
{
    public enum MaskInter
    {
        VisibleOutsideMask,
        VisibleInsideMask,
        None,
    }
    public MaskInter maskInteraction;
    private void ReRenderGraphics()
    {
        if (TryGetComponent<Image>(out var img))
        {
            img.enabled = false;
            img.enabled = true;
        }
        if (TryGetComponent<TextMeshProUGUI>(out var txt))
        {
            txt.enabled = false;
            txt.enabled = true;
        }
    }
    public void SetInteraction(MaskInter maskInteraction)
    {
        this.maskInteraction = maskInteraction;
        ReRenderGraphics();
    }
}
