using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionTrigger : MonoBehaviour
{
    public GameObject instructionUI;
    public TextMeshProUGUI promptText;
    public string prompt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            promptText.text = prompt;
            instructionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            instructionUI.SetActive(false);
        }
    }

}
