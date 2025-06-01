using System.Collections;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using PassthroughCameraSamples;
using TMPro;
using UnityEngine;
using static TreeEditor.TextureAtlas;

public class PassthroughCameraDescription : MonoBehaviour
{
    [SerializeField] private WebCamTextureManager webCamTextureManager;
    [SerializeField] private OpenAIConfiguration configuration;
    [SerializeField] private Texture2D imageToAnalyze;
    [SerializeField] private TMP_Text resultTxt;

    private Texture2D picture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //StartCoroutine(Delay());
    }

    // Update is called once per frame
    private void Update()
    {
        if (webCamTextureManager.WebCamTexture != null)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                TakePicture();
                SubmitImage();
            }
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(3);

        SubmitImage();
    }

    private void TakePicture()
    {
        int width = webCamTextureManager.WebCamTexture.width;
        int height = webCamTextureManager.WebCamTexture.height;

        if (picture == null)
        {
            picture = new Texture2D(width, height);
        }

        Color32[] pixels = new Color32[width * height];
        webCamTextureManager.WebCamTexture.GetPixels32(pixels);

        picture.SetPixels32(pixels);
        picture.Apply();
    }

    private async void SubmitImage()
    {
        var api = new OpenAIClient(configuration);

        var messages = new List<Message>();

        Message systemMessage = new Message(Role.System, "You are an AI assistant that identifies the main object in an image. "
            + "Describe only the main object centered in the image using one or two simple words (noun or noun phrase). "
            + "Do not mention hands, arms, VR controllers, backgrounds, or other distractions. "
            + "Do not add any extra explanation or punctuation, just the object name. "
            + "Example: chair, table, backpack.");
        //Message systemMessage = new Message(Role.System, "Describe the image in 5 words, don't write anything bold, just plain text. "
        //    + "Just describe the main object at the center of the image and ignore everthing else. "
        //    + "Ignore the hand, arms, and the Vr controller in your description");

        List<Content> imageContents = new List<Content>();
        string textContent = "What is in this image";
        Texture2D imageContent = picture;
        //Texture2D imageContent = imageToAnalyze;

        imageContents.Add(textContent);
        imageContents.Add(imageContent);

        Message imageMessage = new Message(Role.User, imageContents);

        messages.Add(systemMessage);
        messages.Add(imageMessage);

        var chatRequest = new ChatRequest(messages, model: Model.GPT4o);

        var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

        Debug.Log("here is the result" + result.FirstChoice);
        resultTxt.text = result.FirstChoice;
    }
}
