using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Utility = Splines.SplineUtility;
using static Splines.BezierType;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Net;
using System.Dynamic;
using UnityEngine.iOS;

namespace Splines
{
    
    [ExecuteAlways]
    public class Spline : MonoBehaviour
    {
        [SerializeField] BezierType bezierType;
        [SerializeField] int curveAccuracy = 10;
        [SerializeField] bool closedLoop;
        [SerializeField] TransformSpace pointSpace;
        [SerializeField] SplineOrigin splineOrigin;
        [SerializeField] Vector3 customOrigin;
        [SerializeField] int customPoint;

        public Vector3 Origin
        {
            get
            {
                Vector3 pos = Vector3.zero;
                switch (splineOrigin)
                {
                    case SplineOrigin.FirstPoint:
                        if (points != null && points.Length > 0)
                            pos = points[0].Position;
                        else
                            Debug.LogWarning("Spline points not set !");
                        return pos;
                    case SplineOrigin.LastPoint:
                        if (points != null && points.Length > 0)
                            pos = points[points.Length - 1].Position;
                        else
                            Debug.LogWarning("Spline points not set !");
                        return pos;
                    case SplineOrigin.SplineCenter:
                        return Center;
                    case SplineOrigin.TransformOrigin:
                        return transform.position;
                    case SplineOrigin.CustomPoint:
                        return points[customPoint].Position;
                    case SplineOrigin.CustomPosition:
                        return customOrigin;
                    default: return pos;
                }
            }
        }

        public Vector3 RelativeToOrigin (Vector3 point)
        {
            return point - Origin;
        }

        public Splinepoint[] points => transform.GetComponentsInChildren<Splinepoint>();

        [SerializeField] EditorSplineManager.GizmoMode gizmoMode = EditorSplineManager.GizmoMode.OnSelected;

        public List<Vector3> Positions
        {
            get
            {
                List<Vector3> poss = new List<Vector3>();
                foreach (Splinepoint point in points)
                {
                    poss.Add(point.Position);
                }
                return poss;
            }
        }
        public Vector3 Center => FindCenter();
        event Action onChanged;
        public event Action OnChanged { add => onChanged += value; remove => onChanged -= value; }

        [Range(0,1)]
        [SerializeField] float debugPoint = 0;

        public float MoveDistanceAlongSpline (float startTime, float moveDistance)
        {
            //float pDistance = (1f / SplineLength) * moveDistance;
            return startTime + moveDistance;
        }

        public Bounds GetBounds()
        {
            (Vector3 Min, Vector3 Max) minMax = MinMax();
            Vector3 line = minMax.Max - minMax.Min;
            return new Bounds(minMax.Item1 + (line * .5f), line);
        }

        public (Vector3 , Vector3) MinMax()
        {
            List<Vector3> pts = Positions;
            float xMax = Mathf.NegativeInfinity, xMin = Mathf.Infinity;
            float yMax = Mathf.NegativeInfinity, yMin = Mathf.Infinity;
            float zMax = Mathf.NegativeInfinity, zMin = Mathf.Infinity;
            foreach (Vector3 pt in pts)
            {
                if (pt.x < xMin)
                    xMin = pt.x;

                if (pt.x > xMax)
                    xMax = pt.x;

                if (pt.y < yMin)
                    yMin = pt.y;

                if (pt.y > yMax)
                    yMax = pt.y;

                if (pt.z < zMin)
                    zMin = pt.z;

                if (pt.z > zMax)
                    zMax = pt.z;
            }
            Vector3 min = new Vector3(xMin, yMin, zMin), max = new Vector3(xMax, yMax, zMax);
            return (min, max);
        }

        public float SplineLength 
        { 
            get 
            {
                float segments = 0;
                for (int i = 1; i < points.Length; i++)
                    segments += SegmentLength(i - 1, i);
                if (ClosedLoop)
                    segments += SegmentLength(points.Length - 1, 0);
                return segments;
            }
        }

        public TransformSpace TransformSpace { get => pointSpace; }
        public BezierType BezierType { get => bezierType; }
        public bool ClosedLoop => closedLoop;

        public int Accuracy { get => curveAccuracy; }

