using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        FOV fov = (FOV)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.FOVPoint.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);

        Vector3 viewAngleA = fov.DirectionFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirectionFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.FOVPoint.position, fov.FOVPoint.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.FOVPoint.position, fov.FOVPoint.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.red;
        foreach(Transform visibleTarget in fov.visibleTargets)
        {
            Handles.DrawLine(fov.FOVPoint.position, visibleTarget.position);
        }
    }
}
