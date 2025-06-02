using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppGameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text wordPromptTxt;
    [SerializeField] private Button micBtn;
    [SerializeField] private AITranslationConnector m_translationConnector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PromptUser();

        micBtn.onClick.AddListener(RecordUser);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PromptUser()
    {
        wordPromptTxt.text = "Say the word for this in " + m_translationConnector.userTargetLanguage.ToDisplayString();
    }

    private void RecordUser()
    {
        m_translationConnector.Record();
    }
}
