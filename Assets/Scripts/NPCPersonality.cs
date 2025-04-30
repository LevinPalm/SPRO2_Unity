using UnityEngine;
using UnityEngine.UI; // Needed for Button potentially if disabling UI
using TMPro;
using OpenAI; // Make sure you have the correct OpenAI library imported!
using System.Collections.Generic;
using System.Threading.Tasks; // Needed for Task

public class NPCPersonality : MonoBehaviour
{
    // --- Constants for Location Keys ---
    private const string LocationTable = "Table";
    private const string LocationWhirlpool = "Whirlpool";
    private const string LocationKitchen = "Kitchen";

    // --- API Key Configuration ---
    [Header("OpenAI Configuration")]
    [SerializeField]
    private string openAiApiKey = "YOUR_API_KEY_HERE"; // !!! IMPORTANT: Replace with your actual key !!!
    // Consider using more secure methods like ScriptableObjects or environment variables for production builds.

    // --- UI References ---
    [Header("Location UI Menus")]
    public GameObject tableMenu;
    public GameObject whirlpoolMenu;
    public GameObject kitchenMenu;

    [Header("Input Fields")]
    public TMP_InputField tableInputField;
    public TMP_InputField whirlpoolInputField;
    public TMP_InputField kitchenInputField;

    // Add corresponding Submit Buttons if you want to disable them during API calls
    // public Button tableSubmitButton;
    // public Button whirlpoolSubmitButton;
    // public Button kitchenSubmitButton;

    [Header("Dialogue Outputs")]
    public TMP_Text tableDialogueText;
    public TMP_Text whirlpoolDialogueText;
    public TMP_Text kitchenDialogueText;

    [Header("Personality Prompts")]
    [TextArea(4, 10)] public string tablePersonalityPrompt = "You are a wise scholar at a table.";
    [TextArea(4, 10)] public string whirlpoolPersonalityPrompt = "You are a mystical being from a whirlpool.";
    [TextArea(4, 10)] public string kitchenPersonalityPrompt = "You are a friendly chef in a kitchen.";

    // --- Runtime Variables ---
    private OpenAIApi openai;
    // Dictionary to store conversation history per location
    private Dictionary<string, List<ChatMessage>> conversationHistories;

    private void Start()
    {
        // --- Initialize OpenAI API ---
        // This assumes your library takes the key in the constructor. Adjust if needed.
        // Handle potential errors during initialization if necessary.
        if (string.IsNullOrEmpty(openAiApiKey) || openAiApiKey == "YOUR_API_KEY_HERE")
        {
            Debug.LogError("OpenAI API Key is not set in the NPCPersonality script inspector!");
            // Optionally disable the component or prevent interaction
            // this.enabled = false;
            // return;
        }
        openai = new OpenAIApi(openAiApiKey);

        // --- Initialize Conversation Histories ---
        conversationHistories = new Dictionary<string, List<ChatMessage>>
        {
            { LocationTable, new List<ChatMessage>() },
            { LocationWhirlpool, new List<ChatMessage>() },
            { LocationKitchen, new List<ChatMessage>() }
        };

        // --- Initialize UI State ---
        HideAllMenus();
    }

    private void HideAllMenus()
    {
        tableMenu.SetActive(false);
        whirlpoolMenu.SetActive(false);
        kitchenMenu.SetActive(false);
    }

    // Call this from UI Buttons or other interactions, passing the correct constant
    public void ShowMenu(string locationKey) // Use the constants when calling this
    {
        HideAllMenus();

        switch (locationKey)
        {
            case LocationTable:
                tableMenu.SetActive(true);
                // Optional: Clear previous dialogue text or load history display
                break;
            case LocationWhirlpool:
                whirlpoolMenu.SetActive(true);
                break;
            case LocationKitchen:
                kitchenMenu.SetActive(true);
                break;
            default:
                Debug.LogWarning($"ShowMenu called with unknown location key: {locationKey}");
                break;
        }
    }

    public void ExitMenu()
    {
        HideAllMenus();
    }

    // Call this from the Submit Button's OnClick() event in the Inspector, passing the location constant
    public void SubmitInput(string locationKey)
    {
        switch (locationKey)
        {
            case LocationTable:
                HandleNPCConversation(locationKey, tableInputField, tableDialogueText, tablePersonalityPrompt);
                break;
            case LocationWhirlpool:
                HandleNPCConversation(locationKey, whirlpoolInputField, whirlpoolDialogueText, whirlpoolPersonalityPrompt);
                break;
            case LocationKitchen:
                HandleNPCConversation(locationKey, kitchenInputField, kitchenDialogueText, kitchenPersonalityPrompt);
                break;
            default:
                Debug.LogWarning($"SubmitInput called with unknown location key: {locationKey}");
                break;
        }
    }

    // Handles the actual API call and conversation logic
    private async void HandleNPCConversation(string locationKey, TMP_InputField inputField, TMP_Text dialogueOutput, string personalityPrompt)
    {
        string playerInput = inputField.text;

        if (string.IsNullOrWhiteSpace(playerInput))
        {
            Debug.Log("Player input is empty.");
            return; // Don't proceed if input is empty
        }

        if (!conversationHistories.ContainsKey(locationKey))
        {
            Debug.LogError($"Conversation history not found for key: {locationKey}");
            return;
        }

        // Get the history for the current location
        var history = conversationHistories[locationKey];

        // Add the system prompt only if history is empty (first interaction)
        if (history.Count == 0)
        {
            history.Add(new ChatMessage { Role = "system", Content = personalityPrompt });
        }

        // Add the latest player message to the history
        history.Add(new ChatMessage { Role = "user", Content = playerInput });

        // --- Update UI - Show Thinking & Disable Input ---
        dialogueOutput.text = "Thinking...";
        inputField.interactable = false; // Disable input field
        // Optional: Disable submit button here if you have references to them

        try
        {
            // --- Create the API Request ---
            var chatRequest = new CreateChatCompletionRequest
            {
                Model = "gpt-3.5-turbo", // Or whichever model you prefer
                Messages = history // Send the whole history
                // Add other parameters like Temperature, MaxTokens if needed
            };

            // --- Call the API ---
            var response = await openai.CreateChatCompletion(chatRequest); // Use await Task if not called by UI event

            // --- Process the Response ---
            if (response.Choices != null && response.Choices.Count > 0) // && response.Choices[0].Message != null
            {
                string aiResponseContent = response.Choices[0].Message.Content.Trim();
                dialogueOutput.text = aiResponseContent;

                // Add AI response to history
                history.Add(new ChatMessage { Role = "assistant", Content = aiResponseContent });

                // Clear the input field ONLY after successful processing
                inputField.text = "";
            }
            else
            {
                dialogueOutput.text = "I received a strange response. Try again?";
                Debug.LogWarning($"OpenAI response was null or empty. Response object: {response}");
                // Optionally remove the last user message from history if the call failed?
                // history.RemoveAt(history.Count - 1);
            }
        }
        catch (System.Exception ex) // Catch any errors during the API call
        {
            dialogueOutput.text = "Sorry, an error occurred. Couldn't connect or process the request.";
            Debug.LogError($"OpenAI API Error in {locationKey}: {ex.Message}\n{ex.StackTrace}");
            // Optionally remove the last user message from history because the call failed
            // history.RemoveAt(history.Count - 1);
        }
        finally // Runs whether try succeeded or failed
        {
            // --- Re-enable UI ---
            inputField.interactable = true; // Re-enable input field
            // Optional: Re-enable submit button here
        }

        // Optional: Limit history size to prevent excessive token usage/cost
        // Implement logic here to remove older messages from 'history' if it gets too long.
    }
}
