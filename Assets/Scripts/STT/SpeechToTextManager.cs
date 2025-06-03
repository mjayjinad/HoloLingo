using System;
using System.Collections;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SpeechToTextManager : MonoBehaviour
{
    [SerializeField] private AITranslationConnector m_translationConnector;
    [SerializeField] private int recordingDuration = 3; // default 3 seconds

    private Coroutine currentRecordingCoroutine;

    private const string SpeechToTextUrl = "https://speech.googleapis.com/v1/speech:recognize?key=";

    public void StartRecording(string expectedText, Action<string> onComplete)
    {
        if (currentRecordingCoroutine == null)
            currentRecordingCoroutine = StartCoroutine(RecordAndRecognize(onComplete));
    }

    private IEnumerator RecordAndRecognize(Action<string> onComplete)
    {
        AudioClip clip = Microphone.Start(null, false, recordingDuration, 44100);
        yield return new WaitForSeconds(recordingDuration);
        Microphone.End(null);

        yield return RecognizeSpeech(clip, onComplete);

        currentRecordingCoroutine = null;
    }

    private IEnumerator RecognizeSpeech(AudioClip audioClip, Action<string> onComplete)
    {
        // Convert enum to valid language code string (example mapping)
        string languageCode = LanguageToGoogleCode(m_translationConnector.userTargetLanguage);

        string jsonRequestBody = $"{{\"config\":{{\"encoding\":\"LINEAR16\",\"sampleRateHertz\":44100,\"languageCode\":\"{languageCode}\",\"enableWordTimeOffsets\":false}},\"audio\":{{\"content\":\"{ConvertAudioClipToString(audioClip)}\"}}}}";

        string url = SpeechToTextUrl + GetApiKey();

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string transcript = ParseTranscript(request.downloadHandler.text);
            onComplete?.Invoke(transcript);
        }
        else
        {
            Debug.LogError($"Speech recognition failed: {request.error} | {request.downloadHandler.text}");
            onComplete?.Invoke("");
        }
    }

    private string ParseTranscript(string jsonResponse)
    {
        try
        {
            JObject json = JObject.Parse(jsonResponse);
            return json["results"]?[0]?["alternatives"]?[0]?["transcript"]?.Value<string>() ?? "";
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing transcript: " + ex.Message);
            return "";
        }
    }

    private string ConvertAudioClipToString(AudioClip clip)
    {
        byte[] bytes = AudioClipToBytes(clip);
        return Convert.ToBase64String(bytes);
    }

    private byte[] AudioClipToBytes(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] bytes = new byte[samples.Length * 2];
        int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            short val = (short)(samples[i] * rescaleFactor);
            bytes[i * 2] = (byte)(val & 0xff);
            bytes[i * 2 + 1] = (byte)((val >> 8) & 0xff);
        }

        return bytes;
    }

    private string LanguageToGoogleCode(Languages lang)
    {
        switch (lang)
        {
            case Languages.en: return "en-US";
            case Languages.es: return "es-ES";
            case Languages.hu: return "hu-HU";
            case Languages.fr: return "fr-FR";
            case Languages.bg: return "bg-BG";
            default: return "en-US";
        }
    }

    private string GetApiKey()
    {
        return Resources.Load<TextAsset>("Security/APIKey").ToString();
    }
}
