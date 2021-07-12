using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private Text m_uiText;
    // Start is called before the first frame update
    void Start()
    {
        m_uiText = GetComponentInChildren<Text>();
        m_uiText.text = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
