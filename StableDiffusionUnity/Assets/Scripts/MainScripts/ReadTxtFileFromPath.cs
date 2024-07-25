using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class ReadTxtFileFromPath
{
    static string txtContent;
    public static string GetStringFromTxtFile(string filePath)
    {
        string[] fileLines = File.ReadAllLines(filePath);

       foreach (string line in fileLines)
        {
            txtContent += $"{line}\n";
        }

        return txtContent;
    }

    public static string[] GetLineOfTxtFromFile(string filePath)
    {
        string[] fileLines = File.ReadAllLines(filePath);
        return fileLines;
    }
}
