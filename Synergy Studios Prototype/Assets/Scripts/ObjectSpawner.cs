using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] spawnables;
    public Text remainingObjects;
    private float capacity;
    private GameObject currentObject;

    private void Start()
    {
        capacity = 10;
        SpawnNewObject();
    }

    private void Update()
    {
        if (currentObject != null)
        {
            currentObject.transform.position = transform.position;
            
            if (Input.GetKeyDown(KeyCode.Space) )
            {
                currentObject.AddComponent<Rigidbody2D>();
                currentObject = null;

                capacity--;
                if (capacity > 0)
                    Invoke("SpawnNewObject", 1f);

                Invoke("CheckHeight", 2f);
            }
            
            //reset
        }

        DisplayObjectsNumber();
    }

    void SpawnNewObject() // could be coroutine
    {
        currentObject = Instantiate(spawnables[Random.Range(0,4)], transform.position, Quaternion.identity);
    }

    void DisplayObjectsNumber()
    {
        remainingObjects.text = capacity + "";
    }

    void CheckHeight()
    {
        
    }
}
