using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FourLung : PlayableSkill
{
    private bool isXray;
    [SerializeField] private float _skillDuration = 3f;

    [SerializeField]
    private UniversalRenderPipelineAsset Pipeline;
    #region Getter Setter Variable
    public override bool IsSkillOnGoing 
    {
        get
        {
            return isXray;
        }
    }
    #endregion


    protected override void Start()
    {
        base.Start();
        isXray = false;

        SwitchToNormalRenderer();
    }


    public void SetRenderer(int index)
    {
        if (Pipeline != null)
        {
            var field = typeof(UniversalRenderPipelineAsset).GetField("m_DefaultRendererIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(Pipeline, index);  
            }
        }
    }

    public void SwitchToNormalRenderer()
    {
        SetRenderer(0); 
    }

    public void SwitchToXRayRenderer()
    {
        SetRenderer(1); 
    }

    private IEnumerator SkillTime(float delay)
    {
        SwitchToXRayRenderer();
        isXray = true;

        yield return new WaitForSeconds(delay);

        SwitchToNormalRenderer();
        isXray = false;
    }

    public override void UseSkill()
    {
        if(!_gm.IsEventGamePlayMode()) return;
        if(!isXray)
        {
            StartCoroutine(SkillTime(_skillDuration));
        }
    }
}
