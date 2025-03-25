using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissingPart : MonoBehaviour, IInteractable
{
    [SerializeField, Tooltip("0: Ball, 1: Pogo, 2: Arm")]private int playerFormNum;
    private PlayerController player;
    private bool activated;

    public void Interact(){
        player.maxForm = playerFormNum;
        PlayerController.playerPieces[playerFormNum] = true;
        PlayerController.playerForm = (PlayerController.playerForms)playerFormNum;
        player.ChangeForm(playerFormNum);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.GetComponent<PlayerController>();
            if (!activated)
            {
                Interact();
                activated = true;
            }
        }
    }
}