        public Vector3[] GetAllCurvePoints (Vector3 offset = new Vector3())
        {
            if (points.Length <= 1)
                return new Vector3[0];
            List<Vector3> curvePoints = new List<Vector3>();
            for (int i = 1; i < points.Length; i++)
            {
                List<Vector3> curve = SegmentCurve(i - 1, i).ToList();
                if (!closedLoop && i != points.Length - 1)
                    curve.RemoveAt(curve.Count - 1);
                curvePoints.AddRange(curve);
            }
            if (ClosedLoop)
                curvePoints.AddRange(SegmentCurve(points.Length - 1, 0));

            if (offset != Vector3.zero)
                for(int i = 0; i < curvePoints.Count; i++)
                    curvePoints[i] = curvePoints[i] + offset;

            return curvePoints.ToArray();
        }

        Vector3[] SegmentCurve(int start, int end)
        {
            switch(bezierType)
            {
                default:
                    return Utility.GetLinearBezierCurve(2, RelativeToOrigin(points[start].Position), RelativeToOrigin(points[end].Position));

                case Quadratic:
                    return Utility.GetQuadraticBezierCurve(curveAccuracy, RelativeToOrigin(points[start].Position), RelativeToOrigin(points[end].Position), RelativeToOrigin(points[start].HandleA));

                case Cubic:
                    return Utility.GetCubicBezierCurve(curveAccuracy, RelativeToOrigin(points[start].Position), RelativeToOrigin(points[end].Position), RelativeToOrigin(points[start].HandleA), RelativeToOrigin(points[end].HandleB));
            }
        }

        public (Vector3 point, int segment) GetPointOnSpline (float time)
        {
            if (points.Length <= 1)
                return (points.Length == 0 ? transform.position : points[0].Position, 0);

            time = Mathf.Clamp01(time);
            float splineLength = SplineLength;
            float timeLength = splineLength * time;
            float cDist = 0;
            for(int i = 1; i < points.Length; i++)
            {
                float segmentLength = SegmentLength(i - 1, i);
                float tDist = cDist + segmentLength;
                if (tDist >= timeLength)
                {
                    float diff = timeLength - cDist;
                    float localLength = 1f / segmentLength * diff;
                    return (Point(i - 1, i, localLength), i - 1);
                }
                else
                    cDist = tDist;
            }
            if(ClosedLoop)
            {
                float segmentLength = SegmentLength(points.Length - 1, 0);
                float tDist = cDist + segmentLength;
                if (tDist >= timeLength)
                {
                    float diff = timeLength - cDist;
                    float localLength = 1f / segmentLength * diff;
                    return (Point(points.Length - 1, 0, localLength), points.Length - 1);
                }
            }

            Debug.LogWarning("Reaching this should be impossible. Something went wrong in 'GetPointOnSpline(float time)'");
            return (Vector3.zero, 0);
        }

        public Vector3 Point(int start, int end, float t)
        {
            t = Mathf.Clamp01(t);
            switch (bezierType)
            {
                case Linear:
                    return Utility.GetLinearBezierPoint(t, points[start].Position, points[end].Position);
                case Quadratic:
                    return Utility.GetQuadraticBezierPoint(t, points[start].Position, points[start].HandleA, points[end].Position);
                case Cubic:
                    return Utility.GetCubicBezierPoint(t, points[start].Position, points[start].HandleA, points[end].HandleB, points[end].Position);
                default: goto case Linear;
            }
        }

