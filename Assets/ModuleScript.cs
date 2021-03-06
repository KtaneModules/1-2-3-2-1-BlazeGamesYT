using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KModkit;

public class ModuleScript : MonoBehaviour {

    public KMBombInfo bomb;
    public KMAudio audio;

    //Selectables

    public KMSelectable[] keys;

    //Text

    public TextMesh[] labels;

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
        GetComponent<KMNeedyModule>().OnTimerExpired += timerExpired;
        GetComponent<KMNeedyModule>().OnNeedyActivation += onActivate;
        GetComponent<KMNeedyModule>().OnNeedyDeactivation += onDeactivate;
    }

    void Update()
    {

                

    }

    private IEnumerator pressKey(KMSelectable key)
    {

        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
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

        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);

        //Anim
        for (int i = 0; i < 5; i++)
        {

            key.transform.localPosition += new Vector3(0, .0040f / 5, 0);
            yield return null;

        }

        //Function

        if (isActivated)
        {

            lightLED(key.gameObject);
            inputCode += findTextChild(key.gameObject).GetComponent<TextMesh>().text;
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

        GetComponent<KMNeedyModule>().OnPass();
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

        GetComponent<KMNeedyModule>().OnStrike();
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

    private void lightLED(GameObject key)
    {

        findLedChild(key).GetComponent<MeshRenderer>().material = ledGreen;

    }

    private IEnumerator ResetLEDS()
    {

        foreach (KMSelectable key in keys)
        {


            findLedChild(key.gameObject).GetComponent<MeshRenderer>().material = ledRed;

        }

        yield return new WaitForSecondsRealtime(0.5F);

        foreach (KMSelectable key in keys)
        {

            findLedChild(key.gameObject).GetComponent<MeshRenderer>().material = ledOff;

        }

    }

    private void turnOffLEDS()
    {

        foreach (KMSelectable key in keys)
        {

            findLedChild(key.gameObject).GetComponent<MeshRenderer>().material = ledOff;

        }

    }

}
