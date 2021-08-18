using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private Text m_uiText;
    public GameObject GameOverScreen;
    private Health tankHealth;
    // Start is called before the first frame update
    void Start()
    {
        m_uiText = GetComponentInChildren<Text>();
        m_uiText.text = gameObject.name;
        tankHealth = GetComponent<Health>();
        //tankHealth.OnTankKilled.AddListener(HandleGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HandleGameOver(GameObject tankKilled)
    {
        Debug.Log("Handling game over");
        for (int i = 0; i < GameManager.Instance.numberOfPlayers; i++)
        {
            Debug.Log("Checking Game object: " + GameManager.Instance.Players[i].name);
            if (tankKilled == GameManager.Instance.Players[i])
            {
                Debug.Log("Player killed is player " + (i + 1));
                Debug.Log("Lives: " + GameManager.Instance.PlayerLives[i]);
                if (GameManager.Instance.PlayerLives[i] <= 1)
                {
                    GameOverScreen.SetActive(true);
                }
            }
        }
    }
}
