using UnityEngine;
using System.Collections;

public class MusicControl : MonoBehaviour
{
	public AudioSource musicLevel0;
	public AudioSource musicLevel1;
	public AudioSource musicLevel2;
	public float fadeDuration = 1;
	
	public void SetupLevel1()
	{
		StartCoroutine( switchMusic(musicLevel0, musicLevel1) );
	}
	
	public void SetupLevel2()
	{
		StartCoroutine( switchMusic(musicLevel1, musicLevel2) );
	}
	
	IEnumerator switchMusic( AudioSource from, AudioSource to )
	{
		float timeLeft = fadeDuration;
		
		while (timeLeft > 0)
		{
			timeLeft = Mathf.Max(0, timeLeft - Time.deltaTime);
			float cursor = timeLeft / fadeDuration;
			from.volume = cursor;
			to.volume = cursor;
			yield return 0;
		}
	}
}
