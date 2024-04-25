using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Music : MonoBehaviour
{
    private EventInstance[] bgMusic;
    private EventInstance currentBgMusic;

    private EventInstance drumTrack;

    private string[] drumParameters;
    // Start is called before the first frame update
    void Start()
    {
        bgMusic=new EventInstance[4];
        bgMusic[0]=FMODUnity.RuntimeManager.CreateInstance("event:/BGMusic 1");
        bgMusic[1]=FMODUnity.RuntimeManager.CreateInstance("event:/BGMusic 2");
        bgMusic[2]=FMODUnity.RuntimeManager.CreateInstance("event:/BGMusic 3");
        bgMusic[3]=FMODUnity.RuntimeManager.CreateInstance("event:/BGMusic 4");
    
        drumTrack=FMODUnity.RuntimeManager.CreateInstance("event:/DrumLine");

        drumParameters=new string[]{"bassDrum", "bassDrum2", "bassDrum6", "clap", "closedHiHat", "hiConga","lowConga","openHihat","openHihat2","snareDrum","snareDrum2","zap"};
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)){
            PlayBgMusic();
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            PlayDrum(drumParameters[Random.Range(0,drumParameters.Length)]);
        }
    }

    public void PlayBgMusic(){
        currentBgMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentBgMusic=bgMusic[Random.Range(0,bgMusic.Length)];
        currentBgMusic.start();
    }

    public void PlayDrum(string p1,string p2="",float pitch=0){
        drumTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        drumTrack.setTimelinePosition(0);
        drumTrack.start();
        foreach(string p in drumParameters){
            drumTrack.setParameterByName(p,0f);
        }
        drumTrack.setParameterByName(p1,1f);
        if(p2!=""){
            drumTrack.setParameterByName(p2,1f);
        }
        //drumTrack.setPitch(pitch);
    }

}
