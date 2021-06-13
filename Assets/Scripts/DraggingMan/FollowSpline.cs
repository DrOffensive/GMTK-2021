using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splines;

public class FollowSpline : MonoBehaviour
{

    [SerializeField] CurrentTrack track;
    [SerializeField] Transform doggo;
    [SerializeField] float speed;
    
    public float Speed { get => speed; set => speed = value; }

    float flatSpeed = 0f;

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
        var pointOnSpline = track.spline.GetPointOnSpline(Mathf.Clamp01(track.time));
        transform.position = pointOnSpline.point;
        transform.rotation = Quaternion.LookRotation(track.spline.Tangent(track.time) * (track.spline.BezierType == BezierType.Quadratic ? -1 : 1));
        doggo.rotation = Quaternion.LookRotation(transform.right);
    }

    // Update is called once per frame
    void Update()
    {        
        track.time = track.spline.MoveDistanceAlongSpline(track.time, speed/track.spline.SplineLength * Time.deltaTime);
        if (track.time > 1)
            track.time -= 1;
        SnapToPoint(track.time);
    }

    public void SwitchTheTrack(Spline _newSpline)
    {
        track.time = 0;
        track.spline = _newSpline;
    }
}
