using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{

    #region General
    [Header("General")]
    Abilities abilityScript;
    public KeyCode formChangeKey;
    public KeyCode rightformChangeKey;
    [SerializeField]public bool devControl;//Just used to override the locked forms(I got really lazy and I dont want to keep going back and fourth changing the bools)
    public int neareastSpawner;
    public float timer;
    [SerializeField]public int maxTime; //Can adjust the time it takes for text to appear accordingly
    public float textOffsetX;
    public float textOffsetY;
    public Transform spherePoint;
    public TestManager gm;
    [SerializeField]private SpriteRenderer playerSpriteRender;
    [SerializeField]private Sprite[] playerFormSprite;
    [SerializeField]private Animator anim;
    public float jumpTime;
    public AudioManagerScript AMS;

/*    public bool musicHasChangedOne = false;
    public bool musicHasChangedTwo = false;*/
    public GameObject soundTrigger;
    public GameObject soundTriggerTwo;

    private static bool playerDead;

    public Collider2D circleCol; // checks for all colliders
    public Collider2D vineCol;
    

    public GameObject spawner;
    public GameObject player;
    public GameObject grabOn;

    public TMP_Text guideText;
    public Image thoughtBub;
    [SerializeField] GameObject thoughtBubble;
    public IEnumerator thoughtBubbleTime;

    IEnumerator playingSound;
    private bool soundIsPlaying;
    [SerializeField]public CamControllerV2 cam;
    #endregion
    #region movements
    [Header("Movement")]
    public bool canMove = true;
    [SerializeField]private bool isMoving;
    public Rigidbody2D rb;
    public float horizontal, vertical;
    public int horiLatestInput = 1, vertLatestInput = 0;
    public float speed,jumpSpeedX,jumpSpeedY;
    [SerializeField]private float bonusRotationSpeed;
    [SerializeField]private float rotChangePointMax;//The max amount rb rotation can get to before giving a boost when changing dir
    [SerializeField]private float rotChangePointMin;//The minimal amount rb rotation can get to before stoping the boost when changing dir
    private bool canBoostRotSpeed;

    [Header("Interaction")]
    private Collider2D interactCol;
    [SerializeField]public float interactRadius;
    [SerializeField]public LayerMask interactMask, groundMask;//interact mask is for objects you can interact with by pressing E. Ground is for ground
    
    
    [Header("Player Forms")]
    public int maxForm;
    public int curForm;
    public enum playerForms{Ball, Pogo}
    public static playerForms playerForm;
    // Change to false false false for game and initialize in gm
    public static bool[] playerPieces = {true, false};//bools for the player pieces {0: ball, 1: pogo, 2: arm}

    [Header("Physics")]
    public bool ignoreResistences = false;
    [SerializeField] float coefficientOfAirResistence, coefficientOfFriction;
    public isGroundedScript groundedScript;

    #region Ball movement variables

    [Header("Head")]
    [SerializeField]private Collider2D ballCol;

    #endregion

    #region Pogo movement variables

    [Header("Body")]
    [SerializeField]private Collider2D pogoCol;
    public IEnumerator jumping;
    public bool canJump = true;
    
	#endregion

	#region Arm movement variables

    [Header("Arm")]
    [SerializeField]private GameObject leftArm;
    [SerializeField]private GameObject rightArm;
    public bool hasArms;

	#endregion


	#endregion

	private void Start(){
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControllerV2>();
        groundedScript = GameObject.FindGameObjectWithTag("GroundRay").GetComponent<isGroundedScript>();
        playerSpriteRender = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        abilityScript = GetComponent<Abilities>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerForm = playerForms.Ball;
        FormSettings();
        AMS = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        try
        {
            thoughtBub = GameObject.FindGameObjectWithTag("ThoughtBubble").GetComponent<Image>();
        }
        catch (System.Exception)
        {
            return;
        }
        if (thoughtBub != null)
        {
            thoughtBub.enabled = false;
        }else{
            Debug.LogError("Thought bubble variable is empty.");
            return;
        }

        if (!devControl)
        {
            playerPieces[0] = true;
            playerPieces[1] = false;
        }
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<TestManager>();
        if (gm == null)
        {
            return;
        }

        //gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //guideText.text = "";

        soundTrigger.SetActive(true);
        soundTriggerTwo.SetActive(true);

    }
        
    // Update is called once per frame
    void Update()
    {
        RespawnParse();
        PlayerStopMoving();

        if (canMove) 
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            Movements();
        }

        if (Input.GetKeyDown(formChangeKey) || Input.GetKeyDown(rightformChangeKey))
        {
            if (!devControl)
            {
               
                if (curForm >= maxForm)
                {
                    curForm = 0;
                }
                else
                {
                    curForm++;
                }
                ChangeForm(curForm);//Controlls the changing of the players form
            }
            else
            {
                curForm++;
                if (curForm >= 2)//Had to change for current build
                {
                    curForm = 0;
                }
                ChangeForm(curForm);//Controlls the changing of the players form
            }
            print(curForm);
        }

        LatestInput((int)horizontal, (int)vertical);
        
    }

    private void FixedUpdate() {

        if (canMove){
            interactCol = Physics2D.OverlapCircle(transform.position, interactRadius, interactMask);

            if (!ignoreResistences)
            {
                if (groundedScript.isGrounded())
                {
                    Friction();
                }
                else
                {
                    AirResistance();
                }
            }
        }

        if (rb.velocity.x > 1.5f || rb.velocity.x < -1.5)//the game will register if it is moving when its past a certain speed
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        
    }

    private void LatestInput(int horizontalInput, int verticalInput){//Finds the latest input for vertical and horizontal
        if (horizontalInput != 0)
        {
            int i = horizontalInput;
            horiLatestInput = i;
        }
        else
        {
            horiLatestInput = 0;
        }

        if (verticalInput != 0)
        {
            int i = verticalInput;
            vertLatestInput = i;
        }
        else
        {
            vertLatestInput = 0;
        }
    }

    public void ChangeForm(int playerFormNum)
    {
        playerForm = (playerForms)playerFormNum;
        FormSettings();

    }
    void FormSettings(){//defualt settings for each form(mainly for the sprites of each form)
            switch (playerForm)
            {
                case playerForms.Ball:
                    //Sets the balls sprite, unfreezes rotation, and changes the animation
                    curForm = 0;
                    rb.mass = 1;
                    ballCol.enabled = true;
                    pogoCol.enabled = false;
                    playerSpriteRender.sprite = playerFormSprite[0];
                    anim.enabled = false;
                    rb.freezeRotation = false;
                    try
                    {
                        leftArm.SetActive(false);
                        rightArm.SetActive(false);
                    }
                    catch (System.Exception)
                    {
                        
                        Debug.LogError("Left Arm and Right arm are not assigned. If not using them do not mind this message");
                        return;
                    }
                    break;

                case playerForms.Pogo:
                    curForm = 1;
                    rb.mass = 1f;
                    rb.freezeRotation = true;
                    transform.rotation = quaternion.RotateZ(0);//Puts the character up straight
                    ballCol.enabled = false;//changes the collider from ball to pogo
                    pogoCol.enabled = true;
                    //anim.enabled = true;
                    playerSpriteRender.sprite = playerFormSprite[1];//changes the sprites from ball to pogo man
                    anim.SetInteger("Horizontal", (int)horizontal);//this is for walking animation 
                    canJump = true;
                    if (hasArms)
                    {
                        try
                        {
                            leftArm.SetActive(true);
                            rightArm.SetActive(true);
                        }
                        catch (System.Exception)
                        {
                            Debug.LogError("It seems as though you are trying to use arms however YOU DO NOT HAVE THE ARMS IN THE VARIABLE GAMEOBECT CALLED LEFT ARM AND RIGHT ARM - Elyjah Justice Logan");
                            return;
                        }
                    }
                    break;
            }
        }
        

    private void Movements()
    {//different movements for each form
		switch (playerForm)
		{
			case playerForms.Ball:
                rb.AddForce(new Vector2(horizontal * speed * Time.deltaTime, 0), ForceMode2D.Impulse);//moves the player in the direction the player is pressing
                RotationSpeed();
                break;
			case playerForms.Pogo:
                if (horizontal != 0)
                {
                    if (groundedScript.isGrounded())
                    {
                        if (canJump)
                        {
                            canJump = false;
                            print("being called");
                            jumping = Jump();
                            StartCoroutine(jumping);
                        }
                    }
                    else{
                        rb.AddForce(new Vector2(horizontal * speed * Time.deltaTime, 0), ForceMode2D.Impulse);
                    }
                }
                break;
			default:
				break;
		}

	}

    public IEnumerator Jump() 
    {
        Vector2 jumpForce = new Vector2(horizontal * jumpSpeedX, jumpSpeedY);
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
		yield return new WaitUntil (() => groundedScript.isGrounded());
		canJump = true;
    }

    void AirResistance()
    {
        // Air resistance opposes motion
        int OppositedirectionMultipleX = -1 * Mathf.RoundToInt(rb.velocity.x / Mathf.Abs(rb.velocity.x));
        int OppositedirectionMultipleY = -1 * Mathf.RoundToInt(rb.velocity.y / Mathf.Abs(rb.velocity.y));
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfAirResistence * (rb.velocity.x * rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfAirResistence * (rb.velocity.y * rb.velocity.y)));
    }
    void Friction()
    {
        // Air resistance opposes motion but in ball motion is reversed because rotation
        // Grabs the sign of velocity and multiplies it by -1 to get opposite
        int OppositedirectionMultipleX = -1 * Mathf.RoundToInt(rb.velocity.x / Mathf.Abs(rb.velocity.x));
        int OppositedirectionMultipleY = -1 * Mathf.RoundToInt(rb.velocity.y / Mathf.Abs(rb.velocity.y));
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfFriction * Mathf.Abs(rb.velocity.x * rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfFriction * Mathf.Abs(rb.velocity.y * rb.velocity.y)));
    }

    void RespawnParse()
    {
        Collider2D[] circleCols = Physics2D.OverlapCircleAll(spherePoint.transform.position, interactRadius, interactMask);
		for (int i = 0; i < circleCols.Length; i++)
		{
            Collider2D circleCol = circleCols[i];
			if (circleCol == spawner || circleCol == null)
			{
                continue; 
			}

            spawner = circleCol.gameObject;
            break;
		}

    }

    private IEnumerator PlaySound(float waitAmount){//Plays the sound and waits until it is finished + however amount you want to add
        AMS.sfx.clip = AMS.currentSfx;
        AMS.sfx.Play();
        yield return new WaitForSeconds(AMS.sfx.clip.length + waitAmount);
        soundIsPlaying = false;
    }

    private void RotationSpeed(){

        bonusRotationSpeed = -(rb.angularVelocity/2);
        
        if (!groundedScript.isGrounded())
        {
            if(horizontal == 1){
                if (rb.angularVelocity > 0.02f)
                {
                    rb.angularVelocity -= -bonusRotationSpeed * Time.fixedDeltaTime * 10f;
                }
            }else if(horizontal == -1){
                if (rb.angularVelocity < -0.02f)
                {
                    rb.angularVelocity += bonusRotationSpeed * Time.fixedDeltaTime * 10f;
                }
            }
        }
        
        
        switch (horizontal)
        {
            case 1:

                if(canBoostRotSpeed){
                    if (rb.angularVelocity < rotChangePointMin){
                        rb.angularVelocity -= -bonusRotationSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        canBoostRotSpeed = false;
                    }
                }

                if (rb.angularVelocity < -rotChangePointMax)
                {
                    canBoostRotSpeed = true;
                }
                break;

            case -1:

                if (canBoostRotSpeed)
                {
                    if(rb.angularVelocity > rotChangePointMin){
                        rb.angularVelocity += bonusRotationSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        canBoostRotSpeed = false;
                    }
                }

                if (rb.angularVelocity > rotChangePointMax)
                {
                    canBoostRotSpeed = true;
                }
                break;
            
        }
    }

    private void OnDrawGizmos()  
    {
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
	{
		switch (collision.gameObject.tag)
		{
            case "Spike":
                Debug.Log("dead");
                StartCoroutine(PlayDead());
                rb.velocity = Vector3.zero;
                rb.angularVelocity = 0;
                break;
            
            
		}
	}

    private void OnTriggerEnter2D(Collider2D other) {//For level 3 death valley
        switch (other.gameObject.tag)
		{
            case "Spike":
                Debug.Log("dead");
                StartCoroutine(PlayDead());
                //rb.velocity = Vector3.zero;
                rb.angularVelocity = 0;
                break;
		}
    }

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Guide")
		{
			timer += Time.deltaTime;
			if (timer >= maxTime)
			{
                thoughtBub.enabled = true;
            }
			else
			{
                thoughtBub.enabled = false;
			}
		}
		if (collision.tag == "sfx")
		{
            if (!soundIsPlaying && isMoving)
            {
                playingSound = PlaySound(0.6f);
                StartCoroutine(playingSound);
                soundIsPlaying = true;
            }
            
		}
        if(collision.tag == "MusicChange" )
        {
           AMS.soundTrackSource.Stop();
           AMS.currentMusic = AMS.soundTrack[1];
           AMS.soundTrackSource.PlayOneShot(AMS.currentMusic);
           AMS.soundTrackSource.volume = 0.55f;
           soundTrigger.SetActive(false);
           

        }

        if (collision.tag == "MusicChange2")
        {
            AMS.soundTrackSource.Stop();
            AMS.currentMusic = AMS.soundTrack[2];
            AMS.soundTrackSource.PlayOneShot(AMS.currentMusic);
            AMS.soundTrackSource.volume = 0.55f;
            soundTriggerTwo.SetActive(false);


        }


    }

	private void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.tag == "Guide")
        {
            thoughtBub.enabled = false;
            timer = 0;
        }

        if (collision.tag == "sfx")
        {
            StopCoroutine(playingSound);
            soundIsPlaying = false;
        }

	}

    private void PlayerStopMoving() 
    {
		if (playerDead)
		{
            canMove = false;
		}
		if (!playerDead)
		{
            canMove = true;
		}
    }

    public IEnumerator PlayDead() 
    {
        playerDead = true;
        StartCoroutine(gm.RespawnLevel3());
        yield return new WaitUntil(() => groundedScript.isGrounded());
        canMove = true;
        playerDead = false;

    }


}