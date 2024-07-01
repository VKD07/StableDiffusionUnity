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

public class ImageGenerationManager : MonoBehaviour
{

    string myPersistentDataPath;

    public TMP_InputField promptText;

    public string imgPath;
    public void GenerateImage()
    {
        if (promptText.text == "" || promptText.text == "Please enter a prompt")
        {
            promptText.text = "Please enter a prompt";
        }

        else
        {
            StartCoroutine(MakeRequest());
        }
    }

    private void Awake()
    {
        myPersistentDataPath = Application.persistentDataPath + "\\";
        Debug.Log(myPersistentDataPath);
    }


    IEnumerator MakeRequest()
    {

        string promptValue = "Cats";
        string json = "{\r\n \"prompt\": \" "+ promptText.text + "\",\r\n  \"init_images\": [\r\n    \"" + ConvertImageToBase64(imgPath) +"\"\r\n  ]\r\n}";


        Debug.Log(json);

        var jsonBytes = Encoding.UTF8.GetBytes(json);

        var www = new UnityWebRequest("http://127.0.0.1:7860/sdapi/v1/img2img", "POST");

        www.uploadHandler = new UploadHandlerRaw(jsonBytes);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", " text/plain");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string imageData = www.downloadHandler.text;

            //Converting Json data, to image data
            ImageData myImageData = JsonConvert.DeserializeObject<ImageData>(imageData);

            Debug.Log(myImageData.images[0]);

            //Getting image number
            string newImageFileNumber = GetNextImageNumberForFileName(true);
            Debug.Log(newImageFileNumber);

            string newImageFileName = "image_" + newImageFileNumber + ".png";


            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, newImageFileName), Convert.FromBase64String(myImageData.images[0]));

            www.Dispose();

            Process.Start(myPersistentDataPath);
        }

    }

    IEnumerator MakeRequestImg()
    {
        // Read the image file and convert it to Base64
        string imagePath = "C:\\Users\\vince\\Documents\\GitHub\\StableDiffusionUnity\\StableDiffusionUnity\\Assets\\Cat.png";  // Replace with your actual image path
        string base64Image = ConvertImageToBase64(imagePath);

        if (base64Image == null)
        {
            Debug.LogError("Failed to convert image to Base64.");
            yield break;
        }

        #region promptPayloadExample
        /*
            {
                "enable_hr": false,
                "denoising_strength": 0,
                "firstphase_width": 0,
                "firstphase_height": 0,
                "hr_scale": 2,
                "hr_upscaler": "string",
                "hr_second_pass_steps": 0,
                "hr_resize_x": 0,
                "hr_resize_y": 0,
                "prompt": "",
                "styles": [
                  "string"
                ],
                "seed": -1,
                "subseed": -1,
                "subseed_strength": 0,
                "seed_resize_from_h": -1,
                "seed_resize_from_w": -1,
                "sampler_name": "string",
                "batch_size": 1,
                "n_iter": 1,
                "steps": 50,
                "cfg_scale": 7,
                "width": 512,
                "height": 512,
                "restore_faces": false,
                "tiling": false,
                "negative_prompt": "string",
                "eta": 0,
                "s_churn": 0,
                "s_tmax": 0,
                "s_tmin": 0,
                "s_noise": 1,
                "override_settings": {},
                "override_settings_restore_afterwards": true,
                "sampler_index": "Euler"
          }
        */
        #endregion

        // Construct JSON payload with prompt and image
        string paramKey_prompt = "\"prompt\":";
        string paramValue_prompt = "\"" + promptText.text + "\"";

        string json = base64Image;

        Debug.Log("Request JSON: " + json);

        // Convert JSON to bytes
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        // Create UnityWebRequest
        var www = new UnityWebRequest("http://127.0.0.1:7860/sdapi/v1/img2img", "POST");

        www.uploadHandler = new UploadHandlerRaw(jsonBytes);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "text/plain");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Request Error: " + www.error);
        }
        else
        {
            string imageData = www.downloadHandler.text;
            Debug.Log("Response: " + imageData);

            // Process the response as needed
            // Example: Deserialize JSON response
            try
            {
                ImageData myImageData = JsonConvert.DeserializeObject<ImageData>(imageData);
                Debug.Log("Image URL: " + myImageData.images[0]);

                // Save image locally
                string newImageFileNumber = GetNextImageNumberForFileName(true);
                string newImageFileName = "image_" + newImageFileNumber + ".png";
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, newImageFileName), Convert.FromBase64String(myImageData.images[0]));

                // Open the folder containing the saved image
                Process.Start(myPersistentDataPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Deserialization Error: " + ex.Message);
            }
        }

        www.Dispose();
    }


    string GetNextImageNumberForFileName(bool isNewImageFile)
    {
        List<int> imageNumbers = new List<int>();


        int maxImageNumber = 0;


        string[] files = Directory.GetFiles(myPersistentDataPath);

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

    public static string ConvertImageToBase64(string imagePath)
    {
        try
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error converting image to Base64: " + ex.Message);
            return null;
        }
    }

}


public class Parameters
{
    public bool enable_hr { get; set; }
    public int denoising_strength { get; set; }
    public int firstphase_width { get; set; }
    public int firstphase_height { get; set; }
    public string prompt { get; set; }
    public object styles { get; set; }
    public int seed { get; set; }
    public int subseed { get; set; }
    public int subseed_strength { get; set; }
    public int seed_resize_from_h { get; set; }
    public int seed_resize_from_w { get; set; }
    public object sampler_name { get; set; }
    public int batch_size { get; set; }
    public int n_iter { get; set; }
    public int steps { get; set; }
    public double cfg_scale { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public bool restore_faces { get; set; }
    public bool tiling { get; set; }
    public object negative_prompt { get; set; }
    public object eta { get; set; }
    public double s_churn { get; set; }
    public object s_tmax { get; set; }
    public double s_tmin { get; set; }
    public double s_noise { get; set; }
    public object override_settings { get; set; }
    public string sampler_index { get; set; }
}


public class ImageData
{
    public List<string> images { get; set; }
    // public Parameters parameters { get; set; }
    public string info { get; set; }
}
