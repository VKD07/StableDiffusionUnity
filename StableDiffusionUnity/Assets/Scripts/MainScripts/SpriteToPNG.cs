using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteToPNG : MonoBehaviour
{
    // Reference to the sprite you want to convert
    public Sprite sprite;

    // Path to save the PNG file
    string savePath;
    private void Awake()
    {
        savePath = Path.Combine(Application.streamingAssetsPath, "Sketches");
    }

    public void SaveSketch()
    {
        SaveSpriteToPNG(sprite, savePath, "convertedImage.png");
    }

    public void SaveSpriteToPNG(Sprite sprite, string savePath, string fileName)
    {
        // Extract the texture from the sprite
        Texture2D texture = sprite.texture;

        // Create a new texture with the sprite's dimensions
        Texture2D newTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

        // Copy the pixels from the sprite texture to the new texture
        Color[] pixels = texture.GetPixels((int)sprite.textureRect.x,
                                           (int)sprite.textureRect.y,
                                           (int)sprite.textureRect.width,
                                           (int)sprite.textureRect.height);
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        // Encode the texture to PNG format
        byte[] pngData = newTexture.EncodeToPNG();

        if (pngData != null)
        {
            // Ensure the save directory exists
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // Construct the full path
            string fullPath = Path.Combine(savePath, fileName);

            // Write the PNG file to disk
            File.WriteAllBytes(fullPath, pngData);

            Debug.Log("PNG file saved at: " + fullPath);
        }
        else
        {
            Debug.LogError("Failed to encode texture to PNG.");
        }
    }
}
