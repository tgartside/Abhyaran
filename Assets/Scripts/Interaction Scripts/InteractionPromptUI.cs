using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    public bool isDisplayed = false;

    private void Start()
    {
        cam = Camera.main;
        promptPanel.SetActive(false);
    }

    public void SetUp(string pText)
    {
        promptText.text = pText;
        promptPanel.SetActive(true);
        isDisplayed = true;
    }

    public void Close()
    {
        promptPanel.SetActive(false);
        isDisplayed = false;
    }

}
