// Copyright (c) Microsoft Corporation.
// Licensed under the Microsoft Research License Terms.

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AIStoryAzure : MonoBehaviour
{
    public Text storyGenerated, title;
    public string realTimeHR;
    public Text userGoal, userScenery, userFeeling;
    public static string GPTResult, userSceneryPrompt;
    public static string[] SoundTags;
    public Button startButton;
    public TextAsset AzureOpenAIKeyFile;

    public string GPTModel;
    public int maxTokens; 

    void Awake()
    {
        //Safely store key in a .txt file inside a folder "e.g, SecretKeys". Drag and drop in the Inspector the .txt file.
        AzureOpenAI.key = AzureOpenAIKeyFile.text;

        // Set the endpoint for the API that calls the GPT model. E.g: "https://api.openai.com/v1/chat/completions";
        AzureOpenAI.endpoint = "https://api.openai.com/v1/chat/completions";
        
        // Set the model to use. E.g: "gpt4-turbo", "gpt-4o", etc.
        GPTModel = "gpt-4o";
        maxTokens = 4096;
    }

    void Start()
    {
        realTimeHR = HeartBeatSound.roundHR.ToString();
        startButton.onClick.AddListener(TaskOnClick);
        title.text = "The system is waiting for your input:";
        storyGenerated.text = "Type in the boxes below how the user is currently feeling and what's their prefered scenery to relax. "
        +"By default, a heart rate of 40 bpm will be used if no sensor is detected. Press play when ready. You can stop the experience at any time with the pause button. "
        +"Once the story is generated, you can use the arrow keys to move the text up and down.";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {

            GenerateUXParameters();
            Speech.speaking = false;

        }
        realTimeHR = HeartBeatSound.roundHR.ToString();

        if (Input.GetKey(KeyCode.UpArrow))
        {
            //Move uiText down by 1:
            storyGenerated.transform.position = new Vector3(storyGenerated.transform.position.x, storyGenerated.transform.position.y - 1f, storyGenerated.transform.position.z);
        }

        //If key arrow down is being pressed:
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //Move uiText down by 1:
            storyGenerated.transform.position = new Vector3(storyGenerated.transform.position.x, storyGenerated.transform.position.y + 1f, storyGenerated.transform.position.z);

        }
    }

    void TaskOnClick()
    {
        GenerateUXParameters();
        Speech.speaking = false;
        title.text = "Wait for the story and sounds to be generated...";
    }

    // Generates a story based on the prompt
    public async void GenerateUXParameters()
    {
        // Get current HR
        realTimeHR = HeartBeatSound.roundHR.ToString();

        // If no text is input then set these values by default:
        if (userScenery.text == null) userScenery.text = "calming";
        if (userGoal.text == null) userGoal.text = "relax and fall asleep";
        if (userFeeling.text == null) userFeeling.text = "anxious";
        userSceneryPrompt = userScenery.text;

        var textAI = GetComponent<AzureOpenAI>() ?? gameObject.AddComponent<AzureOpenAI>();

        string prompt = "Create an immersive experience that matches the sounds " +
            "with a first-person story based on my heart rate, how I feel and a "
            + userScenery.text + " scenery with lavender scent but no bees. Your goal is to help me relax and fall asleep using guided imagery."
            +"My current heart beats per minute are " + realTimeHR +
            ". I currently feel " + userFeeling.text +
            ". Return in one string with this format:" +
            "[{firstpersonstory};{sounds}] "+
            "where story=firstpersonstory; sounds=freesoundtag1_freesoundtag2; Do not add heart beat soundtags. The 1000 word story will be narrated so take into account that it should last for at least 10 minutes (your voice it's slow at a pace of 100wpm).";

        Debug.Log("Prompt:" + prompt);
        var result = await textAI.GetCompletion(prompt, GPTModel, maxTokens: maxTokens);
        result = result.Substring(result.IndexOf('[') + 1);

        string[] resultArray = result.Split(';');
        string story = resultArray[0];
        Debug.Log("Story:" + story);
        string tags = resultArray.Length > 1 ? resultArray[1].Trim() : null;
        //Debug.Log("Sounds:" + tags);

        // For RAI purposes, we need to double-check that the returned story doesn't contain any sensitive content.
        var safe = resultArray.Length > 1 && await DetermineStorySafety(story);
        Debug.Log("Safe:" + safe);
        if (!safe)
        {
            story = "I'm sorry, please try again with different inputs.";
        }

        storyGenerated.text = story;
        GPTResult = story;
        SoundTags = tags != null ? tags.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Split("_") : null;

        Debug.Log("Sound Tags:" + string.Join(", ", SoundTags));
    }

    // Determines whether a story is safe to narrate
    public async Task<bool> DetermineStorySafety(string story)
    {
        var prompt =
@$"You are tasked with determining whether the provided story violates guidelines related to self-harm, violence or sexual content. Carefully read the entire story and assess its content based on the following criteria:
* Does the story include explicit descriptions of self-harm?
* Does the story promote, encourage, or glorify self-harm in any form?
* Does the story provide methods or instructions for self-harm?
If any of these criteria are met, reply with the word ""Unsafe"". If none of these criteria are met, simply respond with the word ""Safe"".

Story:

{story}";

        var textAI = GetComponent<AzureOpenAI>() ?? gameObject.AddComponent<AzureOpenAI>();
        var result = await textAI.GetCompletion(prompt, GPTModel);
        return Regex.IsMatch(result, @"\b(?<!\bnot.*\s)(safe)\b", RegexOptions.IgnoreCase);
    }
}
