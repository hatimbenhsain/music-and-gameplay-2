using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;
using System.Diagnostics.Tracing;
using MidiParser;

public class ButterflyScript : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;
    TornadoScript tornadoScript;

    public Color dullColor;
    public Color baseColor;
    public Color glowColor;

    public Material wingMaterial;

    Animator animator;

    public StudioEventEmitter wingFlapEmitter;
    public EventInstance wingFlapInstance;

    private EventInstance prevChord;
    private EventInstance currentChord;

    private int flapCount;

    private string[] chords={"Chord 1","Chord 2","Chord 3","Chord 4"};

    private string[][] instrumentParameters;

    
    private float[] tornadoTresholds={1f,2f,5f,10f};

    private float drumVolumeTreshold=10f;
    private float drumMinimumPeriod=0.6f;

    private float celestaTreshold=0.4f;

    private List<MidiEvent> notes;
    private List<Coroutine> coroutines;
    private int ticksPerQuarterNote;

    private int prevTreshold;
    private string currentParameter;
    private string nextParameter;

    private bool firstChord=false;

    void Awake()
    {
        inputManager=GetComponent<InputManager>();
        playerLocomotion=GetComponent<PlayerLocomotion>();
        cameraManager=FindObjectOfType<CameraManager>();
        tornadoScript=FindObjectOfType<TornadoScript>();
        animator=GetComponentInChildren<Animator>();

        wingFlapInstance=wingFlapEmitter.EventInstance;
        flapCount=0;

        ParseMidi();
        coroutines=new List<Coroutine>();

        instrumentParameters=new string[tornadoTresholds.Length+1][];
        string[] t1={"Harp","Mark"};
        string[] t2={"Strings","Brass"};
        string[] t3={"Choir"};
        string[] t4={"Choir","Timpani"};
        instrumentParameters[0]=new string[0];
        instrumentParameters[1]=t1;
        instrumentParameters[2]=t2;
        instrumentParameters[3]=t3;
        instrumentParameters[4]=t4;

        prevTreshold=0;
        currentParameter="";
        nextParameter="";
    }

    // Update is called once per frame
    void Update()
    {
        inputManager.HandleAllInputs();
        animator.SetBool("diving",(playerLocomotion.diving && !playerLocomotion.jumping));

        float v=Mathf.Abs(tornadoScript.period-tornadoScript.avgPeriod);
        if(v<=tornadoScript.periodTreshold1){
            v=1f;
        }else if(v<=tornadoScript.periodTreshold2*2){
            v=1-(v-tornadoScript.periodTreshold1)/(tornadoScript.periodTreshold2*2-tornadoScript.periodTreshold1);
        }else{
            v=0f;
        }
        if(tornadoScript.avgPeriod<tornadoScript.idealPeriod/2){
            v=Mathf.Pow(v*2*(tornadoScript.idealPeriod/2-tornadoScript.avgPeriod)/tornadoScript.idealPeriod,2f);
        }
        Color tColor=Color.Lerp(dullColor,baseColor,v);
        wingMaterial.color=Color.Lerp(wingMaterial.color,tColor,Time.deltaTime*10f);
    }

    private void FixedUpdate() {
        playerLocomotion.speedModifier=Mathf.Pow(tornadoScript.currentRadius,0.4f);
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate() {
        cameraManager.HandleAllCameraMovement();
    }

    public void Jump(float y){
        tornadoScript.Jumped(y);
        animator.SetTrigger("jump");
        // wingFlapInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // wingFlapInstance.start();
        wingFlapInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Wingflap");
        wingFlapInstance.start();
        wingFlapInstance.release(); 
        int maxFlap=2; //number of flaps before next chord play

        if(tornadoScript.period>2f && flapCount%maxFlap==1 && tornadoScript.currentRadius>tornadoTresholds[0]){
            flapCount++;
        }

        if(flapCount%maxFlap==0 && tornadoScript.currentRadius>tornadoTresholds[0]){
            if(!firstChord){
                flapCount=0;
                firstChord=true;
            }
            prevChord=currentChord;
            prevChord.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentChord=FMODUnity.RuntimeManager.CreateInstance("event:/"+chords[(int)Mathf.Floor(flapCount/maxFlap)%chords.Length]);
            currentChord.start();
            foreach(string[] s in instrumentParameters){
                if(s.Length>0){
                    foreach(string p in s){
                        RuntimeManager.StudioSystem.setParameterByName(p,0f);
                    }
                    RuntimeManager.StudioSystem.setParameterByName("Celesta",0f);
                }
            }
            int i=0;
            foreach(float t in tornadoTresholds){
                if(tornadoScript.currentRadius>t){
                    i++;
                }else{
                    break;
                }
            }

            int tempI=i;
            
            if(prevTreshold!=i){
                string[] sa=instrumentParameters[i];
                nextParameter=sa[(int)Mathf.Floor(Random.Range(0f,sa.Length))];
                i=prevTreshold;
                UnityEngine.Debug.Log("change instrument");
                if(prevTreshold==0){
                    flapCount--;
                }
            }else{
                currentParameter=nextParameter;
            }

            if(i>0){
                if(tornadoScript.period<celestaTreshold){
                    RuntimeManager.StudioSystem.setParameterByName("Celesta",1f);
                }else{
                    RuntimeManager.StudioSystem.setParameterByName(currentParameter,1f);
                    if(tornadoScript.currentRadius>tornadoTresholds[tornadoTresholds.Length-1]){
                        RuntimeManager.StudioSystem.setParameterByName("Timpani",1f);
                        RuntimeManager.StudioSystem.setParameterByName("Choir",1f);
                    }
                }
                currentChord.release();
            }

            prevTreshold=tempI;   

        }else if(tornadoScript.currentRadius<=tornadoTresholds[0]){
            prevTreshold=0;
        }

        if(tornadoScript.currentRadius>tornadoTresholds[0]){
            flapCount++;
        }

        
        if(tornadoScript.period>=drumMinimumPeriod){
            foreach(Coroutine c in coroutines){
                StopCoroutine(c);
            }
            coroutines=new List<Coroutine>();

            foreach(MidiEvent m in notes){
                float time=m.Time*60000/((60*2/tornadoScript.period)*ticksPerQuarterNote);
                var note = m.Note;
                var velocity = m.Velocity;
                string eventName="";
                switch(note){
                    case 55:
                        eventName="Drum 2";
                        break;
                    case 60:
                        eventName="Drum 1";
                        break;
                }
                
                coroutines.Add(StartCoroutine(CreateAction(time/1000,eventName)));
                
                if(tornadoScript.currentRadius>drumVolumeTreshold){
                    RuntimeManager.StudioSystem.setParameterByName("DrumVolume",1f);
                }else{
                    RuntimeManager.StudioSystem.setParameterByName("DrumVolume",0f);
                }
            }
        }else{
            RuntimeManager.StudioSystem.setParameterByName("DrumVolume",0f);
        }
    }

    void ParseMidi(){
        UnityEngine.Debug.Log("parse midi");
        MidiFile midiFile = new MidiFile("drumMidi.mid");

        // 0 = single-track, 1 = multi-track, 2 = multi-pattern
        var midiFileformat = midiFile.Format;

        // also known as pulses per quarter note
        ticksPerQuarterNote = midiFile.TicksPerQuarterNote;

        int bpm=80;

        notes=new List<MidiEvent>();

        foreach(var track in midiFile.Tracks){
            UnityEngine.Debug.Log(track);
            foreach(var midiEvent in track.MidiEvents){
                if(midiEvent.MidiEventType==MidiEventType.NoteOn){
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    notes.Add(midiEvent);
                }
            }

            foreach(var textEvent in track.TextEvents){
                if(textEvent.TextEventType==TextEventType.Lyric){
                    var time = textEvent.Time;
                    var text = textEvent.Value;
                }
            }    
        }
    }

    IEnumerator CreateAction(float t, string eventName)
	{
		yield return new WaitForSeconds(t);

    	EventInstance instance = FMODUnity.RuntimeManager.CreateInstance("event:/"+eventName);
        instance.start();
        instance.release(); 
        
	}

}
