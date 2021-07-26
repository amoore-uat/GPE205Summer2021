using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    // Buttons
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button quitButton;

    // Dropdowns
    [SerializeField] private Dropdown mapGeneratorDropdown;
    [SerializeField] private Dropdown playerNumberDropdown;

    // Screens
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject optionsMenuScreen;

    // Sliders
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;

    private void Start()
    {
        masterVolumeSlider.value = GameManager.Instance.masterVolume;
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        startButton.onClick.AddListener(StartGame);
        backButton.onClick.AddListener(ShowMainMenu);
        optionsButton.onClick.AddListener(ShowOptions);
        quitButton.onClick.AddListener(QuitGame);
        mapGeneratorDropdown.onValueChanged.AddListener(ChangeMapSeedType);
        mapGeneratorDropdown.value = (int)GameManager.Instance.m_seedType;
        playerNumberDropdown.onValueChanged.AddListener(ChangeNumberOfPlayers);
        playerNumberDropdown.value = GameManager.Instance.numberOfPlayers - 1;
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
        GameManager.Instance.m_seedType = (RandomSeedType) seedType;
        Debug.Log(GameManager.Instance.m_seedType);
    }

    public void ChangeNumberOfPlayers(int numberOfPlayers)
    {
        GameManager.Instance.numberOfPlayers = numberOfPlayers + 1;
    }

    public void ShowOptions()
    {
        Debug.Log("Attempting to show options");
        optionsMenuScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void ShowMainMenu()
    {
        optionsMenuScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }

    public void OnMasterVolumeChanged(float newVolume)
    {
        PlayerPrefs.SetFloat("MASTERVOLUME", newVolume);
        GameManager.Instance.ChangeMasterVolume(newVolume);
    }
}
