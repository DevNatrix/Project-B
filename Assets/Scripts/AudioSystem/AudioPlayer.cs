using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	[HideInInspector] public static AudioPlayer Instance;
	[SerializeField] GameObject audioSourcePrefab;
	[SerializeField] List<AudioClip> clips;
	[SerializeField] ServerEvents serverEvents;

    private void Start()
    {
		Instance = this;
    }

    public void createAudio(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, Transform parent = null)
	{
		AudioSource newAudioSource;
		if(parent == null)
		{
			newAudioSource = Instantiate(audioSourcePrefab, position, Quaternion.identity).GetComponent<AudioSource>();
		}
		else
		{
			newAudioSource = Instantiate(audioSourcePrefab, position, Quaternion.identity, parent).GetComponent<AudioSource>();
		}
		newAudioSource.clip = clip;
		newAudioSource.volume = volume;
		newAudioSource.pitch = pitch;
		newAudioSource.Play();
		Destroy(newAudioSource.gameObject, clip.length);
	}

	public void sendAudioByClip(AudioClip audioClip, Vector3 position, float volume, float pitch)
	{
		string[] data = { getIDByClip(audioClip) + "", position + "", volume + "", pitch + "" };
		serverEvents.sendGlobalEvent("playAudio", data);
	}
	public void sendAudioByID(int clipID, Vector3 position, float volume, float pitch)
	{
		string[] data = { clipID + "", position + "", volume + "", pitch + "" };
		serverEvents.sendGlobalEvent("playAudio", data);
	}

	public AudioClip getClipByID(int id)
	{
		if(id >= clips.Count)
		{
			Debug.LogError("Audio clip ID is outside the possible indexes of the audio player's clip list");
			return clips[0];
		}
		return clips[id];
	}
	public int getIDByClip(AudioClip audioClip)
	{
		int ID = clips.IndexOf(audioClip);
		if(ID == -1)
		{
			Debug.LogError("Audio clip not found in list - assign audio clip in audio player's clip list");
			return 0;
		}
		return ID;
	}
}
