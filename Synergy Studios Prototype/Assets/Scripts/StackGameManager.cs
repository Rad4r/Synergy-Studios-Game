using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StackGameManager : MonoBehaviour
{
    private static GameManager GM;
    private RemainingObjectsUI remainUIScript;
    
    public GameObject[] spawnables;
    public Transform kraneHandle;
    public GameObject pauseMenu;
    public GameObject starGameObject;
    public GameObject crossGameObject;
    public GameObject particleObject;
    
    public Text winText;
    public Text primaryInstruction;
    public Text secondaryInstruction;
    public Text remainingObjects;
    public AudioClip dropAudio;

    private AudioSource dropSource;
    private static int maxObjects = 30;
    private Transform spawner;
    private float capacity;
    private GameObject currentObject;
    private bool gameWon;
    
    private List<GameObject> temporaryObjects;

    private void Start()
    {
        remainUIScript = FindObjectOfType<RemainingObjectsUI>();
        dropSource = gameObject.AddComponent<AudioSource>();
        GM = FindObjectOfType<GameManager>();
        
        temporaryObjects = new List<GameObject>();
        spawner = kraneHandle.GetChild(0);
        GameSet();
    }

    private void Update()
    {
        if (currentObject != null)
        {
            currentObject.transform.position = spawner.transform.position;
            
            if (Input.GetKeyDown(GM.primary) && gameWon == false)
            {
                currentObject.AddComponent<Rigidbody2D>();
                currentObject = null;
                capacity--;
                remainUIScript.UpdateUI();
                dropSource.PlayOneShot(dropAudio);
                
                if (capacity > 0)
                    Invoke("SpawnNewObject", 1f);
                else
                    Invoke("GameLost", 5f);
            }
            
        }
        
        if(Input.GetKeyDown(GM.secondary))
            pauseMenu.SetActive(true);

        DisplayObjectsNumber();
    }

    void SpawnNewObject() // could be coroutine
    {
        ChangeLayer();
        currentObject = Instantiate(spawnables[Random.Range(0,spawnables.Length)], spawner.transform.position, Quaternion.identity);
        temporaryObjects.Add(currentObject);
    }

    void DisplayObjectsNumber()
    {
        remainingObjects.text = capacity + "";
    }

    void ChangeLayer()
    {
        if(currentObject != null)
            currentObject.layer = 0;
    }

    void GameSet()
    {
        remainUIScript.UIRemainsSetup();
        primaryInstruction.text = "Press " + GM.primary + " to drop object";
        secondaryInstruction.text = "Reset with " + GM.secondary;
        CancelInvoke();
        DestroyExtras();
        temporaryObjects.Clear();
        gameWon = false;
        winText.gameObject.SetActive(false);
        capacity = maxObjects; // change
        SpawnNewObject();
        kraneHandle.transform.position = new Vector3(-2.6f, 4f, 0f);
        kraneHandle.GetComponent<KraneHandleMover>().switchSide = false;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Time.timeScale = 1f;
    }

    public void GameWon()
    {
        if (gameWon == false)
        {
            Instantiate(starGameObject, Vector3.zero, Quaternion.identity);
            Instantiate(particleObject, Vector3.zero, Quaternion.identity);
            Invoke("TimeSlow", 1f);
            Invoke("Pause", 2f);
        }
        gameWon = true;
    }

    void TimeSlow()
    {
        Time.timeScale = 0.1f;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 1;
    }

    void GameLost()
    {
        if (gameWon == false)
        {
            Instantiate(crossGameObject, Vector3.zero, Quaternion.identity);
            Invoke("TimeSlow", 1f);
            Invoke("Pause", 2f);
        }
    }

    void DestroyExtras()
    {
        foreach (var obj in temporaryObjects)
            Destroy(obj);
    }
}
