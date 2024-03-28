using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed=0f;
    private Vector3 moveDir;
    private Rigidbody body;

    public float initSpeed=1f;
    public float acceleration=5f;
    public float maxSpeed=10f;
    public float deceleration=5f;

    public float jumpForce=5f;

    private Vector3 lastDir;
    private Vector3 moveVector;

    private bool grounded=true;
    private bool slow=false;

    public float slowFactor=0.5f;

    public Transform cameraCloseTransform;
    public Transform cameraFarTransform;

    private GameObject camera;
    public AudioHelm.HelmController helmController;

    private int note=60;

    private bool playedNote=false;

    private float initHeight=10f;
    private float maxHeight=20f;

    private FauxGravityBody fauxGravityBody;

    void Start()
    {
        body=GetComponent<Rigidbody>();
        moveVector=Vector3.zero;
        camera=Camera.main.gameObject;
        fauxGravityBody=GetComponent<FauxGravityBody>();

    }

    // Update is called once per frame
    void Update()
    {
        if(!playedNote){
            helmController.NoteOn(note,1f);
            playedNote=true;
        }

        slow=Input.GetMouseButton(1);


        moveDir=new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")).normalized;

        float acc=acceleration;
        float ms=maxSpeed;
        float dec=deceleration;

        if(slow){
            float k=slowFactor;
            acc=acc*k;
            ms=ms*k;
            dec=dec*k;
        }

        if(moveDir.magnitude>0f){
            if(moveVector.magnitude<0.05f){
                moveVector=moveDir*initSpeed;
            }else{
                moveVector+=moveDir*acc*Time.deltaTime;
            }
        }
        if(moveDir.magnitude==0f || moveVector.magnitude>moveDir.magnitude*ms){
            if(moveVector.magnitude>0f){
                moveVector-=dec*moveVector.normalized*Time.deltaTime;
            }
        }

        moveVector=Vector3.ClampMagnitude(moveVector,ms);

        if(Input.GetKeyDown(KeyCode.Space) && grounded){
            body.AddForce(transform.up*jumpForce);
            Debug.Log("jump");
        }

        // if(moveDir.magnitude==0f || moveSpeed>moveDir.magnitude*maxSpeed){
        //     if(moveSpeed>0f){
        //         moveSpeed-=deceleration*Time.deltaTime;
        //     }
        // }else{
        //     moveSpeed+=acceleration*Time.deltaTime;
        // }

        // moveSpeed=Mathf.Clamp(moveSpeed,0f,maxSpeed);

        // if(moveDir.magnitude>0f){
        //     lastDir=moveDir;
        // }else if(moveSpeed>0f){

        // }else{
        //     lastDir=moveDir;
        // }

        float cameraLerp=10f;
        if(slow){
            camera.transform.localPosition=Vector3.Lerp(camera.transform.localPosition,cameraCloseTransform.transform.localPosition,cameraLerp*Time.deltaTime);
        }else{
            camera.transform.localPosition=Vector3.Lerp(camera.transform.localPosition,cameraFarTransform.transform.localPosition,cameraLerp*Time.deltaTime);
        }

        float dis=Vector3.Distance(transform.position,fauxGravityBody.attractor.transform.position);
        float vol=Mathf.Clamp(1-(dis-initHeight)/(maxHeight-initHeight),0f,1f);
        helmController.SetParameterPercent(AudioHelm.Param.kVolume,vol);
    }

    void FixedUpdate() {
        body.MovePosition(body.position+transform.TransformDirection(moveVector)*Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag=="Planet"){
            grounded=true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if(other.gameObject.tag=="Planet"){
            grounded=false;
        }
    }
}
