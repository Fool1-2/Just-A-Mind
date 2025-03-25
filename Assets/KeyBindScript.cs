using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyBindScript : MonoBehaviour
{
    public GameObject player;
    public GameObject currentFormKey;
    public GameObject currentAbilityKey;
    public PlayerController pc;
    public Abilities ab;
    public TMP_Text ability, form;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pc = player.GetComponent<PlayerController>();
        ab = player.GetComponent<Abilities>();

        ability.text = pc.formChangeKey.ToString();
        form.text = ab.abilityKey.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        if (currentFormKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                pc.formChangeKey = e.keyCode;
                form.text = pc.formChangeKey.ToString();
                currentFormKey = null;
            }

			
        }

		if (currentAbilityKey != null)
		{
            Event e = Event.current;
            if (e.isKey && e.keyCode != pc.formChangeKey)
            {
                ab.abilityKey = e.keyCode;
                ability.text = ab.abilityKey.ToString();
                currentAbilityKey = null;
            }
        }
    }


    public void ChangeFormKey(GameObject clickedForm) 
    {
        currentFormKey = clickedForm;
    }

    public void ChangeAbilityKey(GameObject clickedAbility) 
    {
        currentAbilityKey = clickedAbility;
    }
}
