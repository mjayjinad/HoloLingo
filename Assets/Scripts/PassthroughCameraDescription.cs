using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using UnityEngine;

public class PassthroughCameraDescription : MonoBehaviour
{
    [SerializeField] private OpenAIConfiguration configuration;
    [SerializeField] private Texture2D imageToAnalyze;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SubmitImage();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private async void SubmitImage()
    {
        var api = new OpenAIClient(configuration);

        var messages = new List<Message>();

        Message systemMessage = new Message(Role.System, "You are a helpful asistant");

        List<Content> imageContents = new List<Content>();
        string textContent = "What is in this image";
        Texture2D imageContent = imageToAnalyze;

        imageContents.Add(textContent);
        imageContents.Add(imageContent);

        Message imageMessage = new Message(Role.User, imageContents);

        messages.Add(systemMessage);
        messages.Add(imageMessage);

        var chatRequest = new ChatRequest(messages, model: Model.GPT4o);

        var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

        //Debug.Log("here is the result" + result.FirstChoice);
    }
}
