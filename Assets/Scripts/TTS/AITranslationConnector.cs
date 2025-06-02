using System;
using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AITranslationConnector : MonoBehaviour
{
    [SerializeField] private PassthroughCameraDescription aiDetection;
    [SerializeField] private TranslationManager translationManager;
    [SerializeField] private SpeechToTextManager m_speechToTextManager;
    [SerializeField] private TMP_Text translatedResultTxt;

    [SerializeField] private AudioSource _speaker;
    [SerializeField] private TextToSpeechHandler _textToSpeechHandler;

    [SerializeField] private Languages originLanguage = Languages.en;
    public bool transText;
    public Languages userTargetLanguage = Languages.es; // Set based on user preference

    public string inputTxt;

    private void Start()
    {
        OnObjectRecognizedHandler(inputTxt);
    }

    private void Update()
    {
        if (transText)
        {
            transText = false;
            Record();
        }
    }

    private void OnEnable()
    {
        aiDetection.OnObjectRecognized += OnObjectRecognizedHandler;
    }

    private void OnDisable()
    {
        aiDetection.OnObjectRecognized -= OnObjectRecognizedHandler;
    }

    private void OnObjectRecognizedHandler(string recognizedObject)
    {
        Debug.Log("Received AI recognized object: " + recognizedObject);

        // Create UnityEvent callback to receive translated text
        UnityEvent<string> translationCallback = new UnityEvent<string>();
        translationCallback.AddListener(translatedText =>
        {
            Debug.Log("Translated text: " + translatedText);
            translatedResultTxt.text = translatedText;
            translationCallback.RemoveAllListeners();
            Speak(translatedText);
        });

        // Call TranslateText to translate detected object name
        translationManager.TranslateText(translationCallback, recognizedObject, originLanguage, userTargetLanguage);
    }

    public void Record()
    {
        string recognizedText = translatedResultTxt.text;
        m_speechToTextManager.StartRecording(recognizedText, recognizedSpeech =>
        {
            Debug.Log("Speech Recognised " + recognizedSpeech);
            bool isCorrect = recognizedText.ToUpper() == recognizedSpeech.ToUpper();
            if (isCorrect)
                GivePositiveFeedback();
            else
                GiveNegativeFeedback();
        });
    }

    private void GivePositiveFeedback()
    {
        Debug.Log("Very good");
    }

    private void GiveNegativeFeedback()
    {
        Debug.Log("try again");
    }

    /// <summary>
    /// Initiates Text to Speech by Google Rest Api and plays audio clip upon successful request.
    /// </summary>
    public void Speak(string text)
    {
        Action<AudioClip> audioReceived = AudioClipReceived;
        Action<BadRequestData> errorReceived = ErrorReceived;
        _textToSpeechHandler.GetSpeechAudioFromGoogle(text, userTargetLanguage.ToString(), audioReceived, errorReceived);
    }

    /// <summary>
    /// Called on error during Text to Speech request.
    /// </summary>
    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
    }

    /// <summary>
    /// Called on successful Text to Speech request.
    /// </summary>
    private void AudioClipReceived(AudioClip clip)
    {
        _speaker.Stop();
        _speaker.clip = clip;
        _speaker.Play();
    }
}