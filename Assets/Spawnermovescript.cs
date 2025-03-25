using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnermovescript : MonoBehaviour
{
	public Transform newSpawnPos;
	public GameObject spawner;

	private void Start()
	{
		spawner.SetActive(true);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			//spawner.transform.position = newSpawnPos.transform.position;
			spawner.SetActive(false);
		}
	}
}
