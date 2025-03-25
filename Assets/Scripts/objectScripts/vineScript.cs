using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vineScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionStay2D(Collision2D other) {
        // Filter for specific hands
        if (other.gameObject.tag == "hand")
        {
            if (other.gameObject.name == "leftArm")
            {
                
            }
            else
            {

            }
        }
    }
}
