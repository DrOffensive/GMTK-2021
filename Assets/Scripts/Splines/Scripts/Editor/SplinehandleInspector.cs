using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Splines
{
    [CustomEditor(typeof(SplineHandle))]
    public class SplinehandleInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Select Spline"))
            {
                Transform handle = (target as SplineHandle).transform;


                if (handle.parent != null && handle.parent.GetComponent<Splinepoint>())
                {
                    Transform point = handle.parent;
                    if(point.parent != null && point.parent.GetComponent<Spline>())
                    {
                        Selection.activeGameObject = point.parent.gameObject;
                    }
                }
            }
        }
    }
}