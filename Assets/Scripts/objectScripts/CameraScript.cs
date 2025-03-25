using System.Transactions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("----Cam Movement Variables----")]
    public float camDefaultFOV;
    [SerializeField]private float startingFOV;

    private GameObject cameraObj;
    public static GameObject playerObj;
    public GameObject testobj;
    private Camera cam;

    public float cameraSpeed, backSpeed;//Follows player speed and comes back to player speed
    [SerializeField]private float zoomBackSpeed, Distance;//How fast the camera goes back to following the player and how close before switching movements to follow the player
    public bool isFollowingPlayer, isComingBack, isZoom;//Different modes for the camera
    private bool hasZoomed;

    public bool notFollowingX, notFollowingY;
    public static bool isCameraShaking, isCameraZooming;
    [SerializeField, Range(0, 3)]private float shakeAmount;
    private Vector2 origCamPos;

    //The ray cast is for finding if the player is on the same Y axis and close enough distacne to switch between the differen ways of following an object
    [SerializeField]private float rayDistance;
    [SerializeField]private RaycastHit2D findPlayer;

    
    Vector2 curPos;
    Vector2 lastPos;


    private void Start() {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        cam = GetComponentInChildren<Camera>();
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        //testobj = playerObj;
        if (startingFOV == camDefaultFOV || startingFOV == 0)
        {
            cam.orthographicSize = camDefaultFOV;
        }else{
            cam.orthographicSize = startingFOV;
        }
        //playerRB = playerObj.GetComponent<Rigidbody2D>();
        //startCamOffset = cameraOffset;
    }

    private void OnEnable(){
        //cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        //origCamPos = cameraObj.transform.position;
    }

    private void Update(){
        curPos = transform.position;
        Vector2 vel = curPos - lastPos;
        lastPos = transform.position;

        
        if (isZoom)
        {

            playerObj.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;

        }else if(!isZoom && !isComingBack){

            playerObj.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;

        }

        if (isComingBack)
        {
            if (findPlayer.transform.tag == "Player")
            {
                isComingBack = false;
            }
        }

        CameraShake();
        if (isFollowingPlayer)
        {
            if (isComingBack)
            {
                FollowBackToPlayer(backSpeed, playerObj.transform);
            }
        }
    }
    private void FixedUpdate() {

        findPlayer = Physics2D.Raycast(transform.position, -Vector2.up * rayDistance);
        Debug.DrawRay(transform.position, -Vector2.up * rayDistance, Color.blue);
        //print(findPlayer);


        if (isFollowingPlayer)
        {
            if (!isComingBack)
            {
                FollowBackToPlayer(cameraSpeed, playerObj.transform);
            }
        }
    }

    public void FollowObjDelay(float speed, Transform followObj)//Follows any obj(Should always be put in fixed update so it can add the rigidbody. If it is not in F.U it will make anything with a rigidbody jitter when moved.)
    {
        //transform.position = Vector2.Lerp(transform.position, (Vector2)followObj.position , speed);
        transform.position = Vector2.MoveTowards(transform.position, followObj.position, speed * Time.deltaTime);
        //GoingToFast(40, 40);

    }

    public void FollowBackToPlayer(float speed, Transform followObj){

        float playerDist = Vector2.Distance(playerObj.transform.position, transform.position);

        if (notFollowingX && notFollowingY)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y);
        }
        
        if (!notFollowingX || !notFollowingY)//This is used to make sure it doesn't interfere with the both of the axis being stopped.
        {
            if (notFollowingX)
            {
                //Only follows the y axis
                //float Y = Mathf.Lerp(transform.position.y, followObj.transform.position.y, speed);
                float Y = (!isComingBack) ? Mathf.Lerp(transform.position.y, followObj.transform.position.y, speed):
                Mathf.MoveTowards(transform.position.y, followObj.transform.position.y, backSpeed * Time.deltaTime);//Only moves on the Y axis and switches smoothly when the camera is coming back to the player
                
                transform.position = new Vector2(transform.position.x, Y);
                //print("Not following X");
            }

            if (notFollowingY)
            {
                //float X = Mathf.Lerp(transform.position.x, followObj.transform.position.x, speed);
                float X = (!isComingBack) ? Mathf.Lerp(transform.position.x, followObj.transform.position.x, speed):
                Mathf.MoveTowards(transform.position.x, followObj.transform.position.x, backSpeed * Time.deltaTime);////Only moves on the X axis and switches smoothly when the camera is coming back to the player

                transform.position = new Vector2(X, transform.position.y);
                //print("Not following Y");
            }
        }
        
        if (!notFollowingX && !notFollowingY)
        {
            //transform.position = Vector2.Lerp(transform.position, (Vector2)followObj.position, speed);
            transform.position = (isComingBack) ? Vector2.MoveTowards(transform.position, (Vector2)followObj.transform.position, backSpeed * Time.deltaTime):
            Vector2.Lerp(transform.position, (Vector2)followObj.position, speed);//Moves in all directions and smoothly comes back to the player
        }

    }


    public void ZoomCameraChange(float FOV, float zoomSpeed){//Zooms back and fourth wether it is the player or not. Never make the desired FOV smaller than the defualt FOV which is 5
        
        if (Mathf.Abs(cam.orthographicSize - FOV) < 0.1f)
        {
            print("WORKING WOKRING");
            hasZoomed = true;
        }
        if (isZoom)
        {
            if (!hasZoomed)
            {
                if (cam.orthographicSize < FOV)
                {
                    cam.orthographicSize += Time.deltaTime * zoomSpeed;
                }else if(cam.orthographicSize > FOV){
                    cam.orthographicSize -= Time.deltaTime * zoomSpeed;
                }
            }else
            {
                cam.orthographicSize = FOV;
            }
        }
        else
        {
            hasZoomed = false;
            if (cam.orthographicSize > camDefaultFOV)
            {
                cam.orthographicSize -= Time.deltaTime * zoomSpeed;
            }else if(cam.orthographicSize < camDefaultFOV){
                cam.orthographicSize += Time.deltaTime * zoomSpeed;
            }else{
                cam.orthographicSize = camDefaultFOV;
            }
        }
        
    }
    
    private void CameraShake()//Shake camera.
    {
    
        if (isCameraShaking){
            //cameraObj.transform.localPosition = origCamPos + Random.insideUnitCircle * shakeAmount;
            //cameraObj.transform.localPosition = new Vector3(cameraObj.transform.localPosition.x, cameraObj.transform.localPosition.y, -10f);
        }
        else
        {
            //cameraObj.transform.localPosition = (Vector2)origCamPos;
            //cameraObj.transform.localPosition = new Vector3(cameraObj.transform.localPosition.x, cameraObj.transform.localPosition.y, -10f);
        }
        
    }



    /*
    private void GoingToFast(float maxLimitX, float maxLimitY){
        //Should increase the offset in the direction its going to fast in.
        if (playerRB.velocity.x > maxLimitX)
        {
            print("Going Right");
            goingRight = true;
            cameraOffset.x += Time.deltaTime / playerRB.velocity.x;
            
        }else if(playerRB.velocity.x < -maxLimitX){
            print("Going Left");
            goingRight = false;
            cameraOffset.x += Time.deltaTime / playerRB.velocity.x;
        }

        if (goingRight && playerRB.velocity.x < maxLimitX)
        {
            if (cameraOffset.x > startCamOffset.x)
            {
                cameraOffset.x -= Time.deltaTime / playerRB.velocity.x;
            }
            else
            {
                cameraOffset.x = startCamOffset.x;
            }
        }else if(!goingRight && playerRB.velocity.x > -maxLimitX){
            if (cameraOffset.x < startCamOffset.x)
            {
                cameraOffset.x -= Time.deltaTime / playerRB.velocity.x;
            }
            else
            {
                cameraOffset.x = startCamOffset.x;
            }
        }

    }
    */

}
