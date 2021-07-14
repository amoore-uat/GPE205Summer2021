using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Dropdown mapGeneratorDropdown;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        // TODO: Implement options button.
        quitButton.onClick.AddListener(QuitGame);
        mapGeneratorDropdown.onValueChanged.AddListener(ChangeMapSeedType);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeMapSeedType(int seedType)
    {
        Debug.Log(seedType);
    }
    
}
