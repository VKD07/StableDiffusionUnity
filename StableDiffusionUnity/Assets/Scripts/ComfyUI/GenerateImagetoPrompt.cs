using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GenerateImagetoPrompt : MonoBehaviour
{
    public static GenerateImagetoPrompt instance { get; private set; }

    private string url = "http://127.0.0.1:8188/prompt";
    public string inputImageFileName = "convertedImage.png";
    string filePath = "C:\\Users\\vince\\Documents\\ComfyUI\\ComfyUI_windows_portable\\ComfyUI\\output\\GeneratedPrompt\\ComfyUI.txt";
    public string txtContent = "";
    private void Awake()
    {
        instance = this;
    }

    public void RequestImageToPromptFromWeb()
    {
        StartCoroutine(RequestImageToText());
    }

    IEnumerator RequestImageToText()
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(GetJsonBody());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Response: " + responseText);
                txtContent = ReadTxtFileFromPath.GetStringFromTxtFile(filePath);
            }
        }
    }

    string GetJsonBody()
    {
        // Updated JSON workflow with variables
        string workflowJson = $@"
    {{
      ""prompt"": {{
        ""1"": {{
          ""inputs"": {{
            ""image"": ""{inputImageFileName}"",
            ""upload"": ""image""
          }},
          ""class_type"": ""LoadImage"",
          ""_meta"": {{
            ""title"": ""Load Image""
          }}
        }},
        ""2"": {{
          ""inputs"": {{
            ""model"": ""wd-v1-4-convnextv2-tagger-v2"",
            ""threshold"": 0.35,
            ""character_threshold"": 0.85,
            ""replace_underscore"": false,
            ""trailing_comma"": false,
            ""exclude_tags"": """",
            ""image"": [
              ""1"",
              0
            ]
          }},
          ""class_type"": ""WD14Tagger|pysssss"",
          ""_meta"": {{
            ""title"": ""WD14 Tagger""
          }}
        }},
        ""3"": {{
          ""inputs"": {{
            ""text"": [
              ""2"",
              0
            ],
            ""path"": ""./ComfyUI/output/GeneratedPrompt"",
            ""filename_prefix"": ""ComfyUI"",
            ""filename_delimiter"": ""_"",
            ""filename_number_padding"": 0,
            ""file_extension"": "".txt"",
            ""encoding"": ""utf-8""
          }},
          ""class_type"": ""Save Text File"",
          ""_meta"": {{
            ""title"": ""Save Text File""
          }}
        }}
      }}
    }}";

        return workflowJson;
    }
}
