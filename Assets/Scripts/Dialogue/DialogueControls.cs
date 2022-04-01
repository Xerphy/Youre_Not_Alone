﻿using Yarn.Unity;
using UnityEngine;

public class DialogueControls : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text dialogue = null;

    private DialogueUI2 dialogueUI = null;
    private TMPro.TMP_Text[] options;

    private int optionSize;
    private int currentOption;

    private bool isOptionDisplayed;

    // Start is called before the first frame update
    void Start()
    {
        // Flag needed to check when the Options are displayed to enable the controls for them
        isOptionDisplayed = false;
        // Get a reference to the DialogueUI
        dialogueUI = FindObjectOfType<DialogueUI2>();
        // Save the number of options available
        optionSize = dialogueUI.optionButtons.Count;
        // Initialize the current index
        currentOption = 0;
        // Initialize the array size to the number of options
        options = new TMPro.TMP_Text[optionSize];
        // Get the TextMeshPro Text components from the option buttons in the DialogueUI 
        for (int i = 0; i < optionSize; i++)
        {
            options[i] = dialogueUI.optionButtons[i].GetComponentInChildren<TMPro.TMP_Text>();
        }
    }

    public void ChangeOption(string input)
    {
        if (isOptionDisplayed)
        {
            if (input.Equals("Left"))
            {
                currentOption = (currentOption + 1) % optionSize;
                dialogue.SetText(options[currentOption].text);
            }
            else if (input.Equals("Right"))
            {
                //Move to the previous option
                if (currentOption == 0)
                    currentOption = optionSize - 1;
                else
                    currentOption = (Mathf.Abs(currentOption - 1) % optionSize);

                dialogue.SetText(options[currentOption].text);
            }
        }
    }

    public void SkipDialogue()
    {
        if (!isOptionDisplayed)
        {
            dialogueUI.MarkLineComplete();
        }
    }

    public void SelectOption()
    {
        if (isOptionDisplayed)
        {
            dialogueUI.SelectOption(currentOption);
            ResetCurrentOption();
        }
    }

    private void ResetCurrentOption()
    {
        currentOption = 0;
    }

    public void SetStartingOption()
    {
        dialogue.SetText(options[0].text);
    }

    public void SetOptionDisplayed(bool flag)
    {
        isOptionDisplayed = flag;
    }
}