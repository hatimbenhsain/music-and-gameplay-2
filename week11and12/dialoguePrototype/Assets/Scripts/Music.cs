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
    private EventInstance pianoTrack;
    private EventInstance bassTrack;
    private EventInstance saxTrack;

    private string[] drumParameters;

    private string[] jazzParameters;
    // Start is called before the first frame update
    void Start()
    {
        bgMusic=new EventInstance[4];
        bgMusic[0]=FMODUnity.RuntimeManager.CreateInstance("event:/BGMusic 1");
        bgMusic[1]=FMODUnity.RuntimeManager.CreateInstance("event:/BGMusic 2");
        bgMusic[2]=FMODUnity.RuntimeManager.CreateInstance("event:/BGMusic 3");
        bgMusic[3]=FMODUnity.RuntimeManager.CreateInstance("event:/BGMusic 4");
    
        drumTrack=FMODUnity.RuntimeManager.CreateInstance("event:/DrumLine");
        pianoTrack=FMODUnity.RuntimeManager.CreateInstance("event:/PianoLine");
        bassTrack=FMODUnity.RuntimeManager.CreateInstance("event:/BassLine");
        saxTrack=FMODUnity.RuntimeManager.CreateInstance("event:/SaxLine");

        drumParameters=new string[]{"bassDrum", "bassDrum2", "bassDrum6", "clap", "closedHiHat", "hiConga","lowConga","openHihat","openHihat2","snareDrum","snareDrum2","zap"};
        jazzParameters=new string[]{"1","2","3","4","5","6","7"};
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBgMusic(){
        currentBgMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentBgMusic=bgMusic[Random.Range(0,bgMusic.Length)];
        currentBgMusic.start();
    }

    public void PlayDrum(string p1,float pitch=0,string p2="",string p3="",string p4=""){
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
        if(p3!=""){
            drumTrack.setParameterByName(p3,1f);
        }
        if(p4!=""){
            drumTrack.setParameterByName(p4,1f);
        }
        drumTrack.setPitch(pitch);
    }

    public void PlayJazz(string instrument,string p1,float pitch=0,string p2="",string p3="",string p4=""){
        EventInstance inst=pianoTrack;
        switch(instrument){
            case "piano":
                inst=pianoTrack;
                break;
            case "bass":
                inst=bassTrack;
                break;
            case "sax":
                inst=saxTrack;
                break;
        }
        inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        inst.setTimelinePosition(0);
        inst.start();
        foreach(string p in jazzParameters){
            inst.setParameterByName(p,0f);
        }
        inst.setParameterByName(p1,1f);
        if(p2!=""){
            inst.setParameterByName(p2,1f);
        }
        if(p3!=""){
            inst.setParameterByName(p3,1f);
        }
        if(p4!=""){
            inst.setParameterByName(p4,1f);
        }
        inst.setPitch(pitch);
    }

}
