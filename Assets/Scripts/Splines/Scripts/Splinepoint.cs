using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Splines
{
    [ExecuteAlways]
    public class Splinepoint : MonoBehaviour
    {
        Vector3 oldPosition;
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        public Vector3 LocalPosition { get => transform.localPosition; set => transform.localPosition = value; }

        Transform handle;

        event Action onMoved;

        public event Action OnMoved { add => onMoved += value; remove => onMoved -= value; }

        private void Start()
        {
            oldPosition = Position;
        }

        public Transform Handle
        {
            get
            {
                if (handle == null)
                {
                    if (GetComponentInChildren<SplineHandle>())
                        handle = GetComponentInChildren<SplineHandle>().transform;
                    else
                        CreateHandle();
                }
                return handle;
            }
        }

        public Vector3 HandleA
        {
            get
            {
                return Handle.position;
            }
        }

        public Vector3 HandleB
        {
            get
            {
                Vector3 line = Position - HandleA;
                return Position + line;
            }
        }

        void CreateHandle ()
        {
            handle = new GameObject("Handle").transform;
            handle.position = Position;
            SplineHandle splinehandle = handle.gameObject.AddComponent<SplineHandle>();
            splinehandle.OnMoved += transform.parent.GetComponent<Spline>().PointMoved;
            handle.SetParent(this.transform);
        }

        public static Splinepoint CreateNew (Vector3 position, Transform parent)
        {
            GameObject point = new GameObject("splinepoint");
            Splinepoint splinepoint = point.AddComponent<Splinepoint>();
            point.transform.position = position;
            point.transform.SetParent(parent);
            splinepoint.onMoved += parent.GetComponent<Spline>().PointMoved;
            return splinepoint;
        }

        private void Update()
        {
            if(oldPosition != Position)
            {
                oldPosition = Position;
                onMoved?.Invoke();
            }
        }
    }
}