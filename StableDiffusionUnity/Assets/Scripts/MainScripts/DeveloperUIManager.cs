using FreeDraw;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperUIManager : MonoBehaviour
{
    public static DeveloperUIManager instance { get; private set; }

    [Header("=== UI REFERENCES ===")]
    [SerializeField] GameObject developerUIPanel;
    [SerializeField] TMP_InputField promptInputField;
    [SerializeField] TMP_InputField width;
    [SerializeField] TMP_InputField height;
    [SerializeField] Slider cfgScaleValue;
    [SerializeField] TextMeshProUGUI cfgScaleTxt;
    [SerializeField] Slider sampleSteps;
    [SerializeField] TextMeshProUGUI sampleStepsTxt;

    [Header("=== DEVELOPER UI SETTINGS ===")]
    [SerializeField] KeyCode enableDeveloperUIKey = KeyCode.F12;
    [SerializeField] Drawable drawableSettings;

    private void Awake()
    {
        instance = this;
        developerUIPanel.SetActive(false);
        UpdateSliderTextValues();
    }

    private void Update()
    {
        SetActiveDeveloperUI();
    }

    void UpdateSliderTextValues()
    {
        UpdateCfgScaleText(cfgScaleValue.value);
        cfgScaleValue.onValueChanged.AddListener(UpdateCfgScaleText);
        UpdateSampleStepText(sampleSteps.value);
        sampleSteps.onValueChanged.AddListener(UpdateSampleStepText);
    }

    void UpdateCfgScaleText(float value)
    {
        int newVal = (int)value;
        cfgScaleTxt.text = newVal.ToString();
    }

    void UpdateSampleStepText(float value)
    {
        int newVal = (int)value;
        sampleStepsTxt.text = newVal.ToString();
    }

    private void SetActiveDeveloperUI()
    {
        if (Input.GetKeyDown(enableDeveloperUIKey))
        {
            if (!developerUIPanel.activeSelf)
            {
                drawableSettings.enabled = false;
                developerUIPanel.SetActive(true);
            }
            else
            {
                drawableSettings.enabled = true;
                developerUIPanel.SetActive(false);
            }
        }
    }

    #region Getter
    public string GetPromptInput()
    {
        return promptInputField.text;
    }

    public int GetWidth()
    {
        int imgWidth = int.Parse(width.text);
        return imgWidth;
    }

    public int GetHeight()
    {
        int imgHeight = int.Parse(height.text);
        return imgHeight;
    }

    public int GetCfgScaleSlider()
    {
        return (int)cfgScaleValue.value;
    }

    public int GetSampleSteps()
    {
        return (int)sampleSteps.value;
    }
    #endregion
}