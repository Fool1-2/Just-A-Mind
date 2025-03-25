using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vineHingePosScript : MonoBehaviour
{
    // The default distance that the hinge joint should be from the player. Would be negative if left arm
    public GameObject player;
    HingeJoint2D pHingeJoint;

    float delayTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pHingeJoint = player.GetComponent<HingeJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    /*private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.tag == "hand")
        {
            if (Input.GetKeyDown(pmScript.abilityKey) && !pmScript.isConnected)
            {
                // Turns on the hinge joint and makes it move with respect to the vines rigidbody
                pHingeJoint.enabled = true;
                pHingeJoint.connectedBody = this.GetComponentInParent<Rigidbody2D>();
                // Then sets the Anchor point of the joint to this position
                pHingeJoint.connectedAnchor = transform.position;
                // Sets the players position 
                player.transform.position = transform.position - new Vector3 (playerMovementScript.hingeJointAnchorDistance.x * 
                Mathf.Cos(Mathf.Deg2Rad * player.transform.eulerAngles.z), playerMovementScript.hingeJointAnchorDistance.x * 
                Mathf.Sin(Mathf.Deg2Rad * player.transform.eulerAngles.z), 0);
                pmScript.isConnected = true;
                delayTimer = 0;
            }
        }
    }*/
}
