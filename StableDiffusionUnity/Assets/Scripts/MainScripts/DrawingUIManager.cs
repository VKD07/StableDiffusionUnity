using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingUIManager : MonoBehaviour
{
    public static DrawingUIManager instance { get; private set; }
    [SerializeField] Image generatedImageUI;
    [SerializeField] Slider imgGenerationProgress;

    private void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
       
    }

    #region SETTER
    public void SetGeneratedImageUI(string imgPath)
    {
        generatedImageUI.sprite = ImageToSpriteConverter.LoadSpriteFromFile(imgPath);
    }

    public void SetProgressSliderValue(float value)
    {
        imgGenerationProgress.value = value;
    }

    public void buttonPrompt(string text)
    {
        ComfyUiImgGeneration.Instance.AddTextToPrompt(text);
    }

    public void SetActiveSliderProgressUI(bool active)
    {
        imgGenerationProgress.gameObject.SetActive(active);
    }
    #endregion
}
