using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Splines
{
    [CustomEditor(typeof(Splinepoint))]
    public class SplinepointInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Select Spline"))
            {
                Transform point = (target as Splinepoint).transform;

                if(point.parent != null && point.parent.GetComponent<Spline>())
                Selection.activeGameObject = point.parent.gameObject;
            }
        }
    }
}