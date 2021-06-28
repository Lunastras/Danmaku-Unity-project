using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public float effectsVolume;
	public float musicVolume;

	private Sound currentlyPlayingMusic;
    private Sound currentlyPlayingAmbient;
    private Sound darkAmbient;

	//private SaveData saveData;

	public Sound[] sounds;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		effectsVolume = musicVolume = 0;

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}
	
	private void Start()
	{
        musicVolume = 0.2f;
        effectsVolume = 0.1f;
        //SetMusicVolume();
        //SetEffectsVolume();
    }

	public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogError("Sound: " + name + " not found!");
			return;
		}

		//s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.volume = s.volume * (s.isMusic ? musicVolume : effectsVolume);

		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		if(s.isMusic)
		{
            if(currentlyPlayingMusic != null)
                currentlyPlayingMusic.source.Stop();

			currentlyPlayingMusic = s;
		}
		
		s.source.Play();
	}



	public void Stop(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.Stop();
	}

    public void PlayAmbient(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }

        //s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.volume = s.volume * (s.isMusic ? musicVolume : effectsVolume);

        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        if(sound.Equals("Dark Ambient"))
        {
           darkAmbient = s;

           s.source.volume = 0;
        } else
        {
            if(currentlyPlayingAmbient != null)
                currentlyPlayingAmbient.source.Stop();

            currentlyPlayingAmbient = s;
        }

        Debug.Log("I am playing " + sound);
        s.source.Play();
    }

    public void SetPitch(float pitch)
    {
        if(currentlyPlayingAmbient != null)
        {
            currentlyPlayingAmbient.source.pitch = currentlyPlayingAmbient.pitch * pitch;
        }

        if (currentlyPlayingMusic != null)
        {
            currentlyPlayingMusic.source.pitch = currentlyPlayingMusic.pitch * pitch;
        }
    }

    public void SetVolume(float vol)
    {
        if (currentlyPlayingAmbient != null)
        {
            currentlyPlayingAmbient.source.volume = currentlyPlayingAmbient.volume * vol * musicVolume;
        }

        if (currentlyPlayingMusic != null)
        {
            currentlyPlayingMusic.source.volume = currentlyPlayingMusic.volume * vol * musicVolume;
        }
    }

    public void SetDarkAmbientVolume(float vol)
    {
        Debug.Log("I GOT CALLED");
        if (darkAmbient != null)
        {
            Debug.Log("Yoosh there is dark ambient ");
            darkAmbient.source.volume = darkAmbient.volume * vol * musicVolume;
        } else
        {
            Debug.Log("fuc ");
        }
    }

    public void StopAudio()
    {
        if(darkAmbient != null)
            darkAmbient.source.Stop();

        if (currentlyPlayingMusic != null)
            currentlyPlayingMusic.source.Stop();

        if (currentlyPlayingAmbient != null)
            currentlyPlayingAmbient.source.Stop();
    }
 
    public void DefSettings()
    {
        SetVolume(1);
        SetPitch(1);
    }

    /*public void SetMusicVolume()
	{

		if(currentlyPlayingMusic != null)
		{
			currentlyPlayingMusic.source.volume = currentlyPlayingMusic.volume * (currentlyPlayingMusic.isMusic ? musicVolume : effectsVolume);
		} else
		{
			Debug.Log("nothing is playing dfq");
		}

		//saveData.musicVolume = musicVolume;
	}
	public void SetEffectsVolume()
	{

		//saveData.effectsVolume = effectsVolume;

		Play("Dunno");
	}*/

}
