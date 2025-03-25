using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingObjectScript : MonoBehaviour
{
    Rigidbody2D rockRb;
	[SerializeField]private int rockfallSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        rockRb = GetComponent<Rigidbody2D>();
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			rockRb.bodyType = RigidbodyType2D.Dynamic;
			rockRb.gravityScale = rockfallSpeed;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			Debug.Log("Got you");
			/*Destroy(collision.gameObject);*/
		}
	}
}
