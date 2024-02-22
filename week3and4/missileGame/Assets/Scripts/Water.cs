using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager=FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            audioManager.SetLoop(2);
        }
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject.tag=="Player"){
            audioManager.SetLoop(3);
        }
    }
}
