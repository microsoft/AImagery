// Copyright (c) Microsoft Corporation.
// Licensed under the Microsoft Research License Terms.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Sound : MonoBehaviour
{
    /// <summary>
    /// Text file containing a Freesound API key obtained from https://freesound.org/apiv2/apply
    /// </summary>
    public TextAsset FreesoundKeyFile;

    private string[] soundTags = null;

    // Update is called once per frame
    async void Update()
    {
        if (soundTags == null && AIStoryAzure.SoundTags != null)
        {
            soundTags = AIStoryAzure.SoundTags;

            // It's unlikely that we'll find a match that has all of 3 or more tags.
            var tagStack = new Stack<string>(soundTags.Take(3));

            while (tagStack.Count > 0)
            {
                var sound = await GetSound(tagStack.Reverse());
                if (sound != null)
                {
                    var audioSource = GetComponent<AudioSource>();
                    audioSource.clip = sound;
                    audioSource.Play();
                    break;
                }

                // Try again with fewer tags.
                _ = tagStack.Pop();
            }
        }
    }

    private async Task<AudioClip> GetSound(IEnumerable<string> keywords)
    {
        var queryBasedFilter = "query=" + string.Join(" ", keywords.Select(keyword => UnityWebRequest.EscapeURL(keyword)));
        var tagBasedFilter = "filter=" + string.Join(" ", keywords.Select(tag => $"tag:{UnityWebRequest.EscapeURL(tag)}"));
        var url = $"https://freesound.org/apiv2/search/text/?{queryBasedFilter}&token={FreesoundKeyFile.text}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var json = www.downloadHandler.text;
            var results = JObject.Parse(json);
            Debug.Log($"{results["count"]} sounds found for {queryBasedFilter}:\n" + string.Join("\n", ((JArray)results["results"]).Select(r => r["name"])));

            if ((int)results["count"] > 0)
            {
                var sound = results["results"][0];
                //Debug.Log($"Getting preview URL for sound {sound["id"]}: {sound["name"]} by {sound["username"]} ({sound["license"]})");

                // We could download the original sound file, but it may be in a large/uncompressed or unknown format.
                // var soundUrl = $"https://freesound.org/apiv2/sounds/{sound["id"]}/download/?token={FreesoundKeyFile.text}";

                // But Freesound generates previews for sounds that we can use instead for our purposes.
                var soundUrl = $"https://freesound.org/apiv2/sounds/{sound["id"]}?token={FreesoundKeyFile.text}";
                www = UnityWebRequest.Get(soundUrl);
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    json = www.downloadHandler.text;
                    var soundInstance = JObject.Parse(json);

                    // Now download the preview from the URL we got from the sound instance.
                    var previewUrl = (string)soundInstance["previews"]["preview-hq-mp3"];
                    Debug.Log($"Downloading preview for sound {sound["id"]}: {sound["name"]} by {sound["username"]} ({sound["license"]})");

                    www = UnityWebRequestMultimedia.GetAudioClip(previewUrl, AudioType.MPEG);
                    await www.SendWebRequest();
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        return DownloadHandlerAudioClip.GetContent(www);
                    }
                }
            }
        }

        // If we get here, we didn't find a sound.
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{www.uri} returned {www.result}");
        }

        return null;
    }
}
