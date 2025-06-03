using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Lib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppGameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text wordPromptTxt;
    [SerializeField] private Toggle micToggle;
    [SerializeField] private GameObject micUI;

    [Header("Logic Referrences")]
    [SerializeField] private AITranslationConnector m_translationConnector;

    [Header("Prompts Templates")]
    [SerializeField] private List<string> promptTemplates = new List<string>()
    {
        "Say the word for this in {0}.",
        "Can you say this word in {0}?",
        "Try saying this in {0}.",
        "How do you say this in {0}?",
        "Please say the word for this in {0}."
    };

    //public bool test;
    private string infoPrompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        micUI.SetActive(false);
        InitializeInfo();
        micToggle.onValueChanged.AddListener(RecordUser);
    }

    private void Update()
    {
        wordPromptTxt.text = infoPrompt;
        //if (test)
        //{
        //    PromptUser();
        //    test = false;
        //}
    }

    private void InitializeInfo()
    {
        infoPrompt = "Pick up an object, then press the A button to identify it.";
    }

    public void ShowRandomPrompt(string languageName)
    {
        int randomIndex = Random.Range(0, promptTemplates.Count);
        string selectedTemplate = promptTemplates[randomIndex];

        string prompt = string.Format(selectedTemplate, languageName);

        infoPrompt = prompt;
    }

    public void PromptUser()
    {
        ShowRandomPrompt(m_translationConnector.userTargetLanguage.ToDisplayString());
        m_translationConnector.ReadUserPromptText(infoPrompt);
        micUI.gameObject.SetActive(true);
    }

    private void RecordUser(bool value)
    {
        m_translationConnector.Record();
    }

    public void OnAudioRecordStop(string feedback)
    {
        micToggle.onValueChanged.RemoveListener(RecordUser);
        micToggle.isOn = false;
        micToggle.onValueChanged.AddListener(RecordUser);
        infoPrompt = feedback;
    }
}