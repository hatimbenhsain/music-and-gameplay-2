using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heartless : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;
    public float moveSpeed=1f;
    public float rotationSpeed=1f;

    public float stunTime;
    private float stunDuration;
    private float halfStunDuration;
    void Start()
    {
        rb=GetComponent<Rigidbody>();
        stunDuration=5f;
        stunTime=5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(stunTime>halfStunDuration);
        transform.LookAt(target.position);
        stunTime+=Time.deltaTime;
    }

    void FixedUpdate(){
        Vector3 positionDifference=target.position-transform.position;
        float time=positionDifference.magnitude/moveSpeed;
        //rb.velocity=positionDifference/Mathf.Max(time,Time.fixedDeltaTime);
        float k=1;
        if(stunTime<stunDuration){
            k=0;
        }else if(stunTime<halfStunDuration){
            k=(stunTime-halfStunDuration)/(stunDuration-halfStunDuration);
        }
        rb.AddForce(positionDifference*moveSpeed*Time.deltaTime*k,ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag=="Sword" && stunTime>halfStunDuration){
            stunTime=0f;
            Debug.Log("sword");
        }
    }
}
