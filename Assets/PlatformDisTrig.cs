using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDisTrig : MonoBehaviour
{
	public GameObject platform;
	public GameObject treeObj;
	public GameObject treeObjTwo;
	public GameObject brokenObj;
	public float breakTimer;
	public AudioManagerScript ams;

	private void Start()
	{
		ams = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
		treeObj.SetActive(true);
		treeObjTwo.SetActive(true);
		brokenObj.SetActive(false);
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			breakTimer -= Time.deltaTime;
			
		}

		if (breakTimer <= 0)
		{
			Destroy(platform);
			treeObj.SetActive(false);
			treeObjTwo.SetActive(false);
			brokenObj.SetActive(true);
			ams.currentSfx = ams.soundFX[0];
			ams.sfx.clip = ams.currentSfx;
			StartCoroutine(playTreeClip());
				
		}

		
	}

	private IEnumerator playTreeClip()
	{
		ams.sfx.Play();
		yield return new WaitForSeconds(ams.sfx.clip.length);
		ams.sfx.Stop();
	}
}
