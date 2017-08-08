using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{

    public int maxCharacters = 3; 
    public int characterNum = 0; /

    private GameObject[] characters;

    // Use this for initialization
    void Start()
    {
        characters = new GameObject[maxCharacters];
        //like this maybe:
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = GameObject.Find("Character" + (i + 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (characterNum < (maxCharacters - 1))
            {
                characterNum++;
                characters[characterNum - 1].GetComponent<CharacterCombat>().enabled = false;
                characters[characterNum].GetComponent<CharacterCombat>().enabled = true;
            }
            else
            {
                characterNum = 1;
                characters[maxCharacters - 1].GetComponent<CharacterCombat>().enabled = false;
                characters[characterNum].GetComponent<CharacterCombat>().enabled = true;
            }
        }
        //end added
    }
}
