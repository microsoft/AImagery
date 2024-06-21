// Copyright (c) Microsoft Corporation.
// Licensed under the Microsoft Research License Terms.

using System;
using System.Threading;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.IO;
using System.Xml;
using TMPro;
using UnityEngine.UI;

public class Speech: MonoBehaviour
{
    const string pathPrefix = @"Assets/SecretKeys/";
    // Replace with your own subscription key and service region (e.g., "westus").
    private const string Region = "eastus";
    string SubscriptionKey = File.ReadAllText(pathPrefix + "cognitive-services.txt");
    
    public Text inputField;
    public AudioSource audioSource;

    private const int SampleRate = 24000;

    private string message, textAI = null;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    public static bool speaking = false, resetVoice = false;

    // The duration of each pan cycle in seconds
    public float cycleDuration = 6f;

    // The speed of the pan transition in seconds
    public float transitionSpeed = 0.1f;

    // The amplitude of the pan value, from -1 (left) to 1 (right)
    public float panAmplitude = 1f;

    // The offset of the pan value, to adjust the center point
    public float panOffset = 0f;
    
    void Start()
    {
        if (inputField == null)
        {
            message = "inputField property is null! Assign a UI InputField element to it.";
            Debug.LogError(message);
        }
        else
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);

            // The default format is RIFF, which has a riff header.
            // We are playing the audio in memory as audio clip, which doesn't require riff header.
            // So we need to set the format to raw (24KHz for better quality).
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

            // Creates a speech synthesizer.
            // Make sure to dispose the synthesizer after use!

            synthesizer = new SpeechSynthesizer(speechConfig, null);

            synthesizer.SynthesisCanceled += (s, e) =>
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
                message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
            };
        }
    }

    void Update()
    {
        string newMessage = null;
        var startTime = DateTime.Now;

        if (inputField.text != textAI && !speaking)
        {
            resetVoice = true;
            speaking = true;
            string xmlVoice = File.ReadAllText(@"Assets/Voices/ssml.xml");

            textAI = inputField.text;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlVoice);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName("textAI");
            nodes[0].InnerText = textAI;

            xmlDoc.Save(@"Assets/Voices/ssml.xml");

            xmlVoice = File.ReadAllText(@"Assets/Voices/ssml.xml");
            //Debug.Log(xmlVoice);

            // Starts speech synthesis, and returns once the synthesis is started.

            using (var result = synthesizer.StartSpeakingSsmlAsync(xmlVoice).Result)
            {

                // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
                // Use the Unity API to play audio here as a short term solution.
                // Native playback support will be added in the future release.
                var audioDataStream = AudioDataStream.FromResult(result);
                var isFirstAudioChunk = true;
                var audioClip = AudioClip.Create(
                    "Speech",
                    SampleRate * 600, // Can speak 10mins audio as maximum
                    1,
                    SampleRate,
                    true,
                    (float[] audioChunk) =>
                    {
                        var chunkSize = audioChunk.Length;
                        var audioChunkBytes = new byte[chunkSize * 2];
                        var readBytes = audioDataStream.ReadData(audioChunkBytes);
                        if (isFirstAudioChunk && readBytes > 0)
                        {
                            var endTime = DateTime.Now;
                            var latency = endTime.Subtract(startTime).TotalMilliseconds;
                            newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                            isFirstAudioChunk = false;
                        }

                        for (int i = 0; i < chunkSize; ++i)
                        {
                            if (i < readBytes / 2)
                            {
                                audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                            }
                            else
                            {
                                audioChunk[i] = 0.0f;
                            }
                        }

                        if (readBytes == 0)
                        {
                            Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        }
                    });

                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
        else
        {
            resetVoice = false;
        }

        // Calculate the pan value based on the time and the sinus function
        // The sinus function has a period of 2 * pi, so we divide the time by the cycle duration and multiply by 2 * pi
        // The sinus function returns a value between -1 and 1, so we multiply by the pan amplitude and add the pan offset
        // The transition speed determines how fast the pan value changes from -1 to 1 or vice versa, so we use a smoothstep function to interpolate between the previous and the current pan value
        
        float panValue = panAmplitude * Mathf.Sin(Time.time / cycleDuration * 2 * Mathf.PI) + panOffset;
        audioSource.panStereo = Mathf.SmoothStep(audioSource.panStereo, panValue, transitionSpeed);

    }
    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }


}
