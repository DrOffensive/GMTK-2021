using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Splines.Operations
{
    [ExecuteAlways]
    [CustomEditor(typeof(PlaceAlongSpline))]
    public class placeAlongSplineInspector : Editor
    {

        SerializedProperty amountProperty;
        SerializedProperty placementProperty;
        SerializedProperty pathProperty;
        SerializedProperty facingProperty;
        SerializedProperty resetProperty;

        [MenuItem("Splines/Place Along Spline")]
        void PlaceAlongSpline()
        {
            Debug.Log("Doing something");
        }


        private void OnEnable()
        {
            amountProperty = serializedObject.FindProperty("amount");
            placementProperty = serializedObject.FindProperty("placementStyle");
            pathProperty = serializedObject.FindProperty("path");
            facingProperty = serializedObject.FindProperty("faceSplineDirection");
            resetProperty = serializedObject.FindProperty("resetOnUpdate");
        }

        public override void OnInspectorGUI()
        {
            /*EditorGUILayout.PropertyField(pathProperty);
            EditorGUILayout.PropertyField(amountProperty);
            EditorGUILayout.PropertyField(placementProperty);
            EditorGUILayout.PropertyField(facingProperty);
            EditorGUILayout.PropertyField(resetProperty);*/

            base.OnInspectorGUI();
            if (GUILayout.Button("Place"))
            {
                (target as PlaceAlongSpline).Place();
            }

        }
    }
}