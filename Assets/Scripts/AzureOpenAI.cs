// Copyright (c) Microsoft Corporation.
// Licensed under the Microsoft Research License Terms.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading.Tasks;

// Grabs GPT completions via the Azure OpenAI or ChatGPT API, with optional result caching.
public class AzureOpenAI : MonoBehaviour
{
    // This value can be found in the Keys & Endpoint section when examining your resource from the Azure portal. You can use either KEY1 or KEY2.
    public static string key = null;

    // This value can be found in the Keys & Endpoint section when examining your resource from the Azure portal. Alternatively, you can find the value in Azure OpenAI Studio > Playground > Code View. An example endpoint is: https://llmexplorations.openai.azure.com/openai/deployments/HUE-gpt4-32k/chat/completions?api-version=2023-03-15-preview.
    public static string endpoint = null;

    public Task<string> GetCompletion(string prompt, string model = null, bool useCache = false, string cacheKey = null, int maxTokens = 100, string[] stop = null, float temperature = 0.7f, float presencePenalty = 0f, float frequencyPenalty = 0f, bool showResultInfo = false, int responseLengthMaxGoal = 0)
    {
        var aiParams = new AzureOpenAIParams()
        {
            prompt = prompt,
            maxTokens = maxTokens,
            temperature = temperature,
            presencePenalty = presencePenalty,
            frequencyPenalty = frequencyPenalty,
            stop = stop,
            model = model,
        };

        if (responseLengthMaxGoal > 0)
        {
            aiParams.maxTokens = GetApproximateTokensNeededForResponseLength(prompt, responseLengthMaxGoal);
        }

        return GetCompletion(aiParams, useCache, cacheKey, showResultInfo);
    }

    public async Task<string> GetCompletion(AzureOpenAIParams aiParams, bool useCache = false, string cacheKey = null, bool showResultInfo = false)
    {

        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Azure OpenAI Service API key not set.");
            return null;
        }

        if (string.IsNullOrEmpty(endpoint))
        {
            Debug.LogError("Azure OpenAI Service endpoint not set.");
            return null;
        }

        // Detect if we are using chat completion API vs text completion API.
        if (endpoint.Contains("chat"))
        {
            aiParams.messages = new List<OpenAIMessage>
            {
                new OpenAIMessage("user", aiParams.prompt)
            };
            aiParams.prompt = null;
        }

        var serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        string jsonString = JsonConvert.SerializeObject(aiParams, Formatting.None, serializerSettings);

        UnityWebRequest www = UnityWebRequest.Post(endpoint, "");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + key);
        www.SetRequestHeader("api-key", key);
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonString));
        www.downloadHandler = new DownloadHandlerBuffer();

        await www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
            www.Dispose();
            return null;
        }
        else
        {
            string text = www.downloadHandler.text;
            string result = GetResultFromJsonString(text, showResultInfo: showResultInfo);

            www.Dispose();

            return result;
        }
    }

    string GetResultFromJsonString(string jsonString, bool showResultInfo = false, bool cacheWasUsed = false)
    {
        string result = null;

        var jsonData = JsonConvert.DeserializeObject(jsonString) as Newtonsoft.Json.Linq.JObject;

        var error = jsonData.SelectToken("error.message");
        if (error != null)
        {
            throw new System.Exception(error.ToString());
        }

        if (jsonData.SelectToken("choices[0].message") != null)
        {
            result = jsonData.SelectToken("choices[0].message.content").ToString();
        }
        else
        {
            result = jsonData.SelectToken("choices[0].text").ToString();
        }

        if (showResultInfo)
        {
            Debug.Log("- Finish reason: " + jsonData.SelectToken("choices[0].finish_reason").ToString());
            int totalTokens = jsonData.SelectToken("usage.total_tokens").ToObject<int>();

            string info = "- Total tokens used: " + totalTokens;
            if (cacheWasUsed) { info += " (but 0 as retrieved from cache)"; }
            Debug.Log(info);
        }

        return result;
    }

    public static int GetApproximateTokensNeededForResponseLength(string prompt, int responseLengthGoal)
    {
        const int approximateTokenToCharRatio = 4;
        return (prompt.Length + responseLengthGoal) / approximateTokenToCharRatio;
    }
}

public static class Extensions
{
    public static System.Runtime.CompilerServices.TaskAwaiter GetAwaiter(this AsyncOperation operation)
    {
        var source = new TaskCompletionSource<object>();
        operation.completed += source.SetResult;
        return (source.Task as Task).GetAwaiter();
    }
}