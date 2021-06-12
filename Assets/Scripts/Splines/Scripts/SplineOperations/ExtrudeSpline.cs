using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Assertions.Must;
using System.Security.Cryptography;

namespace Splines
{
    namespace Operations {

        [ExecuteAlways]
        [RequireComponent(typeof(Spline))]
        public class ExtrudeSpline : MonoBehaviour
        {
            [SerializeField] bool updateOnChange;
            [SerializeField] SplineOrigin splineOrigin = SplineOrigin.SplineCenter;
            [SerializeField] ExtrusionStyle extrusionStyle = ExtrusionStyle.Direct;
            [SerializeField] ExtrusionOrigin extrusionOrigin = ExtrusionOrigin.Spline;
            [SerializeField] float extrusionDistance = 5f;
            [SerializeField] Spline path;
            [SerializeField] bool flippedNormals;
            [SerializeField] Vector3 directDirection = Vector3.forward;
            [SerializeField] Vector3 customOrigin;
            [SerializeField] int customPoint;
            [SerializeField] bool showGizmo = true;
            [SerializeField] bool capEnds = true;
            [SerializeField] Material meshMaterial;
            bool generated;

            public Spline Path
            {
                get => path;
                set => path = value;
            }

            private void OnValidate()
            {
                if (updateOnChange && generated)
                    UpdateMesh();
            }

            MeshFilter Filter
            {
                get
                {
                    if (GetComponent<MeshFilter>() == null)
                        gameObject.AddComponent<MeshFilter>();
                    return GetComponent<MeshFilter>();
                }
            }

            Mesh Mesh 
            { 
                get 
                {
                    if (Filter.sharedMesh == null)
                        return new Mesh();
                    return Filter.sharedMesh;
                } 
                set => Filter.mesh = value; 
            }

            MeshRenderer Renderer
            {
                get
                {
                    if (GetComponent<MeshRenderer>() == null)
                        gameObject.AddComponent<MeshRenderer>();
                    return GetComponent<MeshRenderer>();
                }
            }

            Material material
            {
                get => Application.isPlaying ? Renderer.material : Renderer.sharedMaterial;
                set
                {
                    if (Application.isPlaying)
                        Renderer.material = value;
                    else
                        Renderer.sharedMaterial = value;
                }
            }

            public Material SetMeshMaterial
            {
                set
                {
                    meshMaterial = value;
                    if (generated)
                        material = meshMaterial;
                }
            }

            Spline spline => GetComponent<Spline>();

            /*(Vector3 min, Vector3 max) minMax 
            { 
                get
                {

                }
            }*/

            Vector3 Origin {
                get
                {
                    Vector3 pos = Vector3.zero;
                    switch (splineOrigin)
                    {
                        case SplineOrigin.FirstPoint:
                            if (spline.points != null && spline.points.Length > 0)
                                pos = spline.points[0].Position;
                            else
                                Debug.LogWarning("Spline points not set !");
                            return pos;
                        case SplineOrigin.LastPoint:
                            if (spline.points != null && spline.points.Length > 0)
                                pos = spline.points[spline.points.Length - 1].Position;
                            else
                                Debug.LogWarning("Spline points not set !");
                            return pos;
                        case SplineOrigin.SplineCenter:
                            return spline.Center;
                        case SplineOrigin.TransformOrigin:
                            return spline.transform.position;
                        case SplineOrigin.CustomPoint:
                            return spline.points[customPoint].Position;
                        case SplineOrigin.CustomPosition:
                            return customOrigin;
                        default: return pos;
                    }
                }
            }

