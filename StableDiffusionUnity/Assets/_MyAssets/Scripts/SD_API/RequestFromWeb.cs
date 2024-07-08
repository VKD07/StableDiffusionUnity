using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class RequestFromWeb
{
    public static UnityWebRequest CreateARequest(string url, RequestType type = RequestType.GET, object data = null)
    {
        var request = new UnityWebRequest(url, type.ToString());

        //if its a post request with data
        if(data != null)
        {
            //converting object to Json file
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            //Uploading the data to the web
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }
        // get data from web
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }

    static void AttachedHeader(UnityWebRequest request, string key, string value)
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
