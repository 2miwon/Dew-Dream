using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    //public AudioClip sound;
    AudioSource audioSource;
    public void Start(){
        audioSource = GetComponent<AudioSource>();
        //audioSource.clip = sound;
    }
    // Start is called before the first frame update
    public void Change()
    {
        audioSource.Play();
        SceneManager.LoadScene("Stage");
    }

}
