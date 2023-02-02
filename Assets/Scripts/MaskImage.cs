using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

[RequireComponent(typeof(MaskInteraction))]
public class MaskImage : Image
{
    public override Material materialForRendering
    {
        get
        {
            int maskInter = (int)GetComponent<MaskInteraction>().maskInteraction;
            if (maskInter != 2)
            {
                Material material = new Material(base.materialForRendering);
                material.SetInt("_StencilComp", maskInter == 0 ? (int)CompareFunction.NotEqual : (int)CompareFunction.Equal);
                return material;
            }
            else
            {
                return base.materialForRendering;
            }
        }
    }
}
