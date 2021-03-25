using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KModkit;
using System;

public class ModuleScript : MonoBehaviour {

    public KMBombInfo bomb;
    public KMAudio audio;
    public KMNeedyModule module;

    //Selectables

    public KMSelectable[] keys;
    public MeshRenderer[] ledRenderers;

    //Text

    public TextMesh[] keyTexts;

    //Init
    public static string code = "321";

    public static string inputCode = "";
    public static int enteredDigits = 0;
    private bool isActivated = false;

    //Materials
    public Material ledOff;
    public Material ledGreen;
    public Material ledRed;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;   
    void Start()
    {
        moduleId = moduleIdCounter++;

        
        //Keys
        foreach(KMSelectable key in keys)
        {

            key.OnInteract += () => { StartCoroutine(pressKey(key)); return false; };
            key.OnInteractEnded += () => StartCoroutine(releaseKey(key)); 

        }

        //Needy
        module.OnTimerExpired += timerExpired;
        module.OnNeedyActivation += onActivate;
        module.OnNeedyDeactivation += onDeactivate;
    }
    private IEnumerator pressKey(KMSelectable key)
    {

        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        key.AddInteractionPunch(.5f);

        //Anim
        for (int i = 0; i < 5; i++)
        {

            key.transform.localPosition -= new Vector3(0, .0040f / 5, 0);
            yield return null;

        }

    }
    private IEnumerator releaseKey(KMSelectable key)
    {

        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);

        //Anim
        for (int i = 0; i < 5; i++)
        {

            key.transform.localPosition += new Vector3(0, .0040f / 5, 0);
            yield return null;

        }

        //Function

        if (isActivated)
        {

            lightLED(ledRenderers[Array.IndexOf(keys, key)]);
            inputCode += keyTexts[Array.IndexOf(keys, key)].text;
            enteredDigits++;

            if (enteredDigits == 3)
            {
                Debug.LogFormat("[1-2-3-2-1 #{0}] Inputted Code: {1}", moduleId, inputCode);
                if (inputCode == code)
                {
                    Debug.LogFormat("[1-2-3-2-1 #{0}] CORRECT!!!", moduleId);
                    inputCode = "";
                    passNeedy();
                }
                else
                {
                    strike();
                    StartCoroutine(ResetLEDS());
                    Debug.LogFormat("[1-2-3-2-1 #{0}] Striked From Wrong Code", moduleId);
                }
            }

        }

    }

    private void passNeedy()
    {

        module.HandlePass();
        isActivated = false;

    }

    private void timerExpired()
    {

        isActivated = false;
        strike();
        Debug.LogFormat("[1-2-3-2-1 #{0}] Striked From Time", moduleId);

    }

    private void onActivate()
    {

        isActivated = true;
        Reset();
        turnOffLEDS();
        reverseAnswer();

    }

    private void onDeactivate()
    {

        isActivated = false;

    }

    private void strike()
    {

        module.OnStrike();
        Reset();

    }

    private void Reset()
    {

        inputCode = "";
        enteredDigits = 0;

    }

    //Code Setters
    private void reverseAnswer()
    {

        code = ReverseString(code);
        Debug.LogFormat("[1-2-3-2-1 #{0}] Current Code: {1}", moduleId, code);

    }
    public static string ReverseString(string str)
    {
        char[] chars = str.ToCharArray();
        char[] result = new char[chars.Length];
        for (int i = 0, j = str.Length - 1; i < str.Length; i++, j--)
        {
            result[i] = chars[j];
        }
        return new string(result);
    }

    

    private void lightLED(MeshRenderer key)
    {

        key.material = ledGreen;

    }

    private IEnumerator ResetLEDS()
    {

        foreach (var key in ledRenderers)
        {


            key.material = ledRed;

        }

        yield return new WaitForSecondsRealtime(0.5F);

        foreach (var key in ledRenderers)
        {

            key.material = ledOff;

        }

    }

    private void turnOffLEDS()
    {

        foreach (var key in ledRenderers)
        {

            key.material = ledOff;

        }

    }

}
