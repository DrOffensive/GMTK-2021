using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Splines
{
    [ExecuteAlways]
    public class SplineHandle : MonoBehaviour
    {
        Vector3 position;
        event Action onMoved;

        public event Action OnMoved { add => onMoved += value; remove => onMoved -= value; }

        private void Start()
        {
            position = transform.position;
        }

        private void Update()
        {
            if(position != transform.position)
            {
                position = transform.position;
                onMoved?.Invoke();
            }
        }
    }
}