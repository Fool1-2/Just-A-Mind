using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSet : MonoBehaviour
{
    private CamControllerV2 cam;
    [SerializeField]private Transform camPos;

    #region Collider
    private float camRadius;
    [SerializeField]private Vector2 camVec;
    private Collider2D camHitCol;
    [SerializeField]private LayerMask playerMask;
    #endregion
    
    [SerializeField]private float zoomCameraAmount, zoomCameraSpeed, zoomBackCameraSpeed, followSpeed, comingBackSpeed;
    [SerializeField]private bool followPlayer, keepChanges;
    public bool activeController;


    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControllerV2>();
        activeController = false;
        if (camPos == null)
        {
            camPos = this.transform;
        }
        else{
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (camHitCol != null)
        {   
            activeController = true;
            cam.hasZoomed = false;
            cam.setBack = false;
            if (activeController)
            {
                if (!followPlayer)
                {
                    cam.isFollowingPlayer = false;
                    cam.objSpeed = followSpeed;
                    cam.comingBackSpeed = comingBackSpeed;
                    cam.objTarget = camPos;
                    cam.isComingBack = true;
                }
                cam.isZoom = true;
                cam.ZoomCameraChange(zoomCameraAmount, zoomCameraSpeed);
            }
            //cam.isComingBack = true;
            
        }else{

            if (activeController)
            {
                cam.isZoom = false;
                cam.isFollowingPlayer = true;
                if (!keepChanges)
                {
                    cam.setBack = true;
                    cam.setBackSpeed = zoomBackCameraSpeed;
                }
                activeController = false;
            }

            
        }
    }

    private void FixedUpdate() => camHitCol = Physics2D.OverlapBox(transform.position, camVec, camRadius, playerMask);

    private void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, camVec);
}
