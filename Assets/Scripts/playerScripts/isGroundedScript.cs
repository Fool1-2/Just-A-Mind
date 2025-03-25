using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    GameObject player;
    [SerializeField] LayerMask groundLayer;
    public List<float> rayScales;
    public int timer;
    public Vector2[] vecScales;
    //public Collider2D groundCol;
    private float angle;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //transform.position = player.transform.position + new Vector3(0, -1 * (vecScales[(int) PlayerController.playerForm].y + .2f), 0);

    }
    
    private void FixedUpdate() 
    {
        transform.position = player.transform.position + new Vector3(0, -1 * (vecScales[(int) PlayerController.playerForm].y + .2f), 0);
        //groundCol = Physics2D.OverlapBox(transform.position, vecScales[(int) PlayerController.playerForm], angle, groundLayer);
        
    }

    public bool isGrounded()
    {
        
        /*
        if (Physics2D.Raycast(transform.position, Vector2.up, rayScales[(int) PlayerController.playerForm], groundLayer))
        {
            Debug.DrawRay(transform.position, Vector2.up * rayScales[(int) PlayerController.playerForm], Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, Vector2.up * rayScales[(int) PlayerController.playerForm], Color.red);
            return false;
        }
        */
        

        return Physics2D.OverlapBox(transform.position, vecScales[(int) PlayerController.playerForm], angle, groundLayer);
    }

    void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, vecScales[(int) PlayerController.playerForm]);

}
