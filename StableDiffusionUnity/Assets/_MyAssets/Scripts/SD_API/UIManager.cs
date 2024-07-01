using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    [SerializeField] Image generatedImage;

    public void SetGeneratedImageUI(Sprite generatedImg)
    {
        generatedImage.sprite = generatedImg;
    }
}
