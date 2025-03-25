using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADASD : MonoBehaviour
{
    [SerializeField]private SpringJoint2D spring;

    // Start is called before the first frame update
    void Start()
    {
        spring = GetComponent<SpringJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            spring.connectedBody = null;
            //isConnected = false;
        }
    }
}
