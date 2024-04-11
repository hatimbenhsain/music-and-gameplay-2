using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Transform targetTransform;

    public float moveSpeed=1f;
    public float rotationSpeed=1f;

    private Rigidbody rb;

    public float minVelocity=5f;
    private float lowVelocityTimer=0f;
    public float lowVelocityDuration=5f;

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

    private void Update() {
        if(rb.velocity.magnitude>minVelocity){
            lowVelocityTimer=0f;
            FindObjectOfType<StudioEventEmitter>().SetParameter("voice",1f);
        }else{
            lowVelocityTimer+=Time.deltaTime;
            if(lowVelocityTimer>=lowVelocityDuration && lowVelocityTimer-Time.deltaTime<lowVelocityDuration){
                FindObjectOfType<StudioEventEmitter>().SetParameter("voice",0f);
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("collision enter");
    }
}
