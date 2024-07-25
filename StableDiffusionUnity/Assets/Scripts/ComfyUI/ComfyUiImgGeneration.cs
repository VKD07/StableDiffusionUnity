using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ComfyUiImgGeneration : MonoBehaviour
{
    public static ComfyUiImgGeneration Instance { get; private set; }

    private string url = "http://127.0.0.1:8188/prompt";

    [Header("=== IMAGE GENERATION SETTINGS ===")]
    [SerializeField] string inputImageFileName = "convertedImage.png";

    string positivePrompt;
    string negativePrompt;
    string additionalPrompt;
    string checkPointModel;
    string controlNetModel;
    long seedValue = 175275630075615;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SetGeneratedImageToScreen());
    }

    public void MakeRequest()
    {
        checkPointModel = ComfyUIDeveloperUI.instance.GetCheckPointModel;
        controlNetModel = ComfyUIDeveloperUI.instance.GetControlNetModel;
        negativePrompt = ComfyUIDeveloperUI.instance.GetNegativePrompt;
        additionalPrompt = ComfyUIDeveloperUI.instance.GetAddtionalPrompt;
        StartCoroutine(RequestImage());
    }

    IEnumerator RequestImage()
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(GetJsonBody());
            Debug.Log(GetJsonBody());
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
            }
        }
    }

    /// <summary>
    /// Getting generated images from Comfy UI temp folder
    /// </summary>
    IEnumerator SetGeneratedImageToScreen()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            DrawingUIManager.instance.SetGeneratedImageUI("C:\\Users\\vince\\Documents\\ComfyUI\\ComfyUI_windows_portable\\ComfyUI\\temp");
        }
    }

    string GetJsonBody()
    {

    string workflowJson = $@"
    {{
      ""prompt"": {{
        ""6"": {{
          ""inputs"": {{
            ""text"": ""{positivePrompt},{additionalPrompt}"",
            ""clip"": [
              ""20"",
              1
            ]
          }},
          ""class_type"": ""CLIPTextEncode"",
          ""_meta"": {{
            ""title"": ""CLIP Text Encode (Prompt)""
          }}
        }},
        ""7"": {{
          ""inputs"": {{
            ""text"": ""{negativePrompt}"",
            ""clip"": [
              ""20"",
              1
            ]
          }},
          ""class_type"": ""CLIPTextEncode"",
          ""_meta"": {{
            ""title"": ""CLIP Text Encode (Prompt)""
          }}
        }},
        ""8"": {{
          ""inputs"": {{
            ""samples"": [
              ""13"",
              0
            ],
            ""vae"": [
              ""20"",
              2
            ]
          }},
          ""class_type"": ""VAEDecode"",
          ""_meta"": {{
            ""title"": ""VAE Decode""
          }}
        }},
        ""13"": {{
          ""inputs"": {{
            ""add_noise"": true,
            ""noise_seed"": [
              ""30"",
              3
            ],
            ""cfg"": 1,
            ""model"": [
              ""20"",
              0
            ],
            ""positive"": [
              ""44"",
              0
            ],
            ""negative"": [
              ""44"",
              1
            ],
            ""sampler"": [
              ""14"",
              0
            ],
            ""sigmas"": [
              ""22"",
              0
            ],
            ""latent_image"": [
              ""34"",
              0
            ]
          }},
          ""class_type"": ""SamplerCustom"",
          ""_meta"": {{
            ""title"": ""SamplerCustom""
          }}
        }},
        ""14"": {{
          ""inputs"": {{
            ""sampler_name"": ""euler""
          }},
          ""class_type"": ""KSamplerSelect"",
          ""_meta"": {{
            ""title"": ""KSamplerSelect""
          }}
        }},
        ""20"": {{
          ""inputs"": {{
            ""ckpt_name"": ""{checkPointModel}""
          }},
          ""class_type"": ""CheckpointLoaderSimple"",
          ""_meta"": {{
            ""title"": ""Load Checkpoint""
          }}
        }},
        ""22"": {{
          ""inputs"": {{
            ""steps"": 1,
            ""denoise"": 1,
            ""model"": [
              ""20"",
              0
            ]
          }},
          ""class_type"": ""SDTurboScheduler"",
          ""_meta"": {{
            ""title"": ""SDTurboScheduler""
          }}
        }},
        ""25"": {{
          ""inputs"": {{
            ""images"": [
              ""8"",
              0
            ]
          }},
          ""class_type"": ""PreviewImage"",
          ""_meta"": {{
            ""title"": ""Preview Image""
          }}
        }},
        ""30"": {{
          ""inputs"": {{
            ""seed"": {seedValue}
          }},
          ""class_type"": ""Seed"",
          ""_meta"": {{
            ""title"": ""Seed""
          }}
        }},
        ""34"": {{
          ""inputs"": {{
            ""pixels"": [
              ""46"",
              0
            ],
            ""vae"": [
              ""20"",
              2
            ]
          }},
          ""class_type"": ""VAEEncode"",
          ""_meta"": {{
            ""title"": ""VAE Encode""
          }}
        }},
        ""42"": {{
          ""inputs"": {{
            ""control_net_name"": ""{controlNetModel}""
          }},
          ""class_type"": ""ControlNetLoader"",
          ""_meta"": {{
            ""title"": ""Load ControlNet Model""
          }}
        }},
        ""44"": {{
          ""inputs"": {{
            ""strength"": 1,
            ""start_percent"": 0,
            ""end_percent"": 1,
            ""positive"": [
              ""6"",
              0
            ],
            ""negative"": [
              ""7"",
              0
            ],
            ""control_net"": [
              ""42"",
              0
            ],
            ""image"": [
              ""46"",
              0
            ]
          }},
          ""class_type"": ""ControlNetApplyAdvanced"",
          ""_meta"": {{
            ""title"": ""Apply ControlNet (Advanced)""
          }}
        }},
        ""46"": {{
          ""inputs"": {{
            ""image"": ""{inputImageFileName}"",
            ""upload"": ""image""
          }},
          ""class_type"": ""LoadImage"",
          ""_meta"": {{
            ""title"": ""Load Image""
          }}
        }}
      }}
    }}";

        return workflowJson;
    }

    public void AddTextToPrompt(string newPrompt)
    {
        positivePrompt = newPrompt;
    }

    public void ResetPrompt()
    {
        positivePrompt = "";
    }
}