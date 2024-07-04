using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] Image generatedImageUI;
    [SerializeField] ImageToSpriteConverter spriteConverter;

    #region SETTER
    public void SetGeneratedImageUI(string imgPath)
    {
        generatedImageUI.sprite = spriteConverter.LoadSpriteFromFile(imgPath);
    }
    #endregion


}
