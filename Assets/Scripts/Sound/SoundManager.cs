using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    //private AudioClip[] mySounds;
    private static SoundManager _instance;
    AudioSource m_audiosource;
    private float audioVolume;
    public static SoundManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SoundManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);
            m_audiosource = gameObject.GetComponent<AudioSource>();
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
        audioVolume = 0.8f;
    }
    public void Play(int id, float volume = 1.0f)
    {
        if (GameLoop.instance)
        {
            //AudioSource.PlayClipAtPoint(GameLoop.instance.getMySound()[id], new Vector3(0, 0, 0), volume);
            m_audiosource.PlayOneShot(GameLoop.instance.getMySound()[id], 1F);
        }
        //Debug.Log("sound leng: " + mySounds.Length);
        //AudioSource.PlayClipAtPoint(mySounds[id], new Vector3(0, 0, 0), volume);
    }

    public void Play(int id, Vector3 pos, float volume = 1.0f)
    {
        if (GameLoop.instance)
        {
            AudioSource.PlayClipAtPoint(GameLoop.instance.getMySound()[id], pos, audioVolume);
        }
        //AudioSource.PlayClipAtPoint(mySounds[id], pos, audioVolume);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1.0f)
    {
        m_audiosource.clip = clip;
        m_audiosource.loop = loop;
        m_audiosource.volume = volume;
        m_audiosource.Play();
    }

    public void PlayClip(AudioClip clip, float volume = 1.0f)
    {
        AudioSource.PlayClipAtPoint(clip, new Vector3(0, 0, 0), audioVolume);
    }

    public void StopMusic()
    {
        m_audiosource.Stop();
    }

    public void PauseMusic()
    {
        m_audiosource.Pause();
    }

    public void ResumeMusic()
    {
        m_audiosource.Play();
    }

    public void setVolume(float value)
    {
        audioVolume = value;
        m_audiosource.volume = audioVolume;
    }

    public float getVolume()
    {
        return audioVolume;
    }
}
