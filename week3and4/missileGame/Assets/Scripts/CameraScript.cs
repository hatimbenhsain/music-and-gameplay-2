using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public float positionSmoothing=1f;
    public float rotationSmoothing=1f;

    public bool smoothPosition=false;
    public bool rotateCamera=false;

    public bool freezeZRotation=true;

    private Vector3 velocity = Vector3.zero;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 rot=transform.rotation.eulerAngles;
        //transform.rotation=Quaternion.Euler(rot.x,rot.y,0f);
        if(smoothPosition){
            transform.position=Vector3.SmoothDamp(transform.position,target.position,ref velocity,positionSmoothing);
        }else{
            transform.position=target.position;
        }
        if(rotateCamera){
            Vector3 rot=target.rotation.eulerAngles;
            if(freezeZRotation){
                rot.z=0f;
            }
            transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.Euler(rot.x,rot.y,rot.z),rotationSmoothing*Time.deltaTime);
        }
    }
}
