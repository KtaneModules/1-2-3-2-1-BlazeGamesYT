using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModuleScript : MonoBehaviour {

    public KMBombInfo bomb;
    public KMAudio audio;

    //Selectables

    public KMSelectable[] keys;

    //Text

    public TextMesh[] labels;

    //Init

    void Awake()
    {
        
        foreach(KMSelectable key in keys)
        {

            key.OnInteract += delegate () { StartCoroutine(pressKey(key)); return false; };
            key.OnInteractEnded += delegate () { StartCoroutine(releaseKey(key)); };

        }

    }

    void Start () {
		


	}

    private IEnumerator pressKey(KMSelectable key)
    {

        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        key.AddInteractionPunch(.5f);

        for (int i = 0; i < 5; i++)
        {

            key.transform.localPosition -= new Vector3(0, .0040f / 5, 0);
            yield return null;

        }

    }

    private IEnumerator releaseKey(KMSelectable key)
    {

        for (int i = 0; i < 5; i++)
        {

            key.transform.localPosition += new Vector3(0, .0040f / 5, 0);
            yield return null;

        }

    }

}
