using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public List<BodyParts> bodyParts = new List<BodyParts>();
    public List<SkinnedMeshRenderer> _charaSkins = new List<SkinnedMeshRenderer>();

    public void ToggleBodyPartsCollider(bool change)
    {
        foreach(BodyParts bodyPart in bodyParts)
        {
            bodyPart.bodyPartsColider.enabled = change;
        }
    }
    public void MakeItTransparent(float duration)
    {
        foreach(SkinnedMeshRenderer skin in _charaSkins)
        {
            foreach(Material mat in skin.materials)
            {
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.SetInt("_Surface", 1);

                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                mat.SetShaderPassEnabled("DepthOnly", false);
                mat.SetShaderPassEnabled("SHADOWCASTER", true);

                mat.SetOverrideTag("RenderType", "Transparent");
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");

                LeanTween.value(mat.color.a, 0, duration).
                setOnUpdate((float value)=>
                    {
                        Color color = mat.color;
                        color.a = value;
                        mat.color = color;
                    }
                );
            }
        }   
    }
}