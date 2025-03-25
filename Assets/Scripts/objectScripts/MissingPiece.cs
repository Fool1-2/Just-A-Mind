using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissingPiece : MonoBehaviour
{
    [SerializeField, Tooltip("0: Ball, 1: Pogo, 2: Arm")]private int playerFormNum;

    public void Interact(){
        PlayerController.playerPieces[playerFormNum] = true;
        PlayerController.playerForm = (PlayerController.playerForms)playerFormNum;
        Destroy(gameObject);
    }
}
