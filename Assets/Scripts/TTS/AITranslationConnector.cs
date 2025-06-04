using System;
using System.Collections.Generic;
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
    [SerializeField] private TextToSpeechHandler _textToSpeechHandler;
    [SerializeField] private AppGameManager m_appGameManager;

    [SerializeField] private AudioSource _speaker;
    [HideInInspector]
    public string translatedWord;

    [SerializeField] private Languages defaultLanguage = Languages.en;
    [SerializeField] private Voices voiceName = Voices.Aoede;
    public Languages userTargetLanguage = Languages.es; // Set based on user preference

    [Header("Positive Feedback Prompts")]
    [SerializeField]
    private List<string> positiveFeedbackTemplates = new List<string>()
    {
        "Great! You said \"{0}\", that's absolutely correct!",
        "Well done! \"{0}\" is exactly right.",
        "Excellent! Saying \"{0}\" is perfect.",
        "Fantastic! You nailed the pronunciation of \"{0}\".",
        "Awesome! \"{0}\" is the right word.",
        "Brilliant! Your pronunciation of \"{0}\" was spot on."
    };

    [Header("Negative Feedback Prompts")]
    [SerializeField]
    private List<string> negativeFeedbackTemplates = new List<string>()
    {
        "Hmmm... you said \"{0}\" that doesn't seem correct. The right word is \"{1}\". Why don't you give it another try?",
        "Almost there! You said \"{0}\", but the correct word is \"{1}\". Try again!",
        "Not quite. You said \"{0}\", but it should be \"{1}\". Give it another shot!",
        "Oops! \"{0}\" isn't quite right. The right word is \"{1}\". Have another go!",
        "Close, but you said \"{0}\". The correct word is \"{1}\". Try once more!"
    };

    [Header("Unrecognized Speech Prompts")]
    [SerializeField]
    private List<string> unrecognizedSpeechTemplates = new List<string>()
    {
        "Hmm, I didn't catch that. Could you say it again?",
        "Sorry, I couldn’t understand you. Please try saying the word once more.",
        "I’m not sure what you said. Let’s give it another try!",
        "Oops, that was unclear. Please repeat the word.",
        "Can you say that again a bit clearer? I want to get it right!",
        "Sorry, I missed that. Please say the word again."
    };

    public string inputTxt;
    private string feedback;

    private void Start()
    {
        //OnObjectRecognizedHandler(inputTxt);
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

        UnityEvent<string> translationCallback = new UnityEvent<string>();
        translationCallback.AddListener(translatedText =>
        {
            Debug.Log("Translated text: " + translatedText);
            translatedWord = translatedText;
            translationCallback.RemoveAllListeners();
        });

        translationManager.TranslateText(translationCallback, recognizedObject, defaultLanguage, userTargetLanguage);
    }

    public void Record()
    {
        m_speechToTextManager.StartRecording(translatedWord, recognizedSpeech =>
        {
            Debug.Log("Speech Recognised: " + recognizedSpeech);

            if (string.IsNullOrWhiteSpace(recognizedSpeech) || recognizedSpeech.Length < 2)
            {
                GiveUnrecognizedSpeechFeedback();
                return;
            }

            bool isCorrect = translatedWord.ToUpper() == recognizedSpeech.ToUpper();
            if (isCorrect)
                GivePositiveFeedback();
            else
                GiveNegativeFeedback(recognizedSpeech);
        });
    }

    private void GivePositiveFeedback()
    {
        feedback = GetRandomPositiveFeedback(translatedWord);
        Debug.Log(feedback);

        ReadUserPromptText(feedback);
        m_appGameManager.OnAudioRecordStop(feedback);
    }

    private void GiveNegativeFeedback(string recognizedWord)
    {
        feedback = GetRandomNegativeFeedback(recognizedWord, translatedWord);
        Debug.Log(feedback);

        ReadUserPromptText(feedback);
        m_appGameManager.OnAudioRecordStop(feedback);
    }

    private void GiveUnrecognizedSpeechFeedback()
    {
        feedback = GetRandomUnrecognizedPrompt();
        Debug.Log(feedback);

        ReadUserPromptText(feedback);
        m_appGameManager.OnAudioRecordStop(feedback);
    }

    private string GetRandomPositiveFeedback(string correctWord)
    {
        if (positiveFeedbackTemplates == null || positiveFeedbackTemplates.Count == 0)
            return $"Great! You said \"{correctWord}\", that's very correct!";

        int index = UnityEngine.Random.Range(0, positiveFeedbackTemplates.Count);
        string template = positiveFeedbackTemplates[index];
        return string.Format(template, correctWord);
    }

    private string GetRandomNegativeFeedback(string recognizedWord, string correctWord)
    {
        if (negativeFeedbackTemplates == null || negativeFeedbackTemplates.Count == 0)
            return $"Hmmm... you said \"{recognizedWord}\" that doesn't seem correct. The right word is \"{correctWord}\". Why don't you give it another try?";

        int index = UnityEngine.Random.Range(0, negativeFeedbackTemplates.Count);
        string template = negativeFeedbackTemplates[index];
        return string.Format(template, recognizedWord, correctWord);
    }

    private string GetRandomUnrecognizedPrompt()
    {
        if (unrecognizedSpeechTemplates == null || unrecognizedSpeechTemplates.Count == 0)
            return "Sorry, I didn't understand that. Please try again.";

        int index = UnityEngine.Random.Range(0, unrecognizedSpeechTemplates.Count);
        return unrecognizedSpeechTemplates[index];
    }

    /// <summary>
    /// Initiates Text to Speech by Google Rest Api and plays audio clip upon successful request.
    /// </summary>
    public void ReadTranslatedText(string text)
    {
        Action<AudioClip> audioReceived = AudioClipReceived;
        Action<BadRequestData> errorReceived = ErrorReceived;
        _textToSpeechHandler.GetSpeechAudioFromGoogle(text, LanguageToGoogleCode(userTargetLanguage), SelectedVoice(userTargetLanguage, voiceName), audioReceived, errorReceived);
    }

    /// <summary>
    /// Initiates Text to Speech by Google Rest Api and plays audio clip upon successful request.
    /// </summary>
    public void ReadUserPromptText(string text)
    {
        Action<AudioClip> audioReceived = AudioClipReceived;
        Action<BadRequestData> errorReceived = ErrorReceived;
        _textToSpeechHandler.GetSpeechAudioFromGoogle(text, LanguageToGoogleCode(defaultLanguage), SelectedVoice(defaultLanguage, voiceName), audioReceived, errorReceived);
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

    private string SelectedVoice(Languages lang, Voices voiceName)
    {
        string voice = LanguageToGoogleCode(lang) + "-Chirp3-HD-" + voiceName;

        return voice;
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
}