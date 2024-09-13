using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AILogic))] 
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        AILogic FOW = (AILogic)target;
        Handles.color = Color.black;
        Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, 360, 60);
    }
}
