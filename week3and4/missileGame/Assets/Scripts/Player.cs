using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool on=false;
    public ParticleSystem particleSystem;

    private Rigidbody body;

    public float jetForce=1f;

    public float rotateSpeed=1f;
    
    private CameraScript cameraScript;

    // Start is called before the first frame update
    void Start()
    {
        body=GetComponentInChildren<Rigidbody>();
        cameraScript=FindObjectOfType<CameraScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            on=true;
            particleSystem.gameObject.SetActive(true);
        }

        if(Input.GetKeyUp(KeyCode.Space)){
            on=false;
            particleSystem.gameObject.SetActive(false);
        }

        if(on){
            body.AddForce(jetForce*Time.deltaTime*transform.forward,ForceMode.Impulse);
        }

        //Vector3 d=Vector3.zero;
        

        if(Input.GetKey(KeyCode.A)){
            //d+=-transform.right;
            transform.Rotate(0f,-rotateSpeed*Time.deltaTime,0f,Space.World);
            //transform.RotateAround(Vector3.up,transform.position,rotateSpeed*Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D)){
            //d+=transform.right;
            transform.Rotate(0f,rotateSpeed*Time.deltaTime,0f,Space.World);
            //transform.RotateAround(Vector3.up,transform.position,-rotateSpeed*Time.deltaTime);
        }

        Vector3 newD=transform.forward;

        if(Input.GetKey(KeyCode.W)){
            //d+=Vector3.up;
            //transform.Rotate(-rotateSpeed*Time.deltaTime,0f,0f,Space.Self);
            newD=Vector3.RotateTowards(transform.forward,Vector3.up,
            rotateSpeed*Time.deltaTime*Mathf.PI/180,0f);
        }
        if(Input.GetKey(KeyCode.S)){
            //d+=Vector3.down;
            //transform.Rotate(rotateSpeed*Time.deltaTime,0f,0f,Space.Self);
            newD=Vector3.RotateTowards(transform.forward,Vector3.down,
            rotateSpeed*Time.deltaTime*Mathf.PI/180,0f);
        }

        transform.rotation = Quaternion.LookRotation(newD);

        //if(d!=transform.forward){
            //transform.rotation=Quaternion.Euler(Vector3.RotateTowards(
            //    transform.rotation.eulerAngles,d,rotateSpeed*Time.deltaTime,0f));
            //transform.Rotate(transform.forward,rotateSpeed*Time.deltaTime);
            //transform.rotation=Quaternion.Euler(transform.rotation.eulerAngles+d*rotateSpeed*Time.deltaTime);
        //}
        //transform.rotation=Quaternion.Euler(Vector3.RotateTowards(
        //        transform.forward,Vector3.right,rotateSpeed*Time.deltaTime,0f));

    }
}
