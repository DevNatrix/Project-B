using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	[HideInInspector] public static AudioPlayer Instance;
	[SerializeField] GameObject audioSourcePrefab;
	[SerializeField] List<AudioClip> clips;
	[SerializeField] ServerEvents serverEvents;
	[SerializeField] Transform playerTransform;
	public static float volumeMult = 1f;

    private void Start()
    {
		Instance = this;
    }

    public void spawnAudio(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, Transform parent = null)
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
		newAudioSource.volume = volume * volumeMult;
		newAudioSource.pitch = pitch;
		newAudioSource.Play();
		Destroy(newAudioSource.gameObject, clip.length);
	}

	public void createAudio(AudioClip audioClip, Vector3 position, float volume = 1, float pitch = 1, int parentClientID = -1)
	{
		createAudio(getIDByClip(audioClip), position, volume, pitch, parentClientID);
	}
	public void createAudio(int clipID, Vector3 position, float volume = 1, float pitch = 1, int parentClientID = -1)
	{
		string[] data = { clipID + "", position + "", volume + "", pitch + "", parentClientID + ""};
		serverEvents.sendEventToOtherClients("playAudio", data);

		if(parentClientID == -1)
		{
			spawnAudio(getClipByID(clipID), position, volume, pitch);
		}
		else if(parentClientID == Client.ID)
		{
			spawnAudio(getClipByID(clipID), position, volume, pitch, playerTransform);
		}
		else
		{
			spawnAudio(getClipByID(clipID), position, volume, pitch, serverEvents.getOtherClientScriptByID(parentClientID).transform);
		}
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
