using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySE : MonoBehaviour{
    public AudioClip[] se;
    AudioSource source;
    void Start(){
        source = GetComponent<AudioSource>();
    }

    public void play(int number) {
        source.PlayOneShot(se[number]);
    }
}
