using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComfyUIDeveloperUI : MonoBehaviour
{
    public static ComfyUIDeveloperUI instance { get; private set; }
    [Header("=== ENABLE KEY ===")]
    [SerializeField] KeyCode enableKey = KeyCode.F12;
    [Header("=== UI REFERENCES ===")]
    [SerializeField] GameObject developerUIPanel;
    [SerializeField] ButtonPrompts[] btnPrompts;
    [SerializeField] TMP_InputField controlNetModel;
    [SerializeField] TMP_InputField checkPointModel;
    [SerializeField] TMP_InputField additionalPrompt;
    [SerializeField] TMP_InputField negativeprompt;

    #region Getter
    public string GetControlNetModel => controlNetModel.text;
    public string GetCheckPointModel => checkPointModel.text;
    public string GetAddtionalPrompt => additionalPrompt.text;
    public string GetNegativePrompt => negativeprompt.text;
    #endregion

    private void Awake()
    {
        instance = this;
        developerUIPanel.SetActive(false);
        ApplyInputFieldTextToBtnPrompt();
    }

    private void Update()
    {
        SetActiveDeveloperUI();
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

    private void SetActiveDeveloperUI()
    {
        if (Input.GetKeyDown(enableKey))
        {
            developerUIPanel.SetActive(!developerUIPanel.activeSelf);
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
