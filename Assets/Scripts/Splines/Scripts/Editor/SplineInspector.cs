using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorUtility = CustomUI.EditorUtility;
using UIStyles = CustomUI.UIStyles;
using Splines.Operations;

namespace Splines
{
    [CustomEditor(typeof(Spline))]
    public class SplineInspector : Editor
    {
        SerializedProperty curveProperty;
        SerializedProperty accuracyProperty;
        SerializedProperty showGizmoProperty;
        SerializedProperty closedLoopProperty;
        SerializedProperty spaceProperty;
        SerializedProperty debugPointProperty;
        SerializedProperty originProperty;
        Vector2 scrollPos = Vector2.zero;
        bool pointsOpen = true;
        int selPoint;

        private void OnEnable()
        {
            curveProperty = serializedObject.FindProperty("bezierType");
            accuracyProperty = serializedObject.FindProperty("curveAccuracy");
            showGizmoProperty = serializedObject.FindProperty("gizmoMode");
            closedLoopProperty = serializedObject.FindProperty("closedLoop");
            spaceProperty = serializedObject.FindProperty("pointSpace");
            debugPointProperty = serializedObject.FindProperty("debugPoint");
            originProperty = serializedObject.FindProperty("splineOrigin");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            Spline spline = target as Spline;
            serializedObject.Update();

            EditorGUILayout.PropertyField(curveProperty);
            if(spline.BezierType != BezierType.Linear)
            EditorGUILayout.PropertyField(accuracyProperty);
            EditorGUILayout.PropertyField(closedLoopProperty);
            EditorGUILayout.PropertyField(originProperty);
            EditorGUILayout.BeginHorizontal();
            if(EditorGUILayout.DropdownButton(new GUIContent((!pointsOpen ? "►" : "▼") + " Spline Points: " + spline.points.Length), FocusType.Keyboard, UIStyles.Bold)) 
                pointsOpen = !pointsOpen;

            EditorGUILayout.PropertyField(spaceProperty);
            EditorGUILayout.EndHorizontal();
            if (pointsOpen)
            {
                EditorGUILayout.Space(5);
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                bool remove = false;
                for(int i = 0; i < spline.points.Length; i++)
                {
                    bool selected = selPoint == i;
                    if (EditorGUILayout.DropdownButton(new GUIContent((!selected ? "+" : "-") + $" Point #{i + 1}"), FocusType.Keyboard, UIStyles.Bold))
                        selPoint = selPoint == i ? -1 : i;

                    Vector3 point = spline.TransformSpace == TransformSpace.Global ? spline.points[i].Position : spline.points[i].LocalPosition;

                    if(selected)
                    {
                        Vector3 nPoint = point;
                        nPoint = EditorGUILayout.Vector3Field("Position:", nPoint);
                        if (point != nPoint)
                        {
                            if (spline.TransformSpace == TransformSpace.Global)
                                spline.points[i].Position = nPoint;
                            else
                                spline.points[i].LocalPosition = nPoint;
                        }
                        EditorGUILayout.Space(5);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Move down"))
                            MovePoint(spline.points, i, i + 1);
                        if (GUILayout.Button("Move up"))
                            MovePoint(spline.points, i, i - 1);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(5);
                        if (GUILayout.Button("Remove Point"))
                            remove = true;
                        if (GUILayout.Button("Select Point"))
                            SelectGameObject(spline.points[i].gameObject);
                        if (GUILayout.Button("Select Handle"))
                            SelectGameObject(spline.points[i].Handle.gameObject);
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"Position ({EditorUtility.LimitDecimals(point.x, 2)}, {EditorUtility.LimitDecimals(point.y, 2)}, {EditorUtility.LimitDecimals(point.z, 2)})", UIStyles.Itallic);
                    }
                }
                if (remove)
                    RemovePoint(spline.points, selPoint);

                EditorGUI.indentLevel = indent;
            }
            if (GUILayout.Button("Add point"))
                CreatePoint(spline);
            EditorGUILayout.LabelField($"Spline length: {spline.SplineLength}", UIStyles.Itallic);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Operations:", UIStyles.Bold);
            EnabledDisabledButton("Extrude", !spline.GetComponent<ExtrudeSpline>(), () => { spline.gameObject.AddComponent<ExtrudeSpline>(); });
            
            EditorGUILayout.PropertyField(showGizmoProperty);
            if(showGizmoProperty.boolValue == true)
                EditorGUILayout.PropertyField(debugPointProperty);
            serializedObject.ApplyModifiedProperties();
        }

        void SelectGameObject (GameObject gameObject)
        {
            Selection.activeGameObject = gameObject;
        }

        void RemovePoint (Splinepoint[] points, int index)
        {
            DestroyImmediate(points[index].gameObject);
            selPoint = Mathf.Clamp(selPoint - 1, 0, points.Length);
        }

        void MovePoint (Splinepoint[] points, int oldPosition, int newPosition)
        {
            if (newPosition < 0 || newPosition >= points.Length)
                return;
            points[oldPosition].transform.SetSiblingIndex(newPosition);
            selPoint = newPosition;
        }

        void CreatePoint (Spline spline)
        {
            if (spline.points.Length == 0)
                Splinepoint.CreateNew(spline.transform.position, spline.transform);
            else if (spline.points.Length == 1)
                Splinepoint.CreateNew(spline.transform.position + Vector3.up, spline.transform);
            else
            {
                int end = spline.points.Length - 1;
                Vector3 position;
                Vector3 line = spline.points[end].Position - spline.points[end - 1].Position;
                position = spline.points[end].Position + line;
                Splinepoint.CreateNew(position, spline.transform);
            }
        }

        void EnabledDisabledButton (string label, bool enabled, System.Action callback)
        {
            GUI.enabled = enabled;
            if (GUILayout.Button(label))
                callback?.Invoke();

            GUI.enabled = true;
        }
    }
}