using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Splines.Operations {
    namespace Helpers {
        [ExecuteAlways]
        public class SplineGhost : MonoBehaviour
        {
            public Transform[] points;

            Vector3 Position { get => transform.position; set => transform.position = value; }
            Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }
            public bool Created { get; set; } = false;
            public bool closedLoop = false;
            Spline path { get; set; }

            List<Vector3> PointPositions
            {
                get
                {
                    List<Vector3> positions = new List<Vector3>();
                    foreach(Transform point in points)
                    {
                        positions.Add(point.position);
                    }
                    return positions;
                }
            }

            public static SplineGhost Create(Spline spline, Spline path, ExtrudeSpline.ExtrusionOrigin extrusionOrigin)
            {
                SplineGhost splineGhost = Instantiate(Resources.Load<SplineGhost>("Splines/SplineGhost"));
                GameObject ghost = splineGhost.gameObject;
                ghost.transform.position = spline.Origin;

                Vector3[] curvePoints = spline.GetAllCurvePoints();
                Transform[] curveTransforms = new Transform[curvePoints.Length];
                Vector3[] pathCurve = path.GetAllCurvePoints();
                Vector3 forward = (pathCurve[1] - pathCurve[0]).normalized;
                /*switch (spline.BezierType)
                {
                    case BezierType.Linear:
                        forward = (path.points[1].Position - path.points[0].Position).normalized;
                        break;

                    case BezierType.Quadratic:
                        forward = SplineUtility.GetQuadraticBezierTangentAtPoint(0, path.points[0].Position, path.points[1].Position, path.points[0].HandleA);
                        break;

                    case BezierType.Cubic:
                        forward = SplineUtility.GetCubicBezierTangentAtPoint(0, path.points[0].Position, path.points[0].HandleA, path.points[1].HandleB, path.points[1].Position);
                        break;
                }*/


                splineGhost.Rotation = Quaternion.LookRotation(forward);
                splineGhost.path = path;
                int index = 0;
                foreach (Vector3 point in curvePoints)
                {
                    Transform tr = curveTransforms[index] = Instantiate(SplineUtility.GetEmpty()).transform;
                    //tr.position = spline.RelativeToOrigin(point);
                    tr.SetParent(ghost.transform);
                    tr.localPosition = point;
                    index++;
                }
                splineGhost.closedLoop = path.ClosedLoop;
                splineGhost.points = curveTransforms;
                /*if(extrusionOrigin == ExtrudeSpline.ExtrusionOrigin.Path)
                    ghost.transform.position = path.points[0].Position;*/
                splineGhost.transform.position = Vector3.zero;
                splineGhost.Created = true;
                return splineGhost;
            }

            public SplineGhostPath CalculatePath ()
            {
                SplineGhostPath splinePath = new SplineGhostPath(closedLoop);
                Vector3[] pathCurve = path.GetAllCurvePoints();
                Vector3 cpoint = pathCurve[0];
                transform.rotation = Quaternion.Euler((pathCurve[1] - pathCurve[0]).normalized);
                splinePath.AddSegment(PointPositions);
                for (int i = 1; i <= pathCurve.Length; i++)
                {
                    Vector3 direction;
                    if (i < pathCurve.Length)
                        direction = (pathCurve[i] - pathCurve[i - 1]);
                    else if (i == pathCurve.Length && closedLoop)
                        direction = (pathCurve[0] - pathCurve[i - 1]);
                    else
                        break;
                    transform.rotation = Quaternion.Euler(direction.normalized);
                    transform.position += direction;
                    splinePath.AddSegment(PointPositions);
                }
                return splinePath;
            }

            bool destroy;

            public void DestroyWhenReady() => destroy = true;

            private void Update()
            {
                if(destroy)
                    if (!Application.isPlaying)
                        DestroyImmediate(gameObject);
                    else
                        Destroy(gameObject);
            }

            public struct SplineGhostPath {
                public struct SplineGhostPathSegment
                {
                    List<Vector3> points;

                    public SplineGhostPathSegment(List<Vector3> points)
                    {
                        this.points = points;
                    }

                    public List<Vector3> Points => points;

                    public Vector3 Point(int index) => points[index];
                }

                List<SplineGhostPathSegment> splineGhostPathSegments;
                bool closedLoop;

                public List<SplineGhostPathSegment> SplineGhostPathSegments { get => splineGhostPathSegments; }
                public List<Vector3> Segment(int i) => splineGhostPathSegments[i].Points;

                public List<Vector3> AllPositions { 
                    get 
                    {
                        List<Vector3> positions = new List<Vector3>();
                        foreach(SplineGhostPathSegment segment in SplineGhostPathSegments)
                        {
                            positions.AddRange(segment.Points);
                        }
                        return positions;
                    } 
                }

                public int SegmentVerts => splineGhostPathSegments[0].Points.Count;
                public int Segments => splineGhostPathSegments.Count;
                public bool ClosedLoop { get => closedLoop; }

                public SplineGhostPath(bool closedLoop)
                {
                    this.closedLoop = closedLoop;
                    splineGhostPathSegments = new List<SplineGhostPathSegment>();
                }

                public void AddSegment (List<Vector3> segment)
                {
                    splineGhostPathSegments.Add(new SplineGhostPathSegment(segment));
                }
            }
        } 
    }
}