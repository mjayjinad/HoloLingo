using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


[Serializable]
public enum Languages
{
    en = 0,
    es = 1,
    hu = 2,
    fr = 3,
    bg = 4
}

[Serializable]
public enum Voices
{
    Aoede,
    Gacrux,
    Puck,
    Kore,
}

public class TranslationManager : MonoBehaviour
{
    private string _translationApiUrl = "https://translation.googleapis.com/language/translate/v2?key=";

    /// <summary>
    /// Translates the given text to a given desired language.
    /// Response is sent through Unity Event in a string.
    /// </summary>
    public void TranslateText(UnityEvent<string> translateEvent, string text, Languages originLanguage, Languages desiredLanguage)
    {
        StartCoroutine(TranslateTextCor(translateEvent, text, originLanguage, desiredLanguage));
    }

    /// <summary>
    /// Handles sending REST API to Googles Translation API based on the text provided and original language and desired language enum.
    /// On finishing the translation, it invokes the translate event.
    /// </summary>
    private IEnumerator TranslateTextCor(UnityEvent<string> translateEvent, string textToTranslate, Languages originLanguage, Languages desiredLanguage)
    {
        string url = _translationApiUrl + GetApiKey();
        string jsonRequestBody = "{\"q\":\"" + textToTranslate + "\",\"source\":\"" + originLanguage + "\",\"target\":\"" + desiredLanguage + "\"}";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);

        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            translateEvent.Invoke(ParseTranslatedText(request.downloadHandler.text));
        }
        else
        {
            Debug.LogError("Translation request failed: " + request.error);
        }
    }

    /// <summary>
    /// Handles Google Translation API response.
    /// Returns the translation in string format.
    /// </summary>
    private string ParseTranslatedText(string jsonResponse)
    {
        JObject json = JObject.Parse(jsonResponse);
        string translatedText = json["data"]["translations"][0]["translatedText"].Value<string>();
        return translatedText;
    }

    private string GetApiKey()
    {
        return Resources.Load<TextAsset>("Security/APIKey").ToString();
    }
}