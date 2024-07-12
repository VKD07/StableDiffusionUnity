using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComfyUIDeveloperUI : MonoBehaviour
{
    [Header("=== ENABLE KEY ===")]
    [SerializeField] KeyCode enableKey = KeyCode.F12;
    [Header("=== UI REFERENCES ===")]
    [SerializeField] GameObject developerUIPanel;
    [SerializeField] ButtonPrompts[] btnPrompts;

    private void Awake()
    {
        developerUIPanel.SetActive(false);
        ApplyInputFieldTextToBtnPrompt();
    }

    private void Start()
    {
    }

    private void Update()
    {
        SetActiveDeveloperUI();
    }

    private void SetActiveDeveloperUI()
    {
        if (Input.GetKeyDown(enableKey))
        {
            developerUIPanel.SetActive(!developerUIPanel.activeSelf);
        }
    }

    private void ApplyInputFieldTextToBtnPrompt()
    {
        for (int i = 0; i < btnPrompts.Length; i++)
        {
            int index = i;

            btnPrompts[index].btn.onClick.AddListener(() =>
            {
                ComfyUiImgGeneration.Instance.AddTextToPrompt(btnPrompts[index].inputField.text);
            });
        }
    }
}

[Serializable]
public class ButtonPrompts
{
    public string btnName;
    public TMP_InputField inputField;
    public Button btn;
}
