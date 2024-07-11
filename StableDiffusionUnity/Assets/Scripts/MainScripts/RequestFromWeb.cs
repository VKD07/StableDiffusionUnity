using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class RequestFromWeb
{
    public static UnityWebRequest CreateARequest(string url, RequestType type = RequestType.GET, byte[]jsonByte = null)
    {
        var request = new UnityWebRequest(url, type.ToString());

        //if its a post request with data
        if(jsonByte != null)
        {
            //Uploading the data to the web
            request.uploadHandler = new UploadHandlerRaw(jsonByte);
        }
        // get data from web
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }

    public static void AttachedHeader(UnityWebRequest request, string key, string value)
    {
        request.SetRequestHeader(key, value);
    }
}

public enum RequestType
{
    None = 0,
    POST,
    GET,
}
