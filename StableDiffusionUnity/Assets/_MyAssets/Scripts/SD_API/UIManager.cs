using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    [SerializeField] Image generatedImageUI;
    [SerializeField] Slider imgGenerationProgress;
    [SerializeField] ImageToSpriteConverter spriteConverter;

    private void Awake()
    {
        instance = this;
    }

    #region SETTER
    public void SetGeneratedImageUI(string imgPath)
    {
        generatedImageUI.sprite = spriteConverter.LoadSpriteFromFile(imgPath);
    }

    public void SetProgressSliderValue(float value)
    {
        imgGenerationProgress.value = value;
    }

    public void SetActiveSliderProgressUI(bool active)
    {
        imgGenerationProgress.gameObject.SetActive(active);
    }
    #endregion
}