        (int index, float localTime) GetSegmentTime(float splinetime)
        {
            if (points.Length <= 1)
                return (0, 0f);

            splinetime = Mathf.Clamp01(splinetime);
            float splineLength = SplineLength;
            float timeLength = splineLength * splinetime;
            float cDist = 0;
            for (int i = 1; i < points.Length; i++)
            {
                float segmentLength = SegmentLength(i - 1, i);
                float tDist = cDist + segmentLength;
                if (tDist >= timeLength)
                {
                    float diff = timeLength - cDist;
                    float p = (1f / segmentLength) * diff;
                    return (i - 1, p);
                }
                else
                    cDist = tDist;
            }
            if (ClosedLoop)
            {
                float segmentLength = SegmentLength(points.Length - 1, 0);
                float tDist = cDist + segmentLength;
                if (tDist >= timeLength)
                {
                    float diff = timeLength - cDist;
                    float p = (1f / segmentLength) * diff;
                    return (points.Length - 1, p);
                }
            }

            Debug.LogWarning("Reaching this should be impossible. Something went wrong in 'GetPointOnSpline(float time)'");
            return (0, 0f);
        }
        private void OnDrawGizmosSelected ()
        {
            if (gizmoMode == EditorSplineManager.GizmoMode.OnSelected && EditorSplineManager.ShowGizmos != EditorSplineManager.GizmoMode.Hidden)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(Origin, .075f);
                for (int i = 0; i < points.Length; i++)
                {
                    Gizmos.DrawSphere(points[i].Position, .025f);
                    if (bezierType == Linear)
                    {
                        if (i > 0)
                        {
                            Gizmos.DrawLine(points[i - 1].Position, points[i].Position);
                        }
                    }
                    else
                    {
                        Vector3[] curve = null;
                        if (i > 0)
                        {
                            curve = GetAllCurvePoints();
                            for (int x = 0; x < curve.Length; x++)
                            {
                                if (x > 0)
                                    Gizmos.DrawLine(curve[x - 1] + Origin, curve[x] + Origin);
                            }
                        }
                        if (i < points.Length - 1 || ClosedLoop)
                        {
                            if (bezierType != BezierType.Linear)
                            {
                                Gizmos.color = Color.red;
                                Gizmos.DrawSphere(points[i].HandleA, .025f);
                                Gizmos.DrawLine(points[i].HandleA, points[i].Position);
                                if (bezierType == Cubic)
                                {
                                    Gizmos.DrawSphere(points[i].HandleB, .025f);
                                    Gizmos.DrawLine(points[i].HandleB, points[i].Position);
                                }
                            }
                            Gizmos.color = Color.white;
                        }
                    }
                }
                /*if (closedLoop && points.Length > 1)
                    Gizmos.DrawLine(points[0].Position, points[points.Length - 1].Position);*/

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(Center, .045f);

                Gizmos.color = Color.cyan;
                //Gizmos.DrawWireSphere(GetPointOnSpline(debugPoint).point, .55f);

                if (bezierType == Quadratic)
                {
                    (int point, float time) segment = GetSegmentTime(debugPoint);
                    int end = segment.point + 1 >= points.Length ? 0 : segment.point + 1;
                    Vector3 pointTangent = Utility.GetQuadraticBezierTangentAtPoint(segment.time, points[segment.point].Position, points[end].Position, points[segment.point].HandleA).normalized;
                    Vector3 dPoint = GetPointOnSpline(debugPoint).point;
                    Gizmos.DrawWireSphere(dPoint + (pointTangent * .25f), .0125f);
                    Gizmos.DrawWireSphere(dPoint - (pointTangent * .25f), .0125f);
                    Gizmos.DrawLine(dPoint + (pointTangent * .25f), dPoint - (pointTangent * .25f));
                }
                else if (bezierType == Cubic)
                {
                    (int point, float time) segment = GetSegmentTime(debugPoint);
                    int end = segment.point + 1 >= points.Length ? 0 : segment.point + 1;
                    Vector3 pointTangent = Utility.GetCubicBezierTangentAtPoint(segment.time, points[segment.point].Position, points[segment.point].HandleA, points[end].HandleB, points[end].Position).normalized;
                    Vector3 dPoint = GetPointOnSpline(debugPoint).point;
                    Gizmos.DrawWireSphere(dPoint + (pointTangent * .25f), .0125f);
                    Gizmos.DrawWireSphere(dPoint - (pointTangent * .25f), .0125f);
                    Gizmos.DrawLine(dPoint + (pointTangent * .25f), dPoint - (pointTangent * .25f));
                }

                Gizmos.DrawWireSphere(GetPointOnSpline(debugPoint).point, .045f);
                Gizmos.color = Color.white;
                foreach (Vector3 vert in GetAllCurvePoints())
                    Gizmos.DrawWireSphere(vert + Origin, .0125f);
            }
        }

