using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ReadModelsFromTxtFile : MonoBehaviour
{
    [SerializeField] string controlNetModelsFileName;
    public string[] controlNetModels;

    private void Start()
    {
        GetTxtModelsFromTxtFile(controlNetModelsFileName);
    }

    void GetTxtModelsFromTxtFile(string txtFileName)
    {

       controlNetModels = ReadTxtFileFromPath.GetLineOfTxtFromFile(Path.Combine(Application.streamingAssetsPath, "AI Models",txtFileName+ ".txt"));
    }
}
