using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.Experimental;

public class ImageGenerationProgress : MonoBehaviour
{
    public static ImageGenerationProgress instance { get; private set; }

    bool getProgressValue = true;
    float progressValue;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DrawingUIManager.instance.SetActiveSliderProgressUI(false);
    }

    public IEnumerator RequestImageGenerationProgress()
    {
        DrawingUIManager.instance.SetActiveSliderProgressUI(true);
        getProgressValue = true;
        while (getProgressValue)
        {
            yield return RequestGenerationProgressFromWeb();
            DrawingUIManager.instance.SetProgressSliderValue(progressValue);
        }
    }

    IEnumerator RequestGenerationProgressFromWeb()
    {
        var getRequest = RequestFromWeb.CreateARequest("http://127.0.0.1:7860/sdapi/v1/progress?skip_current_image=false");
        yield return getRequest.SendWebRequest();
        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + getRequest.error);
            yield break;
        }

        var deserializeGetData = JsonUtility.FromJson<GenerationProgress>(getRequest.downloadHandler.text);
        progressValue = deserializeGetData.progress;

        if (progressValue >= 0.90f)
        {
            progressValue = 0;
            DrawingUIManager.instance.SetActiveSliderProgressUI(false);
            StopAllCoroutines();
            getProgressValue = false;
        }
    }
}

public class GenerationProgress
{
    public float progress;
    public int eta_relative;
    public object state;
    public string current_image;
    public string textinfo;
}
