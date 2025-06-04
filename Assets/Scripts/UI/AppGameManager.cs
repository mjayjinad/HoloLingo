using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Lib;
using Oculus.Interaction.Samples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppGameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text wordPromptTxt;
    [SerializeField] private Toggle micToggle;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private GameObject micUI;
    [SerializeField] private GameObject homeUI;
    [SerializeField] private GameObject mainPageUI;

    [Header("Logic Referrences")]
    [SerializeField] private AITranslationConnector m_translationConnector;
    [SerializeField] private DropDownGroup m_dropdownGroup;

    [Header("Prompts Templates")]
    [SerializeField]
    private List<string> promptTemplates = new List<string>()
    {
        "Say the word '{0}' out loud in {1}!",
        "Can you say '{0}' in {1}?",
        "Try saying '{0}' in {1}.",
        "How do you say '{0}' in {1}? Give it a try!",
        "How would you say '{0}' in {1}?",
        "Please say the word '{0}' in {1}!"
    };

    //public bool test;
    private string infoPrompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        homeBtn.onClick.AddListener(OpenHomeMenu);
        startBtn.onClick.AddListener(OpenMainUI);
        micToggle.onValueChanged.AddListener(RecordUser);

        micUI.SetActive(false);
        InitializeInfo();
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

    public void ShowRandomPrompt(string recognisedWord, string languageName)
    {
        int randomIndex = Random.Range(0, promptTemplates.Count);
        string selectedTemplate = promptTemplates[randomIndex];

        string prompt = string.Format(selectedTemplate, recognisedWord, languageName);

        infoPrompt = prompt;
    }

    public void PromptUser(string recognisedWord)
    {
        ShowRandomPrompt(recognisedWord, m_translationConnector.userTargetLanguage.ToDisplayString());
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

    private void OpenHomeMenu()
    {
        mainPageUI.SetActive(false);
        homeUI.SetActive(true);
    }

    private void OpenMainUI()
    {
        InitializeInfo();
        homeUI.SetActive(false);
        mainPageUI.SetActive(true);
    }

    public void SelectLanguage()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);

        m_translationConnector.userTargetLanguage = LanguageExtensions.ToLanguageEnum(m_dropdownGroup.Title.text);
        Debug.Log("Selected Language: " + m_dropdownGroup.Title.text);
    }
}