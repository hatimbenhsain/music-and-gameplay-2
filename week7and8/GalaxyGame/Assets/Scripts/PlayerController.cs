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

    
    private int homeNote=54;
    private int prevNote;
    private int noteRange=12;

    private float ogOSCVol;
    private float ogSubVol;
    private float ogNoiseVol;

    private bool reachedPeak=false;

    void Start()
    {
        body=GetComponent<Rigidbody>();
        moveVector=Vector3.zero;
        camera=Camera.main.gameObject;
        fauxGravityBody=GetComponent<FauxGravityBody>();

        ogOSCVol=helmController.GetParameterValue(AudioHelm.Param.kOsc2Volume);
        ogSubVol=helmController.GetParameterValue(AudioHelm.Param.kSubVolume);
        ogNoiseVol=helmController.GetParameterValue(AudioHelm.Param.kNoiseVolume);

        ogOSCVol=0.5f;
        ogNoiseVol=0.2f;
        ogSubVol=0.2f;

        prevNote=homeNote;

    }

    // Update is called once per frame
    void Update()
    {
        if(!playedNote & Time.time>1f){
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
            body.velocity=transform.up*jumpForce;
            Debug.Log("jump");
        }

        if(Input.GetKey(KeyCode.Space) && transform.InverseTransformDirection(body.velocity).y>0){
            Debug.Log("pressing space while going up");
            fauxGravityBody.halveGravity=true;
        }else{
            fauxGravityBody.halveGravity=false;
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
        //helmController.SetParameterPercent(AudioHelm.Param.kVolume,vol);
        helmController.SetParameterPercent(AudioHelm.Param.kOsc2Volume,ogOSCVol*vol);
        helmController.SetParameterPercent(AudioHelm.Param.kSubVolume,ogSubVol*vol);
        helmController.SetParameterPercent(AudioHelm.Param.kNoiseVolume,ogNoiseVol+(1-vol)*(1-ogNoiseVol));

        if(!reachedPeak && dis>=maxHeight){
            reachedPeak=true;
            Debug.Log("reached peak");
            helmController.NoteOff(note);
            note=48+Mathf.FloorToInt(Random.Range(0f,12f));
            helmController.NoteOn(note);
        }

        float yAngle=Vector3.Angle(Vector3.up,transform.position-fauxGravityBody.attractor.transform.position);
        // Debug.Log(yAngle);
        // int n=Mathf.RoundToInt(yAngle*(noteRange)/180);
        // n=homeNote+n;
        // if(n!=note){
        //     helmController.NoteOff(note);
        //     helmController.NoteOn(n,1f);
        //     note=n;
        // }
        helmController.SetParameterPercent(AudioHelm.Param.kOsc2Tune,yAngle/180);
        helmController.SetParameterPercent(AudioHelm.Param.kOsc1Tune,yAngle/180);
        helmController.SetParameterPercent(AudioHelm.Param.kOscFeedbackTranspose,yAngle/180);

        float xAngle=Vector3.Angle(Vector3.forward,transform.position-fauxGravityBody.attractor.transform.position); //calculate this and associate it to osc1 volume
        helmController.SetParameterPercent(AudioHelm.Param.kOsc1Volume,xAngle*0.5f/180f);
        // float v=helmController.GetParameterValue(AudioHelm.Param.kOsc2Volume);
        // v=(1-xAngle/180f)*v;
        // helmController.SetParameterPercent(AudioHelm.Param.kOsc2Volume,ogOSCVol*v);

//        helmController.SetParameterValue(AudioHelm.Param.kPolyLfoFrequency,xAngle/180f);

        float camVal=(camera.transform.localPosition-cameraFarTransform.transform.localPosition).magnitude/
        (cameraCloseTransform.transform.localPosition-cameraFarTransform.transform.localPosition).magnitude;

        helmController.SetParameterPercent(AudioHelm.Param.kSubVolume,vol*(ogSubVol+camVal*(0.6f-ogSubVol)));
        helmController.SetParameterPercent(AudioHelm.Param.kOsc2Volume,vol*(ogOSCVol-camVal*(ogOSCVol)));
        
    }

    void FixedUpdate() {
        body.MovePosition(body.position+transform.TransformDirection(moveVector)*Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag=="Planet"){
            grounded=true;
        }
        reachedPeak=false;
    }

    private void OnCollisionExit(Collision other) {
        if(other.gameObject.tag=="Planet"){
            grounded=false;
        }
    }
}
