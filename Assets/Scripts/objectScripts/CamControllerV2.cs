using System.Transactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControllerV2 : MonoBehaviour
{
    [HideInInspector]public Camera cam;
    
    [SerializeField]private float speedToTarg;//How fast it goes to the target(Smaller it is, the faster)
    [HideInInspector]public float objSpeed, comingBackSpeed;
    private Vector3 vel = Vector3.zero;//SmoothDamp calls reference to this.(Don't need to worry about it)
    [SerializeField]private Vector3 offset = new Vector3(0, 0, -10);
    public float camDefaultFOV;
    [SerializeField]private float startingCamFOV;//For when we want to start a scene in a different FOV than the defualt will be.

    public bool notFollowingX, notFollowingY;//For borders
    public float leftLimit, rightLimit, upLimit, downLimit;

    #region Targets for camera
    [HideInInspector]public Transform playerTarget;
    public Transform objTarget;
    private Vector3 target;
    #endregion

    public bool isFollowingPlayer, isZoom, isComingBack;
    public bool hasZoomed;
    public bool setBack;
    [HideInInspector]public float setBackSpeed;
    public static bool isCameraShaking;
    public float shakeAmount, shakeTime;


    


    // Start is called before the first frame update
    void Start()
    {
        setBack = false;
        playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        cam = GetComponent<Camera>();

        if (startingCamFOV == camDefaultFOV || startingCamFOV == 0)
        {
            cam.orthographicSize = camDefaultFOV;
        }else{
            cam.orthographicSize = startingCamFOV;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isCameraShaking)
        {
            StartCoroutine(CameraShake(shakeTime, shakeAmount));
        }

        if (setBack)
        {
            ZoomCameraChange(camDefaultFOV, setBackSpeed);
        }

        if (isFollowingPlayer)
        {
            if (Vector2.Distance(transform.position, playerTarget.position) < 0.5f)
            {
                isComingBack = false;
            }

            if (Vector2.Distance(transform.position, playerTarget.position) > 2f)
            {
                comingBackSpeed -= 0.009f;//If the player is to far than it will gradually increase the speed of the camera
            }

            if (!isComingBack)
            {
                FollowPlayer(speedToTarg);
            }
            else{
                FollowPlayer(comingBackSpeed);
            }
        }
        else{
            FollowObj(objSpeed);
        }
    }

    private void FollowPlayer(float speed){

        target = playerTarget.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, speed);
        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, leftLimit, rightLimit),
            Mathf.Clamp(transform.position.y, downLimit, upLimit),
            transform.position.z
        );
    }

    private void FollowObj(float speed){
        target = objTarget.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, speed);
        //For borders just do new vector3 for both transform.position and target
    }

    public void ZoomCameraChange(float FOV, float zoomSpeed){//Zooms back and fourth wether it is the player or not. Never make the desired FOV smaller than the defualt FOV which is 5
        
        if (Mathf.Abs(cam.orthographicSize - FOV) < 0.1f)
        {
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
            }
            else
            {
                cam.orthographicSize = FOV;
            }
        }
        else
        {
            hasZoomed = false;
            if (setBack)
            {
                if (cam.orthographicSize > camDefaultFOV)
                {
                    cam.orthographicSize -= Time.deltaTime * zoomSpeed;
                }else if(cam.orthographicSize < camDefaultFOV){
                    cam.orthographicSize += Time.deltaTime * zoomSpeed;
                }
            }

            if (Mathf.Abs(cam.orthographicSize - camDefaultFOV) < 0.01f)
            {
                cam.orthographicSize = camDefaultFOV;
            }
        }
        
    }

    public IEnumerator CameraShake(float shakeTime, float shakeAmount)//Shake camera.
    {
    
        if (isCameraShaking){
            transform.position = (Vector2)transform.position + Random.insideUnitCircle * shakeAmount;
            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
            yield return new WaitForSeconds(shakeTime);
            isCameraShaking = false;
        }
        
    }
}
