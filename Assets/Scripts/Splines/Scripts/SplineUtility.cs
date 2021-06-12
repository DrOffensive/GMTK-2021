using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Splines
{
    public static class SplineUtility
    {
        const string EMPTY = "Splines/Empty";

        public static GameObject GetEmpty ()
        {
            return Resources.Load<GameObject>(EMPTY);
        }

        public static float OnCircle(float degree)
        {
            if (degree < 0)
            {
                return OnCircle(360 - degree);
            }
            else if (degree > 360)
            {
                return OnCircle(degree - 360);
            }
            else
            {
                return degree;
            }

        }

        public static Vector3 GetLinearBezierPoint(float time, Vector3 point0, Vector3 point1)
        {
            time = Mathf.Clamp01(time);
            return point0 + time * (point1 - point0);
        }

        public static Vector3 GetQuadraticBezierPoint(float time, Vector3 point0, Vector3 point1, Vector3 point2)
        {
            time = Mathf.Clamp01(time);
            return Mathf.Pow(1f - time, 2) * point0 + 2 * (1 - time) * time * point1 + Mathf.Pow(time, 2) * point2;
        }

        public static Vector3 GetCubicBezierPoint(float time, Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
        {
            time = Mathf.Clamp01(time);
            return Mathf.Pow(1f - time, 3) * point0 + 3 * Mathf.Pow(1 - time, 2) * time * point1 + 3 * (1 - time) * Mathf.Pow(time, 2) * point2 + Mathf.Pow(time, 3) * point3;
            //B(t) = (1-t)3P0 + 3(1-t)2tP1 + 3(1-t)t2P2 + t3P3 , 0 < t < 1 
        }

        public static Vector3[] GetQuadraticBezierCurve(int points, Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint)
        {
            if (points > 1)
            {
                int p = points - 1;
                Vector3[] positions = new Vector3[points];
                float inc = 1f / p;
                for (int i = 0; i < p; i++)
                {
                    float t = inc * i;
                    positions[i] = GetQuadraticBezierPoint(t, startPoint, controlPoint, endPoint);
                }
                positions[p] = endPoint;
                return positions;
            }
            else
                return new Vector3[] { startPoint, controlPoint, endPoint };
        }

        public static Vector3[] GetCubicBezierCurve(int points, Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint1, Vector3 controlPoint2)
        {
            if (points > 1)
            {
                int p = points - 1;
                Vector3[] positions = new Vector3[points];
                float inc = 1f / p;
                for (int i = 0; i < p; i++)
                {
                    float t = inc * i;
                    positions[i] = GetCubicBezierPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
                }
                positions[p] = endPoint;
                return positions;
            }
            else
                return new Vector3[] { startPoint, controlPoint1, controlPoint2, endPoint };
        }


        public static Vector3[] GetLinearBezierCurve(int points, Vector3 startPoint, Vector3 endPoint)
        {
            if (points > 1)
            {
                int p = points - 1;
                Vector3[] positions = new Vector3[points];
                float inc = 1f / p;
                for (int i = 0; i < p; i++)
                {
                    float t = inc * i;
                    positions[i] = GetLinearBezierPoint(t, startPoint, endPoint);
                }
                positions[p] = endPoint;
                return positions;
            }
            else
                return new Vector3[] { startPoint, endPoint };
        }

        public static Vector3 GetQuadraticBezierTangentAtPoint(float time, Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint)
        {
            time = Mathf.Clamp01(time);
            Vector3 tangent = 2 * (1 - time) * (controlPoint - startPoint) + 2 * time * (endPoint - controlPoint);
            return tangent.normalized;
        }
        public static Vector3 GetCubicBezierTangentAtPoint(float t, Vector3 start, Vector3 handleA, Vector3 handleB, Vector3 end)
        {
            // note that abcd are aka x0 x1 x2 x3

            /*  the four coefficients ..
                A = x3 - 3 * x2 + 3 * x1 - x0
                B = 3 * x2 - 6 * x1 + 3 * x0
                C = 3 * x1 - 3 * x0
                D = x0

                and then...
                Vx = 3At2 + 2Bt + C         */

            // first calcuate what are usually know as the coeffients,
            // they are trivial based on the four control points:

            Vector3 C1 = (end - (3f * handleB) + (3f * handleA) - start);
            Vector3 C2 = ((3f * handleB) - (6f * handleA) + (3f * start));
            Vector3 C3 = ((3f * handleA) - (3f * start));
            Vector3 C4 = (start);  // (not needed for this cstartlculstarttion)

            // finstartlly it is estartsy to cstartlculstartte the slope element,
            // using those coefficients:

            return ((3f * C1 * t * t) + (2f * C2 * t) + C3);

            // note that this routine works for both the x and y side;
            // simply run this routine twice, once for x once for y
            // note that there are sometimes said to be 8 (not 4) coefficients,
            // these are simply the four for x and four for y,
            // calculated as above in each case.
        }
    }

    public enum BezierType
    {
        Linear, Quadratic, Cubic
    }

    public enum SplineOrigin
    {
        TransformOrigin, SplineCenter, FirstPoint, LastPoint, CustomPoint, CustomPosition 
    }
}