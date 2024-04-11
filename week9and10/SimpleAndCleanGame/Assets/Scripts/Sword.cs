using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Transform targetTransform;

    public float moveSpeed=1f;
    public float rotationSpeed=1f;

    private Rigidbody rb;

    void Start()
    {
        rb=GetComponentInChildren<Rigidbody>();
    }

    private void FixedUpdate() {
        Vector3 positionDifference=targetTransform.position-transform.position;
        float time=positionDifference.magnitude/moveSpeed;
        rb.velocity=positionDifference/Mathf.Max(time,Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.RotateTowards(targetTransform.rotation,transform.rotation,rotationSpeed*Time.fixedDeltaTime));
    }
}
