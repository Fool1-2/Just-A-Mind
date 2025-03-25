using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScenes : MonoBehaviour
{
    [SerializeField]private int sceneNum;

    [SerializeField]private Animator anim;
    [SerializeField]private int shots;
    [SerializeField]private int maxShots;
    private float fadeInDelay;
    [SerializeField]private float endDelay;

    [SerializeField]private AnimationClip fadeIn;
    [SerializeField]private AnimationClip fadeOut;
    [SerializeField]private Animation thingabob;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cutscene());
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger("Shots", shots);
    }

    private IEnumerator Cutscene(){
        fadeInDelay = fadeIn.length;
        thingabob.Play("ThingabobStart");
        yield return new WaitForSeconds(fadeInDelay);
        for (int i = 0; i < maxShots; i++)
        {
            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Space));
            shots += 1;
        }
        yield return new WaitForSeconds(endDelay);
        fadeInDelay = fadeOut.length;
        thingabob.Play("ThingabobEnd");
        yield return new WaitForSeconds(fadeInDelay);
        SceneManager.LoadScene(sceneNum);
    }

}