            private void OnDrawGizmos()
            {
                if (showGizmo)
                {
                    Gizmos.color = Color.white;
                    if (generated)
                        foreach(Vector3 vert in Mesh.vertices)
                            Gizmos.DrawWireSphere(vert, .01f);

                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(Origin, .05f);

                    switch (extrusionStyle)
                    {
                        case ExtrusionStyle.Direct:
                            if (splineOrigin == SplineOrigin.CustomPosition || splineOrigin == SplineOrigin.SplineCenter || splineOrigin == SplineOrigin.TransformOrigin) {
                                Gizmos.color = Color.yellow;
                                Gizmos.DrawLine(Origin, Origin + (directDirection.normalized * extrusionDistance));
                                Gizmos.color = Color.red;
                                Gizmos.DrawWireSphere(Origin + (directDirection.normalized * extrusionDistance), .05f);
                            }

                            int directIndex = 0;

                            foreach (Vector3 point in spline.Positions)
                            {
                                Gizmos.color = Color.green;
                                bool dontDraw = false;
                                if (splineOrigin == SplineOrigin.CustomPoint && directIndex == customPoint)
                                    dontDraw = true;
                                if (splineOrigin == SplineOrigin.FirstPoint && directIndex == 0)
                                    dontDraw = true;
                                if (splineOrigin == SplineOrigin.LastPoint && directIndex == spline.points.Length - 1)
                                    dontDraw = true;

                                if (!dontDraw)
                                    Gizmos.DrawWireSphere(point, .05f);

                                Gizmos.color = dontDraw ? Color.yellow : Color.white;
                                Gizmos.DrawLine(point, point + (directDirection.normalized * extrusionDistance));

                                Gizmos.color = !dontDraw ? Color.green : Color.red;
                                Gizmos.DrawWireSphere(point + (directDirection.normalized * extrusionDistance), .05f);
                                directIndex++;
                            }
                            Vector3[] curvePoints = spline.GetAllCurvePoints();
                            Gizmos.color = Color.white;
                            for(int i = 1; i < curvePoints.Length; i++)
                            {
                                Gizmos.DrawLine((curvePoints[i - 1] + spline.Origin) + (directDirection.normalized * extrusionDistance), (curvePoints[i] + spline.Origin) + (directDirection.normalized * extrusionDistance));
                            }
                            break;

                        case ExtrusionStyle.AlongSpline:

                            break;

                        default: throw new NotImplementedException();
                    }
                }
            }

            Vector3[] GetEndPoints ()
            {
                Vector3[] endPoints = new Vector3[spline.points.Length];
                int index = 0;
                switch (splineOrigin)
                {
                    case SplineOrigin.CustomPoint:
                        goto case SplineOrigin.LastPoint;


                    case SplineOrigin.FirstPoint:
                        goto case SplineOrigin.LastPoint;

                    case SplineOrigin.LastPoint:
                        if(extrusionStyle == ExtrusionStyle.Direct)
                        {  
                            foreach(Vector3 point in spline.Positions)
                            {
                                endPoints[index] = spline.Positions[index] + (directDirection.normalized * extrusionDistance);
                                index++;
                            }
                        }
                        return endPoints;

                    default: throw new NotImplementedException();
                }
            }

            Vector3 RelativeToOrigin (Vector3 point)
            {
                return point - Origin;
            }

            public static Vector3 ReflectPointOverPlane(Vector3 point, Plane plane)
            {
                Vector3 reflectPoint = plane.ClosestPointOnPlane(point);
                Vector3 mirrorLine = reflectPoint - point;
                return point + (mirrorLine * 2);
            }

            public void UpdateMesh ()
            {
                if (generated)
                    ClearMesh();
                if(extrusionStyle != ExtrusionStyle.AlongSpline || path != null)
                    CreateMesh();
            }

            void ClearMesh()
            {
                Mesh.Clear();
                generated = false;
            }

