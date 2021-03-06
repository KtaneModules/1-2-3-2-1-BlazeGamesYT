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
        
    void Start()
    {
        
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

                if (inputCode == code)
                {

                    Debug.Log("CORRECT!!!");
                    inputCode = "";
                    passNeedy();

                }
                else
                {


                    strike();
                    StartCoroutine(ResetLEDS());
                    Debug.Log("Striked From Wrong Code");



                }

            }

            Debug.Log(inputCode);

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
        Debug.Log("Striked from time");

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
        Debug.Log("Current Code: " + code);

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

    //Child Finders
    public static GameObject findTextChild(GameObject parent)
    {
        GameObject result;
        result = parent;

        foreach (Transform child in parent.transform)
        {
            if (child.tag == "keyLabel")
            {

                result = child.gameObject;
                Debug.Log("Found Text Child: " + child.gameObject.name);

            }
           
        }

        return result;

    }

    public GameObject findLedChild(GameObject parent)
    {
        GameObject result;
        result = parent;

        foreach (Transform child in parent.transform)
        {
            if (child.tag == "keyLED")
            {

                result = child.gameObject;
                Debug.Log("Found LED Child: " + child.gameObject.name);

            }

        }

        return result;

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
