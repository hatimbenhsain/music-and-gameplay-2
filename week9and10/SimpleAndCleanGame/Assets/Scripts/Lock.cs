using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public float minForce=1f;
    public GameObject starsPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag=="Sword"){
            float force=other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            if(force>=minForce){
                FindObjectOfType<Music>().PlayCrash();
                Instantiate(starsPrefab,other.contacts[0].point,Quaternion.Euler(0,0,0));
                FindObjectOfType<Sword>().locksDestroyed++;
                Destroy(gameObject);
            }
        }
    }
}
