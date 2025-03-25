using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class asdas : MonoBehaviour
{
    PlayerController player;
    private Collider2D hit;
    [SerializeField]private LayerMask hitLayer;
    [SerializeField]private float hitFloat;

    public bool isConnected;

    [SerializeField]private HingeJoint2D spring;
    [SerializeField]private float upperLimit, lowerLimit;
    [SerializeField]private float boostX, boostY;
    [SerializeField]private Vector2 rightSide, leftSide;
    private Vector2 side;

    // Start is called before the first frame update
    void Start()
    {
        //spring = GetComponent<SpringJoint2D>();
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

        
        if (hit != null)
        {
            if (IsVineDirRight(hit.transform.position))
            {
                side = rightSide;
            }
            else{
                side = leftSide;
            }

            if (!isConnected)
            {
                if (Input.GetKeyDown(KeyCode.K))
                {
                    spring.enabled = true;
                    spring.autoConfigureConnectedAnchor = false;
                    spring.useLimits = true;
                    Vector2 vec = hit.GetComponent<vinetest>().transformTest.localPosition;
                    spring.connectedBody = hit.GetComponent<Rigidbody2D>();
                    spring.anchor = side;
                    spring.connectedAnchor = vec;
    
                    hit.GetComponent<vinetest>().onVine = true;
                    isConnected = true;
                }
            }
        }

        
        if (isConnected)
        {
            player.rb.freezeRotation = false;
            if (Input.GetKeyDown(KeyCode.L))
            {
                //Destroy(GetComponent<HingeJoint2D>());
                spring.connectedBody = null;
                spring.enabled = false;
                player.rb.AddForce(new Vector2(player.horizontal * boostX, boostY), ForceMode2D.Impulse);
                player.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
                hit.GetComponent<vinetest>().onVine = false;
                isConnected = false;
            }
        }
        else{
            if (player.curForm == 1)
            {
                player.rb.freezeRotation = true;
            }
        }
    }

    bool IsVineDirRight(Vector2 vine){
        if (vine.x > transform.position.x)
        {
            return true;
        }

        return false;
    }

    void FixedUpdate() {
        hit = Physics2D.OverlapCircle(transform.position, hitFloat, hitLayer);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, hitFloat);
    }
}
