using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FourLung : MonoBehaviour
{
    private PlayerActionInput inputActions;
    private bool isXray;

    [SerializeField]
    private UniversalRenderPipelineAsset Pipeline;

    private void Awake()
    {
        inputActions = new PlayerActionInput();
    }

    private void Start()
    {
        isXray = false;
        inputActions.InputPlayerAction.SkillButton.performed += SkillButton_performed;
        SwitchToNormalRenderer();
    }

    private void SkillButton_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isXray)
        {
            StartCoroutine(SkillTime(3f));
        }
    }

    private void OnEnable()
    {
        inputActions.InputPlayerAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.InputPlayerAction.Disable();
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
}
