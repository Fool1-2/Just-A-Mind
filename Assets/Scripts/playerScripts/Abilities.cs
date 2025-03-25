using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    [Header("General Variables")]
    private PlayerController player;
    public KeyCode abilityKey;
   
    private int bonusCharges;//Mechanic that will never be used...
    isGroundedScript groundedScript;

    #region Dashing variables

    [Header("Dash Variables")]
    
    [SerializeField]private float dashingDuration;//How long the dash will go for
    [SerializeField] float DASHPOWER;
    public static bool isDashing;
    [SerializeField]private int maxDashAmount;
    private int dashAmount;
    
    [SerializeField]float dashDelay;

    [SerializeField]float yDashModifier;
    [SerializeField] float dashInputForgivenessTime = 0.2f;
    bool tryingToDash;
    float attemptingToDashTimer;
    #endregion


    #region Pogo Jumpy up up variables

    [Header("Pogo Variables")]
    public float jumpForce;
    public float superJumpForce;
    public Transform groundPoint;
    public bool canSuperJump = false;
    public float keyHoldDown;//Amount the jump is held down
    public float coyoteTimeVar = 0.2f;
    public float coyotoeTimer;

    #endregion
    
    #region Arm Variable

    [Header("Arms Variables")]

    [SerializeField]private HingeJoint2D hinge;
    private Collider2D armCol;
    [SerializeField]private LayerMask vineLayer;
    [SerializeField]private float armColRadius;

    public bool isConnected;

    [SerializeField]private float boostX, boostY, grabBoostX, grabboostY;//For the boost of the swing
    [SerializeField]private Vector2 rightSide, leftSide;
    private Vector2 side;//The offical position of where the hinge anchor is going to be

    #endregion
    
    private void Start() {
        player = GetComponent<PlayerController>();
        dashAmount = maxDashAmount;
        isDashing = false;
        //player.rb.centerOfMass = COM;
        groundedScript = GameObject.FindGameObjectWithTag("GroundRay").GetComponent<isGroundedScript>();
        hinge = GetComponent<HingeJoint2D>();
        // grab arms

    }


    void Update()
    {

        // We must call in update because input breaks if we dont
        switch (PlayerController.playerForm)
        {
            case PlayerController.playerForms.Ball:
                Dash();
                break;
            case PlayerController.playerForms.Pogo:
                //Have the functions for pogos abilities
                Jumping();
                if (player.hasArms)
                {
                    Grab();
                }
                break;
        }
        
        if (isConnected)
        {
            if (Input.GetKeyDown(player.formChangeKey))
            {//When the player switches while swinging they will detatch
                StartCoroutine(Swinging());
            }
        }
    }

    IEnumerator ignoreResistences()
    {
        player.ignoreResistences = true;
        yield return new WaitForSeconds(.25f);
        player.ignoreResistences = false;
    }
    
    #region Ball abilites
    private void Dash(){
        if (!TestManager.transitioned)
        {
            if (Input.GetKeyDown(abilityKey))
            {
                tryingToDash = true;
                attemptingToDashTimer = 0;
            }
            if (tryingToDash)
            {
                attemptingToDashTimer += Time.deltaTime;
                if (attemptingToDashTimer > dashInputForgivenessTime)
                {
                    tryingToDash = false;
                }
            }
            if (tryingToDash && !isDashing && player.horiLatestInput != 0)
            {
                if (dashAmount > 0 || bonusCharges > 0)
                {
                    StartCoroutine(Dashing(dashingDuration));
                    StartCoroutine(ignoreResistences());
                }
            }
        }
    }
    
    private IEnumerator Dashing(float duration){//Will push the player forward for a certain amount of time at a certain amount of speed
        // Starts camera shaking
        //player.cam.shakeTime = 0.2f;
        //player.cam.shakeAmount = 0.2f;
        //CamControllerV2.isCameraShaking = true;
        if (groundedScript.isGrounded())
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
        }
        if (player.horizontal == 1)
        {
            player.rb.angularVelocity += 300 * player.horizontal;
        }
        else if(player.horizontal == -1)
        {
            player.rb.angularVelocity -= 300 * player.horizontal;
        }
        // We then add the dash force
        player.rb.AddForce(new Vector2(player.horiLatestInput * DASHPOWER, yDashModifier * DASHPOWER), ForceMode2D.Impulse);//dash in any direction boom kachow
        isDashing = true;        
        // wait then turn off cammera shake
        yield return new WaitForSeconds(duration);
        yield return new WaitUntil(() => groundedScript.isGrounded());
        ResetDash();
        // after we reset the dassh we can then transition to an arm boost which may allow more arier movement
    }
    private void ResetDash(){//Resets all of the variables in dash mechanic
        isDashing = false;
    }
    #endregion
    
    #region Pogo abilities
    void Jumping()
    {
        // Only allows if the player is grounded which
        if (groundedScript.isGrounded())
        {
            coyotoeTimer = coyoteTimeVar;
        }
		else
		{
            coyotoeTimer -= Time.deltaTime; 
		}

        if (coyotoeTimer > 0f && Input.GetKeyDown(abilityKey))
        {
            StartCoroutine(ignoreResistences());
            player.rb.AddForce(new Vector2(0, superJumpForce), ForceMode2D.Impulse);
            coyotoeTimer = 0f;
        }

        /*IEnumerator debugger()
        {
            for(int i = 0; i < 100; i++)
            {
                Debug.Log(this.GetComponent<Rigidbody2D>().velocity.y);
                yield return null;
            }
        }*/
    }

    #endregion


    #region Arm Ability

    private void Grab(){

        if (armCol != null)
        {
            if (IsVineDirRight(armCol.transform.position))
            {
                side = rightSide;
            }
            else{
                side = leftSide;
            }

            if (Input.GetKeyDown(abilityKey))
            {
                StartCoroutine(Swinging());
            }

            if (!isConnected)
            {
                if (player.curForm == 1)
                {
                    player.rb.freezeRotation = true;
                }
            }
            else
            {
                player.rb.freezeRotation = false;
            }
        }
    }

    IEnumerator Swinging(){
        if (!isConnected)
        {
            if (!groundedScript.isGrounded())
            {
                player.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
                hinge.enabled = true;
                hinge.autoConfigureConnectedAnchor = false;
                hinge.useLimits = true;
                Vector2 vec = armCol.GetComponent<vinetest>().transformTest.localPosition;
                hinge.connectedBody = armCol.GetComponent<Rigidbody2D>();
                hinge.anchor = side;
                hinge.connectedAnchor = vec;
                armCol.GetComponent<vinetest>().onVine = true;
                player.rb.AddForce(new Vector2(player.horizontal * grabBoostX, grabboostY), ForceMode2D.Impulse);
                isConnected = !isConnected;
                    
                yield return new WaitForEndOfFrame();
                StopCoroutine(Swinging());
            }
        }
        else
        {

            hinge.connectedBody = null;
            hinge.enabled = false;
            player.rb.AddForce(new Vector2(player.horizontal * boostX, boostY), ForceMode2D.Impulse);
            player.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            armCol.GetComponent<vinetest>().onVine = false;
            isConnected = !isConnected;
            StopCoroutine(Swinging());
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
        armCol = Physics2D.OverlapCircle(transform.position + new Vector3(0, .5f, 0), armColRadius, vineLayer);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, .5f, 0), armColRadius);
    }


    #endregion

}
