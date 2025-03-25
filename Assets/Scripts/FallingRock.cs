using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    //Settings for the Rock falling and respawning
    [SerializeField]private GameObject[] rockObj;
    [SerializeField]private Rigidbody2D[] rockRb;
    [SerializeField]private Transform[] rockStartPos;
    [SerializeField]private RockObjScript[] rockScript;
    [SerializeField] private float[] gravityScales;
    private bool isRockFalling;
    private bool initialRockFall;
    public float rockFallDelay, rockShakingTime, respawnTimer, rockRespawnTime;
    private float secDelay;

    //Raycast settings
    public RaycastHit2D[] rockCast;
    [SerializeField]private float[] rockCastDistance;
    [SerializeField]private LayerMask rockMask;

    //Animations
    [SerializeField]private Animator[] rockAnims;
    public AnimationClip[] rockAnimClip;//Shaking, Disassemble, Respawn {0, 1, 2}

    [SerializeField]private Animation[] lights;



    private void Start() {

        for (int i = 0; i < rockObj.Length; i++)//Makes sure all of the rocks have not fallen.
        {
            rockRb[i] = rockObj[i].GetComponent<Rigidbody2D>();
            rockAnims[i] = rockObj[i].GetComponent<Animator>();
            rockObj[i].transform.position = (Vector2)rockStartPos[i].position;
            rockScript[i].hasRockFallen = false;
        }

        rockCast = new RaycastHit2D[rockObj.Length];//Adds the amount of ray cast needed for each rock
    }


    private void Update()
    {

        if (!initialRockFall)
        {
            for (int i = 0; i < rockObj.Length; i++)//Constantly changes it's status wether it has fallen or not
            {
                if (!isRockFalling)
                {
                    rockRb[i].bodyType = RigidbodyType2D.Static;
                    rockAnims[i].SetBool("Idle", true);
                }
            }
        }

        for (int i = 0; i < rockObj.Length; i++)
        {
            if (rockCast[i].collider == null)
            {
                rockRb[i].bodyType = RigidbodyType2D.Static;
                rockScript[i].hasRockFallen = true;
            }else{
                rockScript[i].hasRockFallen = false;
            }
        }
    }

    private IEnumerator InitialRockFalling(){
        
        isRockFalling = true;

        if (!allRocksFallen())
        {
            for (int i = 0; i < rockAnims.Length; i++)
            {
                if (!rockScript[i].hasRockFallen)
                {
                    rockAnims[i].SetBool("Idle", false);
                    rockAnims[i].SetBool("Shaking", true);
                }
            }
        }

        yield return new WaitForSeconds(rockShakingTime);//Shake

        for (int i = 0; i < rockScript.Length; i++)
        {//Goes through each rock and checks to see if it has already fallen before turning its body type to dynamic

            //do Animation for the rock to start falling
            yield return new WaitForSeconds(rockFallDelay);//Extra delay
            rockAnims[i].SetBool("Shaking", false);
            rockAnims[i].SetBool("Idle", true);
            if (!rockScript[i].hasRockFallen)
            {
                rockRb[i].bodyType = RigidbodyType2D.Dynamic;
                rockRb[i].gravityScale = gravityScales[i];
            }
        }

        yield return new WaitUntil(() => allRocksFallen());
        for (int i = 0; i < rockScript.Length; i++)
        {
            yield return new WaitForSeconds(1f);
            lights[i].Play();
            yield return new WaitForSeconds(0.3f);
        }
        
        initialRockFall = true;
    }
    

    private bool allRocksFallen(){//Checks if all of the rocks have fallen
        for (int i = 0; i < rockScript.Length; i++)
        {
            if (!rockScript[i].hasRockFallen)
            {
                return false;
            }
        }

        return true;
    }

    private void FixedUpdate()
    {
        
        for (int i = 0; i < rockObj.Length; i++)
        {
            //Goes through each raycastHit variable in the array turning them into usable ray cast for its own specific rock
            rockCast[i] = Physics2D.Raycast(rockStartPos[i].transform.position, Vector2.down, rockCastDistance[i], rockMask);
        }
        

    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < rockObj.Length; i++)
        {
            //Makes each raycast show in the inspector
            Gizmos.DrawRay(rockStartPos[i].transform.position, Vector2.down * rockCastDistance[i]);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!initialRockFall)
            {
                //Starts the entire process of a rock falling and respawning
                StartCoroutine(InitialRockFalling());
            }
        }
    }

}