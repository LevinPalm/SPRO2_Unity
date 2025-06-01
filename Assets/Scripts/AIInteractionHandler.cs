using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json; // Install Newtonsoft.Json via Unity's Package Manager

public class GeminiInteractionHandler : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField inputField;
    public TMP_Text outputText;
    public Button submitButton;
    public Button exitButton;

    [Header("Gemini Settings")]
    [TextArea(4, 10)] public string personalityPrompt = "You are a helpful assistant."; //will be replaced by vendor specific prompts
    public string apiKey = "Gemini API key";

    private HttpClient httpClient;
    private SimpleInteractable interactable;

    private const string GeminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-pro:generateContent?key=";

    private void Start()
    {
        httpClient = new HttpClient();
        interactable = GetComponent<SimpleInteractable>();

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExit);
    }

    private async void OnSubmit()
    {
        string playerInput = inputField.text;
        if (string.IsNullOrWhiteSpace(playerInput)) //check for valid input
            return;

        outputText.text = "Thinking...";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = personalityPrompt + "\n" + playerInput }
                    }
                }
            }
        };

        string jsonBody = JsonConvert.SerializeObject(requestBody);

        try
        {
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(GeminiApiUrl + apiKey, content);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseJson);

            if (geminiResponse != null && geminiResponse.candidates != null && geminiResponse.candidates.Length > 0)
            {
                outputText.text = geminiResponse.candidates[0].content.parts[0].text.Trim();
            }
            else
            {
                outputText.text = "I have nothing to say.";
            }
        }
        catch (HttpRequestException e)
        {
            Debug.LogError("Request error: " + e.Message);
            outputText.text = "Error connecting to Gemini.";
        }
    }

    private void OnExit()
    {
        if (interactable != null)
        {
            interactable.ExitInteract();    //calling the exit function of the script attached to the vendor object
        }
    }
}

// Response classes matching Gemini API structure
[System.Serializable]
public class GeminiResponse
{
    public Candidate[] candidates;
}

[System.Serializable]
public class Candidate
{
    public Content content;
}

[System.Serializable]
public class Content
{
    public Part[] parts;
}

[System.Serializable]
public class Part
{
    public string text;
}
