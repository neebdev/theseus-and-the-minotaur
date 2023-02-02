using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

[RequireComponent(typeof(MaskInteraction))]

public class MaskTextMeshProUGUI : TextMeshProUGUI
{
    public override Material materialForRendering
    {
        get
        {
            int maskInter = (int)GetComponent<MaskInteraction>().maskInteraction;
            if (maskInter != 2)
            {
                Material material = new Material(base.materialForRendering);
                if (maskInter != 2)
                {
                    material.SetInt("_StencilComp", maskInter == 0 ? (int)CompareFunction.Equal : (int)CompareFunction.NotEqual);
                }
                else
                {
                    material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                }
                return material;
            }
            else
            {
                return base.materialForRendering;
            }
        }
    }
}