        private void OnDrawGizmos()
        {
            if(gizmoMode == EditorSplineManager.GizmoMode.Shown && EditorSplineManager.ShowGizmos != EditorSplineManager.GizmoMode.Hidden)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(Origin, .075f);
                for (int i = 0; i < points.Length; i++)
                {
                    Gizmos.DrawSphere(points[i].Position, .025f);
                    if (bezierType == Linear)
                    {
                        if (i > 0)
                        {
                            Gizmos.DrawLine(points[i - 1].Position, points[i].Position);
                        }
                    }
                    else
                    {
                        Vector3[] curve = null;
                        if (i > 0)
                        {
                                curve = GetAllCurvePoints();
                            for (int x = 0; x < curve.Length; x++)
                            {
                                if (x > 0)
                                    Gizmos.DrawLine(curve[x - 1] + Origin, curve[x] + Origin);
                            }
                        }
                        if (i < points.Length - 1 || ClosedLoop)
                        {
                            if (bezierType != BezierType.Linear)
                            {
                                Gizmos.color = Color.red;
                                Gizmos.DrawSphere(points[i].HandleA, .025f);
                                Gizmos.DrawLine(points[i].HandleA, points[i].Position);
                                if (bezierType == Cubic)
                                {
                                    Gizmos.DrawSphere(points[i].HandleB, .025f);
                                    Gizmos.DrawLine(points[i].HandleB, points[i].Position);
                                }
                            }
                            Gizmos.color = Color.white;
                        }
                    }
                }
                /*if (closedLoop && points.Length > 1)
                    Gizmos.DrawLine(points[0].Position, points[points.Length - 1].Position);*/

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(Center, .045f);

                Gizmos.color = Color.cyan;
                //Gizmos.DrawWireSphere(GetPointOnSpline(debugPoint).point, .55f);
                
                if(bezierType == Quadratic)
                {
                    (int point, float time) segment = GetSegmentTime(debugPoint);
                    int end = segment.point + 1 >= points.Length ? 0 : segment.point + 1;
                    Vector3 pointTangent = Utility.GetQuadraticBezierTangentAtPoint(segment.time, points[segment.point].Position, points[end].Position, points[segment.point].HandleA).normalized;
                    Vector3 dPoint = GetPointOnSpline(debugPoint).point;
                    Gizmos.DrawWireSphere(dPoint + (pointTangent * .25f), .0125f);
                    Gizmos.DrawWireSphere(dPoint - (pointTangent * .25f), .0125f);
                    Gizmos.DrawLine(dPoint + (pointTangent * .25f), dPoint - (pointTangent * .25f));
                } else if (bezierType == Cubic)
                {
                    (int point, float time) segment = GetSegmentTime(debugPoint);
                    int end = segment.point + 1 >= points.Length ? 0 : segment.point + 1;
                    Vector3 pointTangent = Utility.GetCubicBezierTangentAtPoint(segment.time, points[segment.point].Position, points[segment.point].HandleA, points[end].HandleB, points[end].Position).normalized;
                    Vector3 dPoint = GetPointOnSpline(debugPoint).point;
                    Gizmos.DrawWireSphere(dPoint + (pointTangent * .25f), .0125f);
                    Gizmos.DrawWireSphere(dPoint - (pointTangent * .25f), .0125f);
                    Gizmos.DrawLine(dPoint + (pointTangent * .25f), dPoint - (pointTangent * .25f));
                }

                Gizmos.DrawWireSphere(GetPointOnSpline(debugPoint).point, .045f);
                Gizmos.color = Color.white;
                foreach (Vector3 vert in GetAllCurvePoints())
                    Gizmos.DrawWireSphere(vert + Origin, .0125f);
            }
        }

        public Vector3 Tangent (float time)
        {
            var segment = GetSegmentTime(time);
            return Tangent(segment.localTime, segment.index, segment.index + 1 >= points.Length ? 0 : segment.index + 1);
        }

        public Vector3 Tangent (float time, int start, int end)
        {
            Vector3 tangent = new Vector3();
            switch (bezierType)
            {
                case Linear:
                    tangent = (points[start].Position - points[end].Position).normalized;
                    break;
                case Quadratic:
                    tangent = Utility.GetQuadraticBezierTangentAtPoint(time, points[start].Position, points[end].Position, points[start].HandleA);
                    break;
                case Cubic:
                    tangent = Utility.GetCubicBezierTangentAtPoint(time, points[start].Position, points[start].HandleA, points[end].HandleB, points[end].Position);
                    break;
            }
            float distToStart = Vector3.Distance(points[start].Position + tangent, points[start].Position);
            float distToEnd = Vector3.Distance(points[start].Position + tangent, points[end].Position);
            return (distToStart < distToEnd) ? -tangent : tangent;
        }

        public void PointMoved ()
        {
            onChanged?.Invoke();
        }

        public float SegmentLength (int segmentStart, int segmentEnd)
        {
            if (points.Length > 1)
            {
                Vector3 start = points[segmentStart].Position;
                Vector3 end = points[segmentEnd].Position;
                if (bezierType == Linear)
                    return Vector3.Distance(start, end);
                else
                {
                    Vector3[] curve = new Vector3[0];
                    if (bezierType == Quadratic)
                        curve = Utility.GetQuadraticBezierCurve(curveAccuracy, start, end, points[segmentStart].HandleA);
                    else
                        curve = Utility.GetCubicBezierCurve(curveAccuracy, start, end, points[segmentStart].HandleA, points[segmentEnd].HandleB);
                    float distance = 0;

                    for (int i = 1; i < curveAccuracy; i++)
                    {
                        distance += Vector3.Distance(curve[i - 1], curve[i]);
                    }
                    return distance;
                }
            }
            else return 0f;
        }
    
        Vector3 FindCenter ()
        {
            float xMin = Mathf.Infinity, yMin = Mathf.Infinity, zMin = Mathf.Infinity;
            float xMax = Mathf.NegativeInfinity, yMax = Mathf.NegativeInfinity, zMax = Mathf.NegativeInfinity;
            if (bezierType == Linear)
            {
                foreach (Splinepoint point in points)
                {
                    Vector3 position = point.Position;
                    if (position.x < xMin)
                        xMin = position.x;
                    if (position.x > xMax)
                        xMax = position.x;
                    if (position.y < yMin)
                        yMin = position.y;
                    if (position.y > yMax)
                        yMax = position.y;
                    if (position.z < zMin)
                        zMin = position.z;
                    if (position.z > zMax)
                        zMax = position.z;
                }
            } else
            {
                for (int i = 0; i < points.Length; i++) {

                    Vector3[] curve = new Vector3[0];
                    if (i == 0 && !ClosedLoop)
                        continue;
                    else if (i == 0)
                    {
                        if (bezierType == Quadratic)
                            curve = Utility.GetQuadraticBezierCurve(curveAccuracy, points[points.Length - 1].Position, points[i].Position, points[points.Length - 1].HandleA);
                        else
                            curve = Utility.GetCubicBezierCurve(curveAccuracy, points[points.Length - 1].Position, points[i].Position, points[points.Length - 1].HandleA, points[i].HandleB);
                    }
                    else
                    {
                        if (bezierType == Quadratic)
                            curve = Utility.GetQuadraticBezierCurve(curveAccuracy, points[i - 1].Position, points[i].Position, points[i - 1].HandleA);
                        else
                            curve = Utility.GetCubicBezierCurve(curveAccuracy, points[i - 1].Position, points[i].Position, points[i - 1].HandleA, points[i].HandleB);
                    }
                    for(int x = 0; x < curve.Length; x++)
                    {
                        Vector3 position = curve[x];
                        if (position.x < xMin)
                            xMin = position.x;
                        if (position.x > xMax)
                            xMax = position.x;
                        if (position.y < yMin)
                            yMin = position.y;
                        if (position.y > yMax)
                            yMax = position.y;
                        if (position.z < zMin)
                            zMin = position.z;
                        if (position.z > zMax)
                            zMax = position.z;
                    }
                }
            }
            Vector3 min = new Vector3(xMin, yMin, zMin);
            Vector3 max = new Vector3(xMax, yMax, zMax);
            Vector3 difference = max - min;
            return min + (difference / 2);
        }
    }

    [ExecuteAlways]
    public static class EditorSplineManager
    {
        static GizmoMode gizmoMode = GizmoMode.OnSelected;

        [MenuItem("Tools/Splines/Show|Hide Gizmos")]
        static void GizmosShow ()
        {
            if (gizmoMode != GizmoMode.Hidden)
                gizmoMode = GizmoMode.Hidden;
            else
                gizmoMode = GizmoMode.Shown;
        }

        public static GizmoMode ShowGizmos => gizmoMode;
        public enum GizmoMode { Hidden, OnSelected, Shown }
    }

    public enum TransformSpace { Global, Local }

}