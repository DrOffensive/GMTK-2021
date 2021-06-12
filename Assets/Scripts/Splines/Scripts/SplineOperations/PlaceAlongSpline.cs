using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Splines.Operations
{
    [ExecuteAlways]
    public class PlaceAlongSpline : MonoBehaviour
    {
        [SerializeField] int amount = 1;
        [SerializeField] PlacementStyle placementStyle;
        [SerializeField] Spline path;
        [SerializeField] bool faceSplineDirection;
        [SerializeField] bool resetOnUpdate;
        [SerializeField] ParentingOption instanceParenting;
        List<GameObject> instances = new List<GameObject>();

        public enum ParentingOption
        {
            NoParent, OriginalParent
        }

        public enum PlacementStyle
        {
            Equal, FixedDistance
        }

        public void Place ()
        {
            if (path == null)
                return;

            if (amount < 1)
                amount = 1;

            if (resetOnUpdate && instances != null)
            {
                for (int i = 0; i < instances.Count; i++)
                {
                    if (Application.isEditor)
                        DestroyImmediate(instances[i]);
                    else
                        Destroy(instances[i]);
                }
                instances = new List<GameObject>();
            }
            else if (instances == null)
                instances = new List<GameObject>();

            GameObject parentObject = new GameObject($"{gameObject.name} - Placed along {path.name}");
            parentObject.transform.position = path.transform.position;
            switch (placementStyle)
            {
                case PlacementStyle.Equal:
                    if (amount == 1)
                    {
                        GameObject instance = Instantiate(gameObject);
                        instance.transform.position = path.GetPointOnSpline(.5f).point;
                        instances.Add(instance);
                        if (faceSplineDirection)
                            instance.transform.rotation = Quaternion.LookRotation(path.Tangent(.5f));
                        instance.transform.SetParent(parentObject.transform);
                    } else
                    {
                        float p = 1f / amount;
                        for (float i = p; i < 1f; i += p)
                        {
                            GameObject instance = Instantiate(gameObject);
                            instance.transform.position = path.GetPointOnSpline(i).point;
                            instances.Add(instance);
                            if (faceSplineDirection)
                                instance.transform.rotation = Quaternion.LookRotation(path.Tangent(i));
                            instance.transform.SetParent(parentObject.transform);
                            if (!Application.isPlaying)
                                DestroyImmediate(instance.GetComponent<PlaceAlongSpline>());
                            else
                                Destroy(instance.GetComponent<PlaceAlongSpline>());
                        }
                    }
                    if(instanceParenting==ParentingOption.OriginalParent)
                        parentObject.transform.SetParent(this.transform.parent);
                    instances.Add(parentObject);
                    break;
            }
        }
    }
}