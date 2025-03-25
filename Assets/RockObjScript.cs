using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockObjScript : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [SerializeField]private Transform startPos;
    [SerializeField]private float respawnDelayTimer;
    [SerializeField]private FallingRock fallingRockScript;
    public bool hasRockFallen;

    [SerializeField]private float rockFallDelay;
    [SerializeField]private float gravityScale;

    [SerializeField]private Collider2D bodyCol, groundCol;//Ignore the grounds collider

    [SerializeField]private GameObject lightObj;


    private void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update() => Physics2D.IgnoreCollision(bodyCol, groundCol);
    private void FixedUpdate() => Physics2D.IgnoreCollision(bodyCol, groundCol);

    private IEnumerator Respawn(){
        yield return new WaitForSeconds(respawnDelayTimer);
        anim.SetBool("Idle", false);
        anim.SetTrigger("Disassemble");
        yield return new WaitForSeconds(fallingRockScript.rockAnimClip[1].length);
        lightObj.SetActive(false);
        transform.position = startPos.position;
        hasRockFallen = false;

        anim.ResetTrigger("Disassemble");
        anim.SetTrigger("Respawn");
        yield return new WaitForSeconds(fallingRockScript.rockAnimClip[2].length);
        anim.ResetTrigger("Respawn");
        //After the rock respawns

        anim.SetBool("Shaking", true);
        anim.SetBool("Idle", false);
        yield return new WaitForSeconds(rockFallDelay);
        anim.SetBool("Shaking", false);
        anim.SetBool("Idle", true);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;
        StopCoroutine(Respawn());
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(Respawn());
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(Respawn());
        }
    }
}
