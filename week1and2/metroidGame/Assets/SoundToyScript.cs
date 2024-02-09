using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using JetBrains.Annotations;

public class SoundToyScript : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    public string parameterName;
    // Start is called before the first frame update

    public bool playing=false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        playing=!playing;
        if(playing){
            FindObjectOfType<StudioEventEmitter>().EventInstance.setParameterByName(parameterName,1f);
            Debug.Log(parameterName+" is 1");
        }else{
            FindObjectOfType<StudioEventEmitter>().EventInstance.setParameterByName(parameterName,0f);
            Debug.Log(parameterName+" is 0");
        }
        float val;
        FindObjectOfType<StudioEventEmitter>().EventInstance.getParameterByName(parameterName,out val);
        Debug.Log(val);
    }
}
