using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private CameraScript cam;
    [SerializeField, Tooltip("zoomCamera is the FOV, zoom speed is how fast it zooms out, and follow speed is how fast it goes into the position of this obj")]
    private float zoomCameraAmount, zoomCameraSpeed, zoomBackCameraSpeed, followSpeed;
    [SerializeField]private bool followPlayer, keepChanges;
    public static bool activeController;

    private float camRadius;
    [SerializeField]private Vector2 camVec;
    private Collider2D camHitCol;
    [SerializeField]private LayerMask playerMask;


    // Start is called before the first frame update
    void Start()
    {
        try
        {
            cam = GameObject.Find("MainCameraHolder").GetComponent<CameraScript>();
        }
        catch (System.Exception)
        {
            Debug.LogError("The cameras parent is not named MainCameraHolder");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (camHitCol != null)
        {   
            activeController = true;
            if (!followPlayer)
            {
                cam.isFollowingPlayer = false;
                cam.FollowObjDelay(followSpeed, transform);
            }
            cam.isZoom = true;
            cam.ZoomCameraChange(zoomCameraAmount, zoomCameraSpeed);
            cam.isComingBack = true;
            
        }else{
            if (!activeController)
            {
                cam.isFollowingPlayer = true;
                cam.isZoom = false;
                if (!keepChanges)
                {
                    cam.ZoomCameraChange(cam.camDefaultFOV, zoomBackCameraSpeed);
                }
            }

            activeController = false;
            
        }
    }

    private void FixedUpdate() => camHitCol = Physics2D.OverlapBox(transform.position, camVec, camRadius, playerMask);

    private void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, camVec);
}
