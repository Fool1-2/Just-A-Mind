using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Animation transition;
    private AnimationClip transitionClip;

    public void PlayButton(int SceneNum) => SceneManager.LoadScene(SceneNum);

    /*
    private IEnumerator Transition(){
        transition.Play();
    }
    */
}
