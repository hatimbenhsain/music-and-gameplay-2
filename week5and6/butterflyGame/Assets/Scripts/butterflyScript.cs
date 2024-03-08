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

    private string[] chords={"Chord 1","Chord 2","Chord 3","Chord 4","Chord 5","Chord 6","Chord 7","Chord 8","Chord 9","Chord 10","Chord 11","Chord 12","Chord 13"
    ,"Chord 14","Chord 15","Chord 16","Chord 17","Chord 18","Chord 19","Chord 20"};

    private string[] drums={"Tabla","Conga","Bongo",};   //"Shaker","Stick"

    private string currentDrum="";
    private string nextDrum="";

    private string[][] instrumentParameters;

    
    public float[] tornadoTresholds={1f,10f,100f,1000f};

    public float drumVolumeTreshold=10f;
    public float malletVolumeTreshold=10f;
    private float drumMinimumPeriod=0.6f;

    private float malletMinimumPeriod=0.6f;

    private float celestaTreshold=0.4f;

    private List<MidiEvent> drumNotes;
    private List<MidiEvent> malletNotes;
    private List<MidiEvent> melody1Notes;
    private List<MidiEvent> melody2Notes;
    private List<MidiEvent> melody3Notes;
    private List<MidiEvent> melody4Notes;
    private List<Coroutine> drumCoroutines;
    private List<Coroutine> malletCoroutines;
    private int ticksPerQuarterNote;

    private int prevTreshold;
    private string currentParameter;
    private string nextParameter;

    private bool firstChord=false;
    private EventInstance prevInstance;

    private bool playMelody=false;
    private bool playMallet=false;
    private bool playDrums=false;

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
        drumCoroutines=new List<Coroutine>();
        malletCoroutines=new List<Coroutine>();

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
                        currentChord.setParameterByName(p,0f);
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
                string nd=drums[(int)Mathf.Floor(Random.Range(0f,drums.Length))];
                while(nd==currentDrum){
                    nd=drums[(int)Mathf.Floor(Random.Range(0f,drums.Length))];
                }
                nextDrum=nd;
                if(prevTreshold==0){
                    flapCount--;
                }
            }else{
                currentParameter=nextParameter;
                foreach(string d in drums){
                    RuntimeManager.StudioSystem.setParameterByName(d,0f);
                }
                RuntimeManager.StudioSystem.setParameterByName(nextDrum,1f);
                currentDrum=nextDrum;
            }
            RuntimeManager.StudioSystem.setParameterByName("Kalimba",1f);

            if(i>0){
                if(tornadoScript.period<celestaTreshold){
                    RuntimeManager.StudioSystem.setParameterByName("Celesta",1f);
                }else{
                    currentChord.setParameterByName(currentParameter,1f);
                    if(tornadoScript.currentRadius>tornadoTresholds[tornadoTresholds.Length-1]){
                        currentChord.setParameterByName("Timpani",1f);
                        currentChord.setParameterByName("Choir",1f);
                        playDrums=false;
                        playMelody=false;
                        playMallet=false;
                    }else if(tornadoScript.currentRadius>tornadoTresholds[tornadoTresholds.Length-2]){
                        playDrums=true;
                        playMelody=true;
                        playMallet=false;
                    }else if(tornadoScript.currentRadius>tornadoTresholds[tornadoTresholds.Length-3]){
                        playDrums=true;
                        playMelody=false;
                        playMallet=true;
                    }else{
                        playDrums=false;
                        playMelody=false;
                        playMallet=false;
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

        if(tornadoScript.period>=drumMinimumPeriod && playDrums){
            foreach(Coroutine c in drumCoroutines){
                StopCoroutine(c);
            }
            drumCoroutines=new List<Coroutine>();

            foreach(MidiEvent m in drumNotes){
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
                
                drumCoroutines.Add(StartCoroutine(CreateAction(time/1000,eventName)));
                
                if(tornadoScript.currentRadius>drumVolumeTreshold){
                    RuntimeManager.StudioSystem.setParameterByName("DrumVolume",1f);
                }else{
                    RuntimeManager.StudioSystem.setParameterByName("DrumVolume",0f);
                }
            }
        }else{
            RuntimeManager.StudioSystem.setParameterByName("DrumVolume",0f);
        }

        if(tornadoScript.period>=malletMinimumPeriod){
            foreach(Coroutine c in malletCoroutines){
                StopCoroutine(c);
            }
            malletCoroutines=new List<Coroutine>();

            List<MidiEvent> notes=malletNotes;

            if(playMelody){
                if(flapCount%4==0){
                    notes=melody1Notes;
                }else if(flapCount%4==1){
                    notes=melody2Notes;
                }else if(flapCount%4==2){
                    notes=melody3Notes;
                }else if(flapCount%4==3){
                    notes=melody4Notes;
                }
            }

            if(playMallet || playMelody){
                foreach(MidiEvent m in notes){
                    float time=m.Time*60000/((60*2/tornadoScript.period)*ticksPerQuarterNote);
                    var note = m.Note;
                    var velocity = m.Velocity;
                    string eventName="";
                    switch(note){
                        case 57:
                            eventName="Mallet 1";
                            break;
                        case 62:
                            eventName="Mallet 2";
                            break;
                        case 65:
                            eventName="Mallet 3";
                            break;
                        case 72:
                            eventName="Melody C1";
                            break;
                        case 75:
                            eventName="Melody Eb";
                            break;
                        case 79:
                            eventName="Melody G";
                            break;
                        case 81:
                            eventName="Melody A";
                            break;
                        case 82:
                            eventName="Melody Bb";
                            break;
                        case 84:
                            eventName="Melody C2";
                            break;
                    }
                    
                    malletCoroutines.Add(StartCoroutine(CreateAction(time/1000,eventName)));
                    
                    if(tornadoScript.currentRadius>malletVolumeTreshold){
                        RuntimeManager.StudioSystem.setParameterByName("MalletVolume",1f);
                        RuntimeManager.StudioSystem.setParameterByName("MelodyVolume",1f);
                    }else{
                        RuntimeManager.StudioSystem.setParameterByName("MalletVolume",0f);
                        RuntimeManager.StudioSystem.setParameterByName("MelodyVolume",0f);
                    }
                }
            }
        }else{
            RuntimeManager.StudioSystem.setParameterByName("MalletVolume",0f);
            RuntimeManager.StudioSystem.setParameterByName("MelodyVolume",0f);
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

        drumNotes=new List<MidiEvent>();

        foreach(var track in midiFile.Tracks){
            UnityEngine.Debug.Log(track);
            foreach(var midiEvent in track.MidiEvents){
                if(midiEvent.MidiEventType==MidiEventType.NoteOn){
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    drumNotes.Add(midiEvent);
                }
            }

            foreach(var textEvent in track.TextEvents){
                if(textEvent.TextEventType==TextEventType.Lyric){
                    var time = textEvent.Time;
                    var text = textEvent.Value;
                }
            }    
        }
        drumNotes.RemoveAt(drumNotes.Count/2);

        midiFile = new MidiFile("malletMidi.mid");

        // 0 = single-track, 1 = multi-track, 2 = multi-pattern
        midiFileformat = midiFile.Format;

        // also known as pulses per quarter note
        ticksPerQuarterNote = midiFile.TicksPerQuarterNote;

        bpm=80;

        malletNotes=new List<MidiEvent>();

        foreach(var track in midiFile.Tracks){
            UnityEngine.Debug.Log(track);
            foreach(var midiEvent in track.MidiEvents){
                if(midiEvent.MidiEventType==MidiEventType.NoteOn){
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    malletNotes.Add(midiEvent);
                }
            }

            foreach(var textEvent in track.TextEvents){
                if(textEvent.TextEventType==TextEventType.Lyric){
                    var time = textEvent.Time;
                    var text = textEvent.Value;
                }
            }    
        }
        malletNotes.RemoveAt(malletNotes.Count/2);


        melody1Notes=new List<MidiEvent>();
        midiFile = new MidiFile("melody1.mid");
        foreach(var track in midiFile.Tracks){
            UnityEngine.Debug.Log(track);
            foreach(var midiEvent in track.MidiEvents){
                if(midiEvent.MidiEventType==MidiEventType.NoteOn){
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    melody1Notes.Add(midiEvent);
                    UnityEngine.Debug.Log(note);
                }
            }
        }
        melody2Notes=new List<MidiEvent>();
        midiFile = new MidiFile("melody2.mid");
        foreach(var track in midiFile.Tracks){
            UnityEngine.Debug.Log(track);
            foreach(var midiEvent in track.MidiEvents){
                if(midiEvent.MidiEventType==MidiEventType.NoteOn){
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    melody2Notes.Add(midiEvent);
                    UnityEngine.Debug.Log(note);
                }
            }
        }
        melody3Notes=new List<MidiEvent>();
        midiFile = new MidiFile("melody3.mid");
        foreach(var track in midiFile.Tracks){
            UnityEngine.Debug.Log(track);
            foreach(var midiEvent in track.MidiEvents){
                if(midiEvent.MidiEventType==MidiEventType.NoteOn){
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    melody3Notes.Add(midiEvent);
                    UnityEngine.Debug.Log(note);
                }
            }
        }
        melody4Notes=new List<MidiEvent>();
        midiFile = new MidiFile("melody4.mid");
        foreach(var track in midiFile.Tracks){
            UnityEngine.Debug.Log(track);
            foreach(var midiEvent in track.MidiEvents){
                if(midiEvent.MidiEventType==MidiEventType.NoteOn){
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    melody4Notes.Add(midiEvent);
                    UnityEngine.Debug.Log(note);
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
        UnityEngine.Debug.Log(eventName);
	}

}
