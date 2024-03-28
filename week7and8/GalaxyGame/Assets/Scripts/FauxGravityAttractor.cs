using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FauxGravityAttractor : MonoBehaviour
{
    // Start is called before the first frame update
    public float gravity=-10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attract(Transform body){
        Vector3 gravityUp=(body.position-transform.position).normalized;
        Vector3 bodyUp=body.up;

        body.GetComponent<Rigidbody>().AddForce(gravityUp*gravity);

        Quaternion targetRotation=Quaternion.FromToRotation(bodyUp,gravityUp)*body.rotation;
        body.rotation=Quaternion.Slerp(body.rotation,targetRotation,50*Time.deltaTime);
    }
}
