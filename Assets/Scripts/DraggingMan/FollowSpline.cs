using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splines;

public class FollowSpline : MonoBehaviour
{

    [SerializeField] CurrentTrack track;

    [SerializeField] float speed;

    [System.Serializable]
    public struct CurrentTrack
    {
        public Spline spline;
        public float time { get; set; }
    }


    private void Start()
    {
        SnapToPoint(0);
    }

    // Start is called before the first frame update
    void SnapToPoint(float point)
    {
        var pointOnSpline = track.spline.GetPointOnSpline(track.time);
        transform.position = pointOnSpline.point;
        transform.rotation = Quaternion.LookRotation(track.spline.Tangent(track.time));
    }

    // Update is called once per frame
    void Update()
    {
        track.time = track.spline.MoveDistanceAlongSpline(track.time, speed * Time.deltaTime);
        if (track.time > 1)
            track.time -= 1;
        SnapToPoint(track.time);
    }
}
