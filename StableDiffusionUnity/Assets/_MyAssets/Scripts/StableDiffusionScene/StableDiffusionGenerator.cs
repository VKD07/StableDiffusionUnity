using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using System;


public class StableDiffusionGenerator : MonoBehaviour
{
    [SerializeField] string stableDiffusionUrl = "http://127.0.0.1:7860";
    [SerializeField] string myPersistentDataPath;
    [SerializeField] string imgPath;

    [Header("=== GENERATION SETTINGS ===")]
    [TextArea, SerializeField] string prompt = "empty";
    [SerializeField] int iterations = 1;
    [SerializeField] int steps = 20;
    [SerializeField] float cfg_scale = 7.5f;
    [SerializeField] string sampler_name = "k_lms";
    [SerializeField] int width = 512;
    [SerializeField] int height = 512;
    [SerializeField] int seed = -1;
    [SerializeField] float variation_amount = 0;
    [SerializeField] string with_variations = "";
    [SerializeField] UnityEngine.Object initimg = null;
    [SerializeField] float strength = .75f;
    [SerializeField] bool fit = true;
    [SerializeField] float gfpgan_strength = .8f;
    [SerializeField] string upscale_level = "";
    [SerializeField] float upscale_strength = .75f;
    [SerializeField] string initimg_name = "";

    private void Awake()
    {
        myPersistentDataPath = Application.streamingAssetsPath;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Generate()
    {
        Debug.Log("Generating...");

        string fitstr = fit ? "on" : "off";
        var initImgObj = MyJsonConverter.Serialize(initimg);
        string postData = $"{{'prompt':'{prompt}','iterations';'{iterations}','steps':'{steps}','cfg_scale':'{cfg_scale}'," +
            $"'sampler_name':'{sampler_name}','width':'{width}','height':'{height}','seed':'{seed}','variation_amount':'{variation_amount}" +
            $"','with_varations':'{with_variations}','initimg':{initImgObj},'strength':'{strength}','fit':'{fit}','gfpgan_strength':'{gfpgan_strength}" +
            $"','upscale_level':'{upscale_level}','upscale_strength':'{upscale_strength}','initimg_name':'{initimg_name}'}}";

        postData = postData.Replace("'", "\"");

        var request = (HttpWebRequest)WebRequest.Create(stableDiffusionUrl);
        var data = Encoding.ASCII.GetBytes(postData);
        request.KeepAlive = true;
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        //Response
        var response = (HttpWebResponse)request.GetResponse();
        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Debug.Log("responseString: " + responseString);
        var jsonRows = responseString.Split('\n');
        var lastRow = jsonRows[jsonRows.Length - 2];
        Debug.Log("lastRow: " + lastRow);

        // Parse the JSON response
        JObject jsonResponse = JObject.Parse(responseString);
        string url = jsonResponse["url"].ToString();

        Debug.Log(url);
    }

    IEnumerator MakeRequest()
    {
        string paramKey_prompt = "\"init_image\":";
        string paramValue_prompt = "\"" + MyJsonConverter.Serialize(initimg) + "\""; 

        string json = "{" + paramKey_prompt + paramValue_prompt + "}";

        //Converting prompt to bytes
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        //UnityWebRequest from link
        var www = new UnityWebRequest("http://127.0.0.1:7860/sdapi/v1/img2img", "POST");

        //Uploading bytes to Web
        www.uploadHandler = new UploadHandlerRaw(jsonBytes);

        //Downloading contents from web, handling bytes received from web
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
            //Getting the data received from web and converting to string
            string imageData = www.downloadHandler.text;
            //Converting Json data, to image data
            ImageData myImageData = JsonConvert.DeserializeObject<ImageData>(imageData);

            Debug.Log(myImageData.images[0]);

            //Getting image number
            string newImageFileNumber = GetNextImageNumberForFileName(true);
            Debug.Log(newImageFileNumber);

            string newImageFileName = "image_" + newImageFileNumber + ".png";

            //Converting received bytes and storing it into the file path
            File.WriteAllBytes(Path.Combine(myPersistentDataPath, newImageFileName), Convert.FromBase64String(myImageData.images[0]));

            www.Dispose();

            System.Diagnostics.Process.Start(myPersistentDataPath);
        }

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