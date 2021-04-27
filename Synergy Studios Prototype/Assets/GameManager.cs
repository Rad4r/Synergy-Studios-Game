using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public KeyCode primary { get; set; }
    public KeyCode secondary { get; set; }
    
    [HideInInspector]public bool ControlSetup;
    private ControlChange controlScript;
    private float downTimeLeft;

    private void Awake()
    {
        controlScript = FindObjectOfType<ControlChange>();
        ControlSetup = false; // something could go wrong here since game manager is not persistant
        
        primary = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("primaryKey", "LeftArrow"));
        secondary = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("secondaryKey", "RightArrow")); //Could be problem so set control change here
    }

    private void Update()
    {
        if(Input.GetKeyDown (secondary))
            downTimeLeft = Time.time;
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        SceneUpdate();
        
    }

    private void SceneUpdate()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Menu"))
        {
            if (Input.GetKeyUp (secondary)){
                if (Time.time - downTimeLeft > 2)
                    controlScript.SetUI();
                else if (Time.time - downTimeLeft > .3f && ControlSetup == false)
                    Application.Quit();
            }
        }
    }
}
