using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GrappleScript : MonoBehaviour
{
	#region SphereCast
	public float sphereRadius = 5f;
	public float distance = 3.5f;
	public Transform spherePoint;
	public LayerMask grappleLayer;
	public bool isConnected = false;
	#endregion

	#region Grappling
	public LineRenderer Lr;
	public SpringJoint2D Sj;
	public Transform grapplePoint;
	#endregion

	#region KeyCodes
	public KeyCode grapple;
	#endregion

	// Start is called before the first frame update
	void Start()
    {
        Lr = GetComponent<LineRenderer>();
		Sj = GetComponent<SpringJoint2D>();
		Sj.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
		SphereCast();

		if (Input.GetKeyDown(grapple) && isConnected)
		{
		   StartGrapple();
		}
		else if (Input.GetKeyUp(grapple)) 
		{
			EndGrapple();
			
		}
    }

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(spherePoint.position, sphereRadius);
	}

	void SphereCast() 
	{
		RaycastHit2D hit = Physics2D.CircleCast(spherePoint.position, sphereRadius, Vector2.right, distance, grappleLayer);
		isConnected = grapplePoint != null;
	}

	void StartGrapple() 
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, Mathf.Infinity, grappleLayer);
		if (isConnected)
		{
			Sj.enabled = true;
			Sj.connectedAnchor = grapplePoint.position;
			Sj.distance = Vector2.Distance(transform.position, grapplePoint.position);
			Sj.dampingRatio = 0.5f; // Adjust as needed
			Sj.frequency = 1.5f; // Adjust as needed

			// Optional: Draw the grapple line
			Lr.enabled = true;
			Lr.SetPosition(0, transform.position);
			Lr.SetPosition(1, grapplePoint.position);
		}
	}

	void EndGrapple()
	{
		Sj.enabled = false;
		Lr.enabled = false;
	}

}
