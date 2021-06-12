using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Splines.Operations
{
    [CustomEditor(typeof(ExtrudeSpline))]
    public class extrudeSplineInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Extrude"))
            {
                ExtrudeSpline extrude = target as ExtrudeSpline;
                extrude.UpdateMesh();
            }
        }
    }
}