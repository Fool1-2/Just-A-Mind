using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Timeline;

public class SpaceBubble : MonoBehaviour
{
    private Animation anim;
    private bool isAnimPlaying;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    private void OnGUI() {
        Event e = Event.current;
        if (e.isKey)
        {
            if (!isAnimPlaying)
            {
                anim.Play();
                isAnimPlaying = true;
            }
        }
    }
    
}
