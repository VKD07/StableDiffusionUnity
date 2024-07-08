using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Debug = UnityEngine.Debug;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.UI;

public class ImageGenerationManager : MonoBehaviour
{
    [Header("=== UI ===")]
    [SerializeField] TMP_InputField promptText;

    [Header("=== IMG GENERATION SETTINGS ===")]
    [SerializeField, Range(0, 30)] int cfgScaleValue = 30;
    [SerializeField, Range(1, 150)] int sampleSteps = 10;

    [Header("=== CONTROL NET SETTINGS ===")]
    [SerializeField] ControlNetModels controlNetModels;
    [SerializeField] bool lowVram;
    [SerializeField] bool pixelPerfect;

    [Header("=== GENERATED IMAGE SETTINGS ===")]
    [SerializeField] string generatedImgFolder;
    [SerializeField] string imageName;
    string generatedImgPath;

    private void Awake()
    {
        generatedImgPath = Path.Combine(Application.streamingAssetsPath, generatedImgFolder);
    }

    public void GenerateImage()
    {
        //if (promptText.text == "" || promptText.text == "Please enter a prompt")
        //{
        //    promptText.text = "Please enter a prompt";
        //}

        //else
        //{

        StartCoroutine(MakeRequest());
        StartCoroutine(ImageGenerationProgress.instance.RequestImageGenerationProgress());

        //}
    }

    IEnumerator MakeRequest()
    {
        //converting json file to Bytes
        var jsonBytes = Encoding.UTF8.GetBytes(DiffusionJsonbody());

        //Requesting from Website for a post request
        var www = new UnityWebRequest("http://127.0.0.1:7860/sdapi/v1/txt2img", "POST");

        //Uploading the bytes to the website
        www.uploadHandler = new UploadHandlerRaw(jsonBytes);
        //Receiving the data received from the requested website
        www.downloadHandler = new DownloadHandlerBuffer();
        //SEtting Header request
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", " text/plain");
        //Send web request
        yield return www.SendWebRequest();

        //Checking for errors
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);   
        }
        else
        {
            //storing string from website to image data
            string imageData = www.downloadHandler.text;
            //Converting Json data, to image data
            ImageData myImageData = JsonConvert.DeserializeObject<ImageData>(imageData);

            Debug.Log(myImageData.images[0]);

            //Getting image number
            string newImageFileNumber = GetNextImageNumberForFileName(true);
            Debug.Log(newImageFileNumber);

            string newImageFileName = "image_" + newImageFileNumber + ".png";

            //Adding the generated image to file path
            File.WriteAllBytes(Path.Combine(generatedImgPath, newImageFileName), Convert.FromBase64String(myImageData.images[0]));
            //UIManager.instance.SetGeneratedImageUI()
            www.Dispose();

            Process.Start(generatedImgPath);

            UIManager.instance.SetGeneratedImageUI(generatedImgPath);
        }

    }

    string DiffusionJsonbody()
    {
        string json = $@"
        {{
            'prompt': '{promptText.text}',
            'init_images': ['{ConvertImageToBase64(imageName)}'],
            'cfg_scale': {cfgScaleValue},
            'sampler_name': 'DPM++ 2M',
            'steps': {sampleSteps},
            'alwayson_scripts': {{
                'controlnet': {{
                    'args': [
                        {{
                            'enabled': true,
                            'module': 'scribble_pidinet',
                            'model': '{controlNetModels.ToString().ToLower()}',
                            'image': '{ConvertImageToBase64(imageName)}',
                            'weight': 1.0,
                            'resize_mode': 'Crop and Resize',
                            'lowvram': {lowVram.ToString().ToLower()},
                            'processor_res': 64,
                            'threshold_a': 64,
                            'threshold_b': 64,
                            'guidance_start': 0.0,
                            'guidance_end': 1.0,
                            'control_mode': 'ControlNet is more important',
                            'pixel_perfect': {pixelPerfect.ToString().ToLower()}
                        }}
                    ]
                }}
            }}
        }}";

        return json = json.Replace("'", "\""); // Ensure proper JSON formatting
    }

    public static string ConvertImageToBase64(string imgName)
    {
        try
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "Sketches", imgName);
            byte[] imageBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error converting image to Base64: " + ex.Message);
            return null;
        }
    }

    string GetNextImageNumberForFileName(bool isNewImageFile)
    {
        List<int> imageNumbers = new List<int>();


        int maxImageNumber = 0;


        string[] files = Directory.GetFiles(generatedImgPath);

        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            string justFileName = fi.Name;
            string extn = fi.Extension;

            if (extn == ".png")
            {
                string fileNumberString = justFileName.Substring(6, 2);
                int fileNumber = int.Parse(fileNumberString);
                imageNumbers.Add(fileNumber);
            }
        }


        string strMaxImageNumber;

        if (imageNumbers.Count > 0)
        {
            maxImageNumber = imageNumbers.Max();
            if (isNewImageFile)//if this method is being called to return a number for a new image file then we have to take the current max and add 1
            {
                maxImageNumber++;
            }

            strMaxImageNumber = (maxImageNumber).ToString();
            if (maxImageNumber < 10)
            {
                strMaxImageNumber = "0" + strMaxImageNumber;
            }

            Debug.Log("strMaxImageNumber " + strMaxImageNumber);
        }
        else
        {
            strMaxImageNumber = "01";
        }


        return strMaxImageNumber;
    }
}

public class ImageData
{
    public List<string> images { get; set; }
    // public Parameters parameters { get; set; }
    public string info { get; set; }
}
