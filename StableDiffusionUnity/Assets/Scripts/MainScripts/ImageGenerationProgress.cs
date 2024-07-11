using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.Experimental;

public class ImageGenerationProgress : MonoBehaviour
{
    public static ImageGenerationProgress instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DrawingUIManager.instance.SetActiveSliderProgressUI(false);
    }

    bool getProgressValue = true;
    float progressValue;

    public IEnumerator RequestImageGenerationProgress()
    {
        while (getProgressValue)
        {
            if (progressValue >= 0.99f)
            {
                DrawingUIManager.instance.SetActiveSliderProgressUI(false);
                getProgressValue = false;
                break;
            }

            DrawingUIManager.instance.SetActiveSliderProgressUI(true);
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
     
    }

    public class GenerationProgress
    {
        public float progress;
        public int eta_relative;
        public object state;
        public string current_image;
        public string textinfo;
    }
}
