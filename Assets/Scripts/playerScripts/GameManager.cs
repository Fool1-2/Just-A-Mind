using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController pc;
    public GameObject player;
    public GameObject musicChanger;
    float speedrunTimer;

    void Start()
    {
        player = GameObject.Find("Player");
        pc = player.GetComponent<PlayerController>();
        musicChanger.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        speedrunTimer += Time.deltaTime;
		/*if (pc.devControl == true)
		{
            if (Input.GetKeyDown(KeyCode.R))
            {
                player.transform.position = pc.spawner.transform.position;
            }
        }*/

        

    
		
    }


}