            void CreateMesh()
            {
                (Vector3[] vertices, Vector2[] UVs, int[] triangles) MeshData ()
                {
                    Vector3[] verts = new Vector3[0];
                    Vector2[] uvs = new Vector2[verts.Length];
                    List<int> tris = new List<int>();
                    switch (extrusionStyle)
                    {
                        case ExtrusionStyle.Direct:
                            List<Vector3> start, end;
                            start = spline.GetAllCurvePoints().ToList();
                            end = spline.GetAllCurvePoints(directDirection.normalized * extrusionDistance).ToList();
                            start.AddRange(end);
                            verts = start.ToArray();
                            uvs = new Vector2[verts.Length];
                            int add = end.Count;
                            for (int i = 0; i < end.Count; i++)
                            {
                                int second = i + 1;
                                if (second >= end.Count)
                                    second = 0;
                                if (flippedNormals)
                                {
                                    if (i + 1 < end.Count || spline.ClosedLoop)
                                        tris.AddRange(new int[] { second, i + add, i, second + add, i + add, second});
                                } else
                                {
                                    if (i + 1 < end.Count || spline.ClosedLoop)
                                        tris.AddRange(new int[] { i, i + add, second, second, i + add, second + add });
                                }
                                if (i + 1 < start.Count || spline.ClosedLoop)
                                {
                                    uvs[i] = new Vector2(0f, 1f - ((1f / end.Count) * i));
                                    try
                                    {
                                        uvs[second + add] = new Vector2(1f, 1f - ((1f / end.Count) * i));
                                    } catch (Exception e)
                                    {
                                        Debug.LogError(e + "_-_ (" + (second + add) +" / " + end.Count + ")");
                                    }
                                }
                            }

                            return (verts, uvs, tris.ToArray());

                        case ExtrusionStyle.AlongSpline:

                            Helpers.SplineGhost splineGhost = Helpers.SplineGhost.Create(spline, path, extrusionOrigin);
                            Helpers.SplineGhost.SplineGhostPath splineGhostPath = splineGhost.CalculatePath();
                            
                            int segments = splineGhostPath.Segments;
                            int segmentVerts = splineGhostPath.SegmentVerts;


                            List<Vector3> points = splineGhostPath.AllPositions;
                        
                            List<Vector2> pathUVs = new List<Vector2>();
                            //if (!Application.isPlaying)
                            splineGhost.DestroyWhenReady();
                            /*else
                                Destroy(splineGhost.gameObject);
                            */

                            for(int i = 0; i < segments; i++)
                            {
                                /*int secondSegment = i + 1;
                                if (secondSegment >= segments)
                                    secondSegment = 0;*/
                                for (int z = 0; z < segmentVerts; z++)
                                {
                                    int vert = segmentVerts * i + z;
                                    int secondVert = z + 1;
                                    if (secondVert >= segmentVerts)
                                        secondVert = 0 + (segmentVerts * i);
                                    else
                                        secondVert += (segmentVerts * i);

                                    pathUVs.Add(new Vector2(1f / segments * i, 1f - ((1f / segmentVerts) * z)));
                                    if (secondVert == 0 && !spline.ClosedLoop)
                                        continue;
                                    if (i < segments - 1 /*|| splineGhost.closedLoop*/)
                                    {
                                        if (flippedNormals)
                                        {
                                            tris.AddRange(new int[] { secondVert, vert + segmentVerts, vert, secondVert + segmentVerts, vert + segmentVerts, secondVert });
                                        }
                                        else
                                        {
                                            tris.AddRange(new int[] { vert, vert + segmentVerts, secondVert, secondVert, vert + segmentVerts, secondVert + segmentVerts });
                                        }
                                    }
                                }
                            }

                            /*for (int i = 0; i < points.Count; i++)
                            {
                                //points[i] = transform.TransformPoint(points[i]);
                                Vector3 p = new Vector3(points[i].x*-1, points[i].y, points[i].z);
                                points[i] = p;
                            }*/
                            return (points.ToArray(), pathUVs.ToArray(), tris.ToArray());

                        default: throw new NotImplementedException();
                    } 
                }
                Mesh eMesh = Mesh;
                var meshData = MeshData();
                eMesh.vertices = meshData.vertices;

                eMesh.uv = meshData.UVs;
                eMesh.triangles = meshData.triangles;
                eMesh.RecalculateNormals();
                Mesh = eMesh;

                material = meshMaterial;

                Mesh.MarkDynamic();
                generated = true;
            }

            public enum ExtrusionOrigin
            {
                Spline, Path
            }

            public enum ExtrusionStyle
            {
                Direct, Mirror, AlongSpline
            }

            public enum TextureMode
            {
                Strecthed, Full, Tiled
            }
        }
    }
}