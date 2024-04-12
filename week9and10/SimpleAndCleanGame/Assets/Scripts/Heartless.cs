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
    public float stunDuration=5f;
    private float halfStunDuration;

    public float minForce=1f;

    public float maxSpeed=1f;

    public GameObject starsPrefab;
    void Start()
    {
        rb=GetComponent<Rigidbody>();
        halfStunDuration=stunDuration/2f;
        stunTime=5f;
        target=GameObject.Find("PlayerCapsule").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(stunTime<stunDuration && stunTime>=halfStunDuration){
            Quaternion ogRotation=transform.rotation;
            transform.LookAt(target.position);
            float k=(stunTime-halfStunDuration)/(stunDuration-halfStunDuration);
            transform.rotation=Quaternion.Lerp(ogRotation,transform.rotation,k);
        }else if(stunTime>halfStunDuration){
            transform.LookAt(target.position);
        }
        stunTime+=Time.deltaTime;
    }

    void FixedUpdate(){
        halfStunDuration=stunDuration/2f;
        Vector3 positionDifference=target.position-transform.position;
        float time=positionDifference.magnitude/moveSpeed;
        //rb.velocity=positionDifference/Mathf.Max(time,Time.fixedDeltaTime);
        float k=1;
        if(stunTime<stunDuration){
            k=0;
        }else if(stunTime<halfStunDuration){
            k=(stunTime-halfStunDuration)/(stunDuration-halfStunDuration);
        }
        float d1=Vector3.Distance(target.position,transform.position);
        float d2=Vector3.Distance(target.position,transform.position+rb.velocity*Time.deltaTime);
        if(d2<d1){
            if(rb.velocity.magnitude>maxSpeed){
                positionDifference=Vector3.zero;
            }
        }

        rb.AddForce(positionDifference*moveSpeed*Time.deltaTime*k,ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag=="Sword" && stunTime>halfStunDuration){
            float force=other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            if(force>=minForce){
                stunTime=0f;
                Instantiate(starsPrefab,other.contacts[0].point,Quaternion.Euler(0,0,0));
            }
            FindObjectOfType<Music>().PlayStinger();
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag=="TriggerSphere"){
            Destroy(gameObject);
        }
    }
}
