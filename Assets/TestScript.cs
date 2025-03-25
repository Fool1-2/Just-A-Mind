using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    private Collider2D hit;
    [SerializeField]private LayerMask hitLayer;
    [SerializeField]private float hitFloat;

    [SerializeField]private bool isConnected;

    [SerializeField]private SpringJoint2D spring;

    public Transform hittransform;

    // Start is called before the first frame update
    void Start()
    {
        spring = GetComponent<SpringJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hit != null)
        {
            print("NUll");


            if (Input.GetKeyDown(KeyCode.K))
            {
                spring.connectedBody = hit.GetComponent<Rigidbody2D>();
                //spring.connectedAnchor = hittransform.localPosition;
                isConnected = true;
            }
        }

        
        if (isConnected)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                spring.connectedBody = null;
                isConnected = false;
            }
        }
        
    }

    void FixedUpdate() {
        hit = Physics2D.OverlapCircle(transform.position, hitFloat, hitLayer);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, hitFloat);
    }
}
