using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class TaskObject : MonoBehaviour
{
    [SerializeField] GameObjectReference handObjectReference = null;
    [SerializeField] UnityEvent onPickupEvents = null;
    [SerializeField] UnityEvent onDropEvents = null;

    [Header("Throwing properties")]
    [SerializeField] AnimationCurve speedCurve = AnimationCurve.Linear(0f,1f,1f,1f);
    [SerializeField] AnimationCurve yAxisParaboleCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
    [SerializeField] float maxYOffset = 2f;
    [SerializeField] float timeToReachTarget = 1f;
    [SerializeField] Rigidbody objectRigidbody = null;
    Collider collider = null;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void PickUp() {
        transform.SetParent(handObjectReference.Value.transform, true); 
        transform.localPosition = Vector3.zero;        
        objectRigidbody.isKinematic = true;
        if(collider!= null)
        {
            collider.enabled = false;
        }
        onPickupEvents?.Invoke();
    }

    public void PutInPlace(Transform _target)
    {
        transform.parent = _target;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        objectRigidbody.velocity = Vector3.zero;
        objectRigidbody.isKinematic = true;
    }

    public void ThrowAtTarget(Transform _target)
    {       
        StartCoroutine(throwByLine(_target.position));
        onDropEvents?.Invoke();
    }

    public void PutItBack()
    {
        StartCoroutine(PutBackAfter(timeToReachTarget * 1.5f));
    }

    private IEnumerator PutBackAfter(float _time)
    {
        yield return new WaitForSeconds(_time);
        onDropEvents?.Invoke();
    }


    public void ThrowRandomlyAtTarget(Transform _target)
    {
        onDropEvents?.Invoke();
        float _xOffset = Random.Range(3f, 5f);
        _xOffset *= Random.Range(0f, 1f) < 0.5 ? -1 : 1;

        Vector3 _newTargetposition = _target.localPosition;
        _newTargetposition.x += _xOffset;
        _newTargetposition = _target.localToWorldMatrix * _newTargetposition;

        StartCoroutine(throwByLine(_newTargetposition));
    }

    private IEnumerator throwByLine(Vector3 _target)
    {
        float _time = 0f;
        Vector3 _lastFramePositionChange = Vector3.zero;
        Vector3 _initialPosition = transform.position;
        Vector3 _oldPosition = transform.position;
        Vector3 _newPosition = transform.position;
        transform.parent = null;
        objectRigidbody.isKinematic = true;

        while (_time <= 1f)
        {
            _oldPosition = _newPosition;
            _time += Time.deltaTime / timeToReachTarget;
            _newPosition.x = Mathf.Lerp(_initialPosition.x, _target.x, _time);
            _newPosition.z = Mathf.Lerp(_initialPosition.z, _target.z, _time);
            _newPosition.y = (_initialPosition.y + yAxisParaboleCurve.Evaluate(_time) * maxYOffset) * (1 - _time) + (_target.y * _time);
            transform.position = _newPosition;
            _lastFramePositionChange = _newPosition - _oldPosition;
            yield return null;
        }

        objectRigidbody.velocity = _lastFramePositionChange;
        objectRigidbody.isKinematic = false;        
    }

}
