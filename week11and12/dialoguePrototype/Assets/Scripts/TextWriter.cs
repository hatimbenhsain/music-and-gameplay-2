using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour
{
    public string[] dialogueWhite;
    public string[] dialogueBrown;

    public TMP_Text whiteTxtBox;
    public TMP_Text brownTxtBox;

    public Image whiteImg;
    public Image brownImg;

    private Music music;

    public float tempo;

    private string currentSyllable;

    private string[] vowels=new string[]{"a","e","i","o","u"};
    public float[] pitchesW=new float[]{1.5f,1f,2f,0.5f,0.67f};
    public float[] pitchesB=new float[]{1.1f,1f,1.2f,0.91f,0.83f};
    private string[] consonants=new string[]{"q","w","r","t","y","p","s","d","f","g","h","k","l","m","j","z","x","c","v","b","n"};

    private int currentIndexW=0;
    private int currentIndexB=0;
    private int currentTextIndex=0;
    private float timer=0f;

    public float frequencyW=1f;
    public float frequencyB=1f;

    public Sprite[] whiteSprites;
    public Sprite[] brownSprites;

    private int whiteBreaks=0;
    private int brownBreaks=0;
    public int breakLengthShort=1;
    public int breakLengthLong=2;

    // Start is called before the first frame update
    void Start()
    {
        music=FindObjectOfType<Music>();
        music.PlayBgMusic();
        currentTextIndex=0;
    }

    // Update is called once per frame
    void Update()
    {

        
        timer+=Time.deltaTime;
        

        WhiteHandler();
        BrownHandler();

        if(Input.GetKeyDown(KeyCode.Space)){
            timer=0;
            currentIndexW=0;
            currentIndexB=0;
            music.PlayBgMusic();
            whiteBreaks=0;
            brownBreaks=0;
            currentTextIndex+=1;
        }
       
        
    }

    bool IsConsonant(string s){
        foreach(string c in consonants){
            if(s.ToLower().Equals(c)){
                return true;
            }
        }
        return false;
    }

    bool IsVowel(string s){
        foreach(string v in vowels){
            if(s.ToLower().Equals(v)){
                return true;
            }
        }
        return false;
    }

    void WhiteHandler(){
        float timeBetweenSyllable=60/(tempo*frequencyW);
        string whiteTxt=dialogueWhite[currentTextIndex%dialogueWhite.Length];

        if(currentIndexW<whiteTxt.Length && Mathf.Floor((timer-Time.deltaTime)/timeBetweenSyllable)<Mathf.Floor((timer)/timeBetweenSyllable)){
            if(whiteBreaks>0){
                whiteBreaks-=1;
                whiteTxtBox.transform.parent.gameObject.GetComponent<Animator>().enabled=false;
            }else{
                whiteTxtBox.transform.parent.gameObject.GetComponent<Animator>().enabled=true;
                bool nextSyllable=false;
                int i=0;
                List<string> consonants=new List<string>();
                List<string> vowels=new List<string>();
                bool punctuationFound=false;
                bool consonantFound=false;
                bool vowelFound=false;
                string lastTypeFound="";
                string currentSyllable="";

                bool breakAdded=false;
                while(!nextSyllable && currentIndexW<whiteTxt.Length){
                    string c=ReplaceAccents(whiteTxt.Substring(currentIndexW,1).ToLower());
                    bool isAConsonant=IsConsonant(c);
                    bool isAVowel=IsVowel(c);
                    bool isPunctuation=(!isAConsonant)&(!isAVowel);

                    string type;
                    if(isAConsonant){
                        type="consonant";
                    }else if(isAVowel){
                        type="vowel";
                    }else{
                        type="punctuation";
                    }

                    if(!breakAdded){
                        if(c.Equals(",") || c.Equals("-")){
                            whiteBreaks+=breakLengthShort;
                            breakAdded=true;
                        }else if(c.Equals(".") || c.Equals("?") || c.Equals("!")){
                            whiteBreaks+=breakLengthLong;
                            breakAdded=true;
                        }
                    }

                    if((consonantFound && vowelFound && lastTypeFound!=type && type!="punctuation") ||(!isPunctuation && punctuationFound)){
                        nextSyllable=true;
                        // if(punctuationFound && currentIndex<txt.Length){
                        //     currentIndex+=1;
                        // }
                        break;
                    }

                    if(isAConsonant){
                        consonantFound=true;
                    }
                    if(isAVowel){
                        vowelFound=true;
                    }
                    if(isPunctuation){
                        punctuationFound=true;
                    }
                    
                    currentIndexW++;
                    lastTypeFound=type;
                    currentSyllable=currentSyllable+whiteTxt[currentIndexW-1];
                }
                Debug.Log("*"+currentSyllable+"*");

                PlayDrums(currentSyllable);
            }
        }else if(Mathf.Floor((timer-Time.deltaTime)/timeBetweenSyllable)<Mathf.Floor((timer)/timeBetweenSyllable)){
            whiteImg.sprite=whiteSprites[0];
            whiteTxtBox.transform.parent.gameObject.GetComponent<Animator>().enabled=false;
        }
        
        if(whiteTxt==""){
            whiteTxtBox.transform.parent.gameObject.SetActive(false);
        }else{
            whiteTxtBox.transform.parent.gameObject.SetActive(true);
        }

        whiteTxtBox.text=whiteTxt.Substring(0,Mathf.Min(currentIndexW,whiteTxt.Length));
    }

    void BrownHandler(){
        float timeBetweenSyllable=60/(tempo*frequencyB);
        string brownTxt=dialogueBrown[currentTextIndex%dialogueBrown.Length];

        if(currentIndexB<brownTxt.Length && Mathf.Floor((timer-Time.deltaTime)/timeBetweenSyllable)<Mathf.Floor((timer)/timeBetweenSyllable)){
            if(brownBreaks>0){
                brownBreaks-=1;
                brownTxtBox.transform.parent.gameObject.GetComponent<Animator>().enabled=false;
            }else{
                brownTxtBox.transform.parent.gameObject.GetComponent<Animator>().enabled=true;
                bool nextSyllable=false;
                int i=0;
                List<string> consonants=new List<string>();
                List<string> vowels=new List<string>();
                bool punctuationFound=false;
                bool consonantFound=false;
                bool vowelFound=false;
                string lastTypeFound="";
                string currentSyllable="";

                bool breakAdded=false;
                while(!nextSyllable && currentIndexB<brownTxt.Length){
                    string c=ReplaceAccents(brownTxt.Substring(currentIndexB,1).ToLower());
                    bool isAConsonant=IsConsonant(c);
                    bool isAVowel=IsVowel(c);
                    bool isPunctuation=(!isAConsonant)&(!isAVowel);

                    string type;
                    if(isAConsonant){
                        type="consonant";
                    }else if(isAVowel){
                        type="vowel";
                    }else{
                        type="punctuation";
                    }

                    if(!breakAdded){
                        if(c.Equals(",") || c.Equals("-")){
                            brownBreaks+=breakLengthShort;
                            breakAdded=true;
                        }else if(c.Equals(".") || c.Equals("?") || c.Equals("!")){
                            brownBreaks+=breakLengthLong;
                            breakAdded=true;
                        }
                    }

                    if((consonantFound && vowelFound && lastTypeFound!=type && type!="punctuation") ||(!isPunctuation && punctuationFound)){
                        nextSyllable=true;
                        // if(punctuationFound && currentIndex<txt.Length){
                        //     currentIndex+=1;
                        // }
                        break;
                    }

                    if(isAConsonant){
                        consonantFound=true;
                    }
                    if(isAVowel){
                        vowelFound=true;
                    }
                    if(isPunctuation){
                        punctuationFound=true;
                    }
                    
                    currentIndexB++;
                    lastTypeFound=type;
                    currentSyllable=currentSyllable+brownTxt[currentIndexB-1];
                }
                Debug.Log("*"+currentSyllable+"*");

                PlayJazz(currentSyllable);
            }
        }else if(Mathf.Floor((timer-Time.deltaTime)/timeBetweenSyllable)<Mathf.Floor((timer)/timeBetweenSyllable)){
            brownImg.sprite=brownSprites[0];
            brownTxtBox.transform.parent.gameObject.GetComponent<Animator>().enabled=false;
        }
        
        if(brownTxt==""){
            brownTxtBox.transform.parent.gameObject.SetActive(false);
        }else{
            brownTxtBox.transform.parent.gameObject.SetActive(true);
        }

        brownTxtBox.text=brownTxt.Substring(0,Mathf.Min(currentIndexB,brownTxt.Length));
    }

    void PlayDrums(string currentSyllable){
        List<string> ps=new List<string>(); //parameters
        currentSyllable=currentSyllable.ToLower();

        if(currentSyllable.Contains("t") || currentSyllable.Contains("d")){
            ps.Add("bassDrum");
        }else if(currentSyllable.Contains("m") || currentSyllable.Contains("n")){
            ps.Add("snareDrum");
        }else if(currentSyllable.Contains("c") || currentSyllable.Contains("k")){
            ps.Add("snareDrum2");
        }else if(currentSyllable.Contains("b") || currentSyllable.Contains("v") || currentSyllable.Contains("p")){
            ps.Add("bassDrum6");
        }else if(currentSyllable.Contains("s")){
            ps.Add("openHihat2");
        }else if(currentSyllable.Contains("h")){
            ps.Add("lowConga");
        }else if(currentSyllable.Contains("r") || currentSyllable.Contains("l")){
            ps.Add("hiConga");
        }else if(currentSyllable.Contains("w") || currentSyllable.Contains("y")){
            ps.Add("closedHiHat");
        }else if(currentSyllable.Contains("f")){
            ps.Add("zap");
        }else if(currentSyllable.Contains("g")){
            ps.Add("bassDrum2");
        }else if(currentSyllable.Contains("j")){
            ps.Add("openHihat");
        }else if(currentSyllable.Contains("x") || currentSyllable.Contains("q") || currentSyllable.Contains("z")){
            ps.Add("clap");
        }

        float p=1;

        for(int i=0;i<vowels.Length;i++){
            if(currentSyllable.Contains(vowels[i])){
                p=pitchesW[i];
                break;
            }
        }

        if(ps.Count==0){
            music.PlayDrum("bassDrum6",p);
        }else if(ps.Count==1){
            music.PlayDrum(ps[0],p);
        }else if(ps.Count==2){
            music.PlayDrum(ps[0],p,ps[1]);
        }else if(ps.Count==3){
            music.PlayDrum(ps[0],p,ps[1],ps[2]);
        }else if(ps.Count==4){
            music.PlayDrum(ps[0],p,ps[1],ps[2],ps[3]);
        }

        if(currentSyllable.Contains("a")){
            whiteImg.sprite=whiteSprites[1];
        }else if(currentSyllable.Contains("o")){
            whiteImg.sprite=whiteSprites[2];
        }else if(currentSyllable.Contains("i")){
            whiteImg.sprite=whiteSprites[3];
        }else if(currentSyllable.Contains("u")){
            whiteImg.sprite=whiteSprites[4];
        }else if(currentSyllable.Contains("e")){
            whiteImg.sprite=whiteSprites[5];
        }
    }

    public void PlayJazz(string currentSyllable){
        List<string> pianoPs=new List<string>(); //parameters
        List<string> bassPs=new List<string>(); //parameters
        List<string> saxPs=new List<string>(); //parameters
        currentSyllable=currentSyllable.ToLower();

        string[] pianoConsonants=new string[]{"m","n","z","s","v","l","f"};
        string[] bassConsonants=new string[]{"t","d","g","k","b","p","j"};
        string[] saxConsonants=new string[]{"x","h","w","y","c","q","r"};

        for(int i=0;i<pianoConsonants.Length;i++){
            if(currentSyllable.Contains(pianoConsonants[i])){
                pianoPs.Add((i+1).ToString());
                brownImg.sprite=brownSprites[1];
            }
            if(currentSyllable.Contains(bassConsonants[i])){
                bassPs.Add((i+1).ToString());
                brownImg.sprite=brownSprites[2];
            }
            if(currentSyllable.Contains(saxConsonants[i])){
                saxPs.Add((i+1).ToString());
                brownImg.sprite=brownSprites[3];
            }
        }

        float p=1;
        for(int i=0;i<vowels.Length;i++){
            if(currentSyllable.Contains(vowels[i])){
                p=pitchesB[i];
                break;
            }
        }

        //p=1;

        if(pianoPs.Count==0 && bassPs.Count==0 && saxPs.Count==0){
            pianoPs.Add("4");
        }

        List<string> ps;

        if(pianoPs.Count>0){
            ps=pianoPs;
            if(ps.Count==1){
                music.PlayJazz("piano",ps[0],p);
            }else if(ps.Count==2){
                music.PlayJazz("piano",ps[0],p,ps[1]);
            }else if(ps.Count==3){
                music.PlayJazz("piano",ps[0],p,ps[1],ps[2]);
            }else if(ps.Count==4){
                music.PlayJazz("piano",ps[0],p,ps[1],ps[2],ps[3]);
            }
        }
        if(bassPs.Count>0){
            ps=bassPs;
            if(ps.Count==1){
                music.PlayJazz("bass",ps[0],p);
            }else if(ps.Count==2){
                music.PlayJazz("bass",ps[0],p,ps[1]);
            }else if(ps.Count==3){
                music.PlayJazz("bass",ps[0],p,ps[1],ps[2]);
            }else if(ps.Count==4){
                music.PlayJazz("bass",ps[0],p,ps[1],ps[2],ps[3]);
            }
        }
        if(saxPs.Count>0){
            ps=saxPs;
            if(ps.Count==1){
                music.PlayJazz("sax",ps[0],p);
            }else if(ps.Count==2){
                music.PlayJazz("sax",ps[0],p,ps[1]);
            }else if(ps.Count==3){
                music.PlayJazz("sax",ps[0],p,ps[1],ps[2]);
            }else if(ps.Count==4){
                music.PlayJazz("sax",ps[0],p,ps[1],ps[2],ps[3]);
            }
        }
    }

    string ReplaceAccents(string s){
        return s.Replace("é","e").Replace("è","e").Replace("î","i").Replace("ê","e").Replace("à","a");
    }
}
