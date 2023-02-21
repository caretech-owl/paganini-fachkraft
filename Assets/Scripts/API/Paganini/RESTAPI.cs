using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static ServerCommunication;

public class RESTAPI : PersistentLazySingleton<RESTAPI>
{


    public void Get<T>(string endpoint, UnityAction<T> successCallback, UnityAction<string> errorCallback, Dictionary<string, string> headers = null)
    {
        UnityWebRequest request = UnityWebRequest.Get(endpoint);
        SetHeaders(request, headers);        

        StartCoroutine(PerformRequest<T>(request, successCallback, errorCallback));
    }

    protected void Post<T>(string endpoint, string jsonData, UnityAction<T> successCallback, UnityAction<string> errorCallback, Dictionary<string, string> headers = null)
    {
        UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        SetHeaders(request, headers);

        StartCoroutine(PerformRequest<T>(request, successCallback, errorCallback));

    }

    protected void Put<T>(string endpoint, string jsonData, UnityAction<T> successCallback, UnityAction<string> errorCallback, Dictionary<string, string> headers = null)
    {
        UnityWebRequest request = new UnityWebRequest(endpoint, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        SetHeaders(request, headers);

        StartCoroutine(PerformRequest<T>(request, successCallback, errorCallback));
    }

    protected void Delete<T>(string endpoint, UnityAction<T> successCallback, UnityAction<string> errorCallback, Dictionary<string, string> headers = null)
    {
        UnityWebRequest request = UnityWebRequest.Delete(endpoint);
        SetHeaders(request, headers);

        StartCoroutine(PerformRequest<T>(request, successCallback, errorCallback));
    }

    protected IEnumerator PerformRequest<T>(UnityWebRequest request, UnityAction<T> successCallback, UnityAction<string> errorCallback)
    {
        //certificat workaround
        request.certificateHandler = new ForceAcceptAll();

        yield return request.SendWebRequest();

        if (request.isNetworkError || (request.responseCode != 200 && request.responseCode != 201 && request.responseCode != 204))
        {
            Debug.LogError(request.error);
            errorCallback(request.error);
        }
        else
        {
            T responseData = JsonUtility.FromJson<T>(request.downloadHandler.text);
            successCallback(responseData);
        }
    }


    private class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    private void SetHeaders(UnityWebRequest request, Dictionary<string, string> headers)
    {
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
        }
    }
}
