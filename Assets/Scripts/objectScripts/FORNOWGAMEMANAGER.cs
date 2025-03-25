using UnityEngine;
using System.Collections;

public class ai : MonoBehaviour {

	//Bool
	bool monsterRaycast;
	bool patrol = true;
	
	//Transform
	public Transform raycastLocation;
	
	//Raycast
	private RaycastHit hit;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		monsterRaycast = Physics.Raycast(raycastLocation.position, transform.TransformDirection (Vector3.forward), out hit, 5);
		
		if(hit.transform.tag == "Player")
		{
			patrol = false;
		}
		if(patrol)
		{
			//Do Stuff
		}
		else
		{
			//Do Stuff
		}
	}
}