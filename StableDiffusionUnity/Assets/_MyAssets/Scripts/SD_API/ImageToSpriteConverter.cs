using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ImageToSpriteConverter : MonoBehaviour
{
    public Sprite LoadSpriteFromFile(string generatedImgPath)
    {
        try
        {
            // Construct the full path to the image file within the Sketches folder
            string filePath = Path.Combine(generatedImgPath, GetTheLastGeneratedImageName(generatedImgPath));
            // Read the file bytes
            byte[] fileData = File.ReadAllBytes(filePath);

            // Create a new texture and load the image data
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                // Create and return the sprite from the texture
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load image data into texture.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading sprite from file: " + ex.Message);
            return null;
        }
    }

    string GetTheLastGeneratedImageName(string generatedImagePath)
    {
        string[] files = Directory.GetFiles(generatedImagePath);
        files = files.Where(file => !file.EndsWith(".meta")).ToArray();
        return Path.GetFileName(files[files.Length - 1]);
    }
}
