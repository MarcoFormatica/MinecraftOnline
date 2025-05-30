using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button loadButton;
    public bool shouldLoad;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        loadButton.interactable = PlayerPrefs.HasKey("Salvataggio");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        shouldLoad = true;
        SceneManager.LoadScene(1);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

}
