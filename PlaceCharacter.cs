using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceCharacter : MonoBehaviour
{
    private GameObject character;
	private GameObject firstEnemy;
    private bool canPlace;
	private GameObject floor;
    private Grid grid;
	private IEnumerator coroutine;
	private IEnumerator coroutineRevert;

	public float speed = 10f;
    public Vector2 pos;
		
    public static int maxCharacters = 4; 
    public static int characterNum = 0; 
	public static GameObject[] characters;
   
    void Start()
    {
        character = gameObject;
        pos = transform.position;
        floor = GameObject.Find("Floor");
        grid = floor.GetComponent<Grid>();
        firstEnemy = GameObject.Find("M1");
        character.transform.GetChild(0).GetComponent<Renderer>().enabled = true;

        characters = new GameObject[maxCharacters];
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = GameObject.Find("Character" + (i + 1));
            
        }
    }

    void Update()
    {
        CharacterMove();
        CheckSpot();
        CharacterPlace();
        TileTaken(); 
    }

    void TileTaken()
    {
        for (int i = 0; i < grid.gridWidth; i++) {
            for (int j = 0; j < grid.gridHeight; j++) {
                foreach (GameObject character in characters) {
                    if (new Vector2(character.transform.position.x, character.transform.position.y) == new Vector2(i, j)) {
                        if (character != characters[characterNum])
                        {
                            if (character.transform.GetComponent<Renderer>().enabled == true)
                            {
                                grid.taken[i, j] = 1;//0 means its not taken, 1 means its taken
                            }
                            else 
                            {
                                grid.taken[i, j] = 0;
                            }
                        }                        
                    }
                }
            }
        }
    }

    void CharacterMove()
    {
        if (new Vector3(pos.x, pos.y, 0) == transform.position)
        {
            if (Input.GetButton("MoveUp") && pos.y < grid.gridHeight)
                pos.y += 1;

            else if (Input.GetButton("MoveDown") && pos.y > 1)
                pos.y -= 1;

            else if (Input.GetButton("MoveLeft") && pos.x > 1)
                pos.x -= 1;

            else if (Input.GetButton("MoveRight") && pos.x < 2)
                pos.x += 1;
        }
        transform.position = Vector2.MoveTowards(transform.position, pos, speed * Time.deltaTime);
    }
	
    void CheckSpot()
    {
        int tx = (int)Mathf.Round(pos.x);
        int ty = (int)Mathf.Round(pos.y);

        if (grid.taken[tx, ty] == 0)
        {
            canPlace = true;
            character.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            character.transform.GetChild(1).GetComponent<Renderer>().enabled = false;
        }
        else if (grid.taken[tx, ty] != 0)
        {
            canPlace = false;
            character.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            character.transform.GetChild(1).GetComponent<Renderer>().enabled = true;
        }
    }

    void CharacterPlace()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (new Vector3(pos.x, pos.y, 0) == transform.position)
            {
                if (canPlace)
                {
                    character.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
                    character.transform.GetChild(1).GetComponent<Renderer>().enabled = false;
                    PlaceNextCharacter();
                    
                }
            }
            else if (!canPlace)
                Debug.Log("Can't place");
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            character.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            character.transform.GetChild(1).GetComponent<Renderer>().enabled = true;
            UndoCharacterPlace();
        }
    }

    void UndoCharacterPlace()
    {
		//supposed to be a reverse version of place next character
		characterNum--;//changed to --   
		characters[characterNum + 1].GetComponent<PlaceCharacter>().enabled = false;
		characters[characterNum + 1].GetComponent<Renderer>().enabled = false;
		characters[characterNum + 1].transform.GetChild(0).GetComponent<Renderer>().enabled = false;
		characters[characterNum + 1].transform.GetChild(1).GetComponent<Renderer>().enabled = false;
		
		int tx = (int)Mathf.Round(characters[characterNum].transform.position.x);
		int ty = (int)Mathf.Round(characters[characterNum].transform.position.y);
		
		grid.taken[tx, ty] = 0;

		canPlace = true;

		characters[characterNum].GetComponent<Renderer>().enabled = true;
		characters[characterNum].GetComponent<PlaceCharacter>().enabled = true;
    }

    void PlaceNextCharacter()
    {
        if (characterNum < (maxCharacters - 1)) {
            characterNum++;            
            characters[characterNum - 1].GetComponent<PlaceCharacter>().enabled = false;            
            characters[characterNum].GetComponent<Renderer>().enabled = true;
            characters[characterNum].GetComponent<PlaceCharacter>().enabled = true;
        }
        else if (characterNum == (maxCharacters - 1))
        {
            characters[maxCharacters - 1].GetComponent<PlaceCharacter>().enabled = false;            
            coroutineRevert = WaitForReversionCall(KeyCode.U);
            StartCoroutine(coroutineRevert);
            coroutine = WaitForKeyDown(KeyCode.Return);
            StartCoroutine(coroutine);            
        }
    }

    public IEnumerator WaitForReversionCall(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
        character = GameObject.Find("Character" + (maxCharacters));
        character.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
        character.transform.GetChild(1).GetComponent<Renderer>().enabled = true;
        
        UnDoLastCharacterPlace();
    }

    public IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
        
        firstEnemy.GetComponent<Renderer>().enabled = true;
        firstEnemy.GetComponent<PlaceEnemy>().enabled = true;
        characterNum = 0;
    }

    void UnDoLastCharacterPlace()
    {
        int tx = (int)Mathf.Round(characters[characterNum].transform.position.x);
        int ty = (int)Mathf.Round(characters[characterNum].transform.position.y);
        
        grid.taken[tx, ty] = 0;
        canPlace = true;
        characters[characterNum].GetComponent<Renderer>().enabled = true;
        characters[characterNum].GetComponent<PlaceCharacter>().enabled = true;
    }
}
