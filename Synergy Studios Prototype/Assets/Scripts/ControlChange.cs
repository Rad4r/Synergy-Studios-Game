using UnityEngine;
using UnityEngine.UI;

public enum KEYS {NONE, SECONDARY, PRIMARY}
public class ControlChange : MonoBehaviour
{
    
    private Transform panel;
    private Event keyEvent;
    private KeyCode newKey;
    
    private bool primaryKeySet;
    private bool secondaryKeySet;

    private static GameManager GM;
    private KEYS currentKey;

    private void Start()
    {
        GM = FindObjectOfType<GameManager>();
        panel = transform.Find("Panel");
        
        if(!PlayerPrefs.HasKey("ControlSet"))
            SetUI();
            //duplicatestuff
    }

    private void OnGUI()
    {
        keyEvent = Event.current;
        
        if(keyEvent.isKey && Input.GetKeyUp(keyEvent.keyCode))
        {
            if (currentKey == KEYS.PRIMARY)
            {
                SetKey(primaryKeySet, "primary", 0, true, KEYS.SECONDARY);
            }
            else if (currentKey == KEYS.SECONDARY)
            {
                SetKey(secondaryKeySet, "secondary", 1, false, KEYS.NONE);
            }
        }
        
    }
    
    private void KeySetTrue()
    {
        if(currentKey == KEYS.PRIMARY)
            primaryKeySet = true;
        else if (currentKey == KEYS.SECONDARY)
            secondaryKeySet = true;
    }

    public void AssignKey(string keyName)
    {
        switch(keyName)
        {
            case "primary":
                GM.primary = newKey;
                panel.GetChild(0).GetChild(0).GetComponent<Text>().text = GM.primary.ToString();
                PlayerPrefs.SetString("primaryKey", GM.primary.ToString());
                break;

            case "secondary":
                GM.secondary = newKey;
                panel.GetChild(1).GetChild(0).GetComponent<Text>().text = GM.secondary.ToString();
                PlayerPrefs.SetString("secondaryKey", GM.secondary.ToString());
                break;
        }
    }

    public void SetUI()
    {
        GM.ControlSetup = true;
        currentKey = KEYS.PRIMARY;
        panel.gameObject.SetActive(true);
        panel.GetChild(0).gameObject.SetActive(true);
        panel.GetChild(0).GetComponent<Text>().text = "Press your primary button (Default primary arrow)";
        panel.GetChild(1).GetComponent<Text>().text = "Press your secondary button (Default secondary arrow)";
        panel.GetChild(0).GetChild(0).GetComponent<Text>().text = GM.primary.ToString();
        panel.GetChild(1).GetChild(0).GetComponent<Text>().text = GM.secondary.ToString();
    }

    private void SetKey(bool keySet, string input, int child, bool secondaryButton, KEYS key)
    {
        if (keySet == false)
        {
            newKey = keyEvent.keyCode;
            AssignKey(input);
            panel.GetChild(child).GetComponent<Text>().text = "Press your " + input + " key again!";
            Invoke("KeySetTrue", .5f);
        }
        else
        {
            VerifyInput(child, secondaryButton, key);
        }
    }

    private void VerifyInput(int child, bool secondaryButton, KEYS key)
    {
        if (keyEvent.keyCode == newKey)
        {
            if(currentKey == KEYS.PRIMARY)
                panel.GetChild(0).gameObject.SetActive(false);
            else
            {
                panel.gameObject.SetActive(false);
                Invoke("DelayedSetup", 2f);
                PlayerPrefs.SetInt ("ControlSet", 1);
                PlayerPrefs.Save();
            }
                
            
            panel.GetChild(1).gameObject.SetActive(secondaryButton);
            currentKey = key;
        }
        else if (keyEvent.keyCode != KeyCode.None)
        {
            panel.GetChild(child).GetComponent<Text>().text = "That is the wrong key try again";
            
            if (currentKey == KEYS.PRIMARY)
                primaryKeySet = false;
            else
                secondaryKeySet = false;
        }
    }

    private void DelayedSetup()
    {
        GM.ControlSetup = false;
    }
    
    //Use just one button to set both buttons
}
