using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButton : MonoBehaviour
{
    [SerializeField]private Animation anim;
    private bool isVisible;//Checks if the sprite is visible

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI() {
        Event e = Event.current;
        if (e.isKey && !isVisible)
        {
            anim.Play();
            isVisible = true;
        }
    }
}
