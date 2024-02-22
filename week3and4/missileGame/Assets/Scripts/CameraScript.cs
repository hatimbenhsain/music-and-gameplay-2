using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 rot=transform.rotation.eulerAngles;
        //transform.rotation=Quaternion.Euler(rot.x,rot.y,0f);
        transform.position=target.position;
        Vector3 rot=target.rotation.eulerAngles;
        transform.rotation=Quaternion.Euler(rot.x,rot.y,0f);
    }
}
