using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip musicTrack;
    AudioSource myAudioSource;

    public void Awake() {
        MusicPlayer[] musicPlayers = FindObjectsOfType<MusicPlayer>();
        if (musicPlayers.Length > 2) {
            Destroy(gameObject);
        } else if (musicPlayers.Length == 2) {
            foreach(MusicPlayer music in musicPlayers) {
                if(music != GetComponent<MusicPlayer>()) {
                    if(musicTrack == music.GetMusicTrack()) {
                        Destroy(gameObject);
                    } else {
                        Destroy(music.gameObject);
                        DontDestroyOnLoad(gameObject);
                    }
                    break;
                }
            }
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.clip = musicTrack;
        myAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioClip GetMusicTrack() {
        return musicTrack;
    }
}
