using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{

    public static bool transitioned;
    public GameObject buttonCotainer;
    [SerializeField]private Animator transitionAnim;
    [SerializeField]private AnimationClip start, end;
    [SerializeField]private int sceneNum;

    [Header("Pause Menu")]
    [SerializeField]private GameObject pauseMenu, exitBall;
    private bool isPaused, exit, restarting, buttonConfig;
    [SerializeField]private AnimationClip mainMenuTransition;

    [Header("Player")]
    public PlayerController pc;
    public GameObject player;

    [Header("Audio")]
    public GameObject musicChanger;
    public GameObject musicChangerTwo;

    [Header("Level 3 Respawn")]
    [SerializeField]private Animator respawnAnim;
    [SerializeField]private AnimationClip respawnStart, respawnEnd;


    
    // Start is called before the first frame update
    
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        respawnAnim.gameObject.SetActive(false);
        //buttonCotainer = GameObject.Find("ContentArea");
        //buttonCotainer.SetActive(false);
        exitBall.SetActive(false);
        buttonConfig = false;

        try
        {
            musicChanger.SetActive(true);
        }
        catch (System.Exception)
        {
            return;
        }
        musicChangerTwo.SetActive(true);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        if (exit)
        {
            StartCoroutine(ExitTransition());
        }

        if (restarting)
        {
            StartCoroutine(Transition(SceneManager.GetActiveScene().buildIndex));
            restarting = false;
        }

        Debug.Log(isPaused);

		
    }

    private void OnTriggerEnter2D(Collider2D other) {
        try
        {
            StartCoroutine(Transition(sceneNum));
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    private IEnumerator Transition(int scene){
        transitionAnim.SetTrigger("Transition");
        transitioned = true;
        yield return new WaitForSeconds(start.length);
        SceneManager.LoadScene(scene);
        transitioned = false;
    }

    public IEnumerator RespawnLevel3(){
        respawnAnim.gameObject.SetActive(true);
        yield return new WaitForSeconds(respawnStart.length);
        pc.canMove = false;
        player.transform.position = pc.spawner.transform.position;
        yield return new WaitForSeconds(0.6f);
        respawnAnim.SetTrigger("RespawnEnd");
        yield return new WaitForSeconds(respawnEnd.length);
        respawnAnim.ResetTrigger("RespawnEnd");
        respawnAnim.gameObject.SetActive(false);

    }

    private IEnumerator ExitTransition(){
        exit = false;
        exitBall.SetActive(true);
        transitioned = true;
        yield return new WaitForSeconds(mainMenuTransition.length);
        SceneManager.LoadScene(0);
        transitioned = false;
    }

    public void Exit(){
        isPaused = false;
        if (!exit)
        {
            exit = true;
        }
    }

    public void Restart(){
        isPaused = false;
        if(!restarting){
            restarting = true;
        }
    }

    public void ButtonConfig() 
    {
        isPaused = false;
		if (!buttonConfig)
		{
            buttonConfig = true;
		}
    }

   /* public void MusicTriggerEnd()
    {
        if (pc.musicHasChangedOne)
		{
            musicChanger.SetActive(false);
		}
		if (pc.musicHasChangedTwo)
		{
           Destroy(musicChangerTwo);
		}
    }*/
   /* public void ButtonExit() 
    {
        buttonConfig = true;
		if (buttonConfig)
		{
            buttonCotainer.SetActive(false);
		}
    }*/
    
    
    
}
