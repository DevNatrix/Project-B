using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
	[SerializeField] float hitMarkerFadeSpeed;
	[SerializeField] Color headShotColor;
	[SerializeField] Color hitMarkerColor;
	[SerializeField] Color baseColor;
	[SerializeField] TextMeshProUGUI hitMarker;
	[SerializeField] AudioPlayer audioPlayer;
	[SerializeField] AudioClip hitAudioClip;
	[SerializeField] AudioClip headshotAudioClip;
	[SerializeField] Transform playerTransform;
	[SerializeField] float hitMarkerPitch;
	[SerializeField] float hitMarkerVolume;
	[SerializeField] float headshotVolume;
	[SerializeField] float headshotPitch;
	public void hitPlayer(bool headshot = false)
	{
		if(headshot)
		{
			hitMarker.color = headShotColor;
			audioPlayer.spawnAudio(headshotAudioClip, playerTransform.position, headshotVolume, headshotPitch, playerTransform);
			audioPlayer.spawnAudio(hitAudioClip, playerTransform.position, hitMarkerVolume, hitMarkerPitch, playerTransform);
		}
		else
		{
			hitMarker.color = hitMarkerColor;
			audioPlayer.spawnAudio(hitAudioClip, playerTransform.position, hitMarkerVolume, hitMarkerPitch, playerTransform);
		}
	}

	private void Update()
	{
		hitMarker.color = Color.Lerp(hitMarker.color, baseColor, hitMarkerFadeSpeed * Time.deltaTime);
	}
}
