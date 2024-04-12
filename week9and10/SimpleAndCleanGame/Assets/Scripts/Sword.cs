using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sword : MonoBehaviour
{
    public Transform targetTransform;

    public float moveSpeed=1f;
    public float rotationSpeed=1f;

    private Rigidbody rb;

    public float minVelocity=5f;
    private float lowVelocityTimer=0f;
    public float lowVelocityDuration=5f;

    private float timer=0f;
    public float startTime=10f;

    public GameObject[] thingsToDestroy;

    public int locksDestroyed;
    public int locksToDestroy=12;

    private bool destroyedObjects=false;

    private float endTimer=0f;



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
        if(rb.velocity.magnitude>minVelocity && timer>startTime){
            lowVelocityTimer=0f;
            RuntimeManager.StudioSystem.setParameterByName("voice",1f);
        }else{
            lowVelocityTimer+=Time.deltaTime;
            if(lowVelocityTimer>=lowVelocityDuration && lowVelocityTimer-Time.deltaTime<lowVelocityDuration){
                RuntimeManager.StudioSystem.setParameterByName("voice",0f);
            }
        }

        if(Input.GetKeyDown(KeyCode.R)){
            transform.position=targetTransform.position;
            transform.rotation=targetTransform.rotation;
            rb.velocity=Vector3.zero;
        }

        timer+=Time.deltaTime;

        if(locksDestroyed>=locksToDestroy && !destroyedObjects){
            foreach(GameObject go in thingsToDestroy){
                Destroy(go);
            }
            destroyedObjects=true;
            RuntimeManager.StudioSystem.setParameterByName("finalShift",1f);
            FindObjectOfType<Music>().end=true;
        }
        if(destroyedObjects){
            endTimer+=Time.deltaTime;
            Color c=new Color(0f,0f,0f,endTimer/60f);
            FindObjectOfType<Image>().color=c;
        }
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("collision enter");
    }

}
