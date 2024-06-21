## Unity Setup

This is a Unity project developed in [Unity Editor 2021.3.11f1](https://unity3d.com/unity/whats-new/2021.3.11).  It uses LLMs such as GPT-4 to generate text, Cognitive Service text-to-speech to generate speech, and Freesound.org to choose background sounds.

* Once you have Unity (Hub) installed, open this project and open the Assets > Scenes > AImagery_Open_Source scene. You should see a black/gray UI with a disclaimer at the top and inputs for desired mood, current mood, scenery, and heart rate, followed by a play button.

* Before being able to play/start the app, you'll need to configure some places in the code and create some key files.

  * In Assets > Scripts > AIStoryAzure.cs: The code is by default set up to use OpenAI's chat completion endpoint with gpt-4o.  You can change these values in AIStoryAzure's Awake() function.

  * In Assets > Scripts > Speech.cs: The code is by default set up to use the EastUS Cognitive Services region.  You can change it near the very start of the Speech class.

  * In Assets > SecretKeys folder: Create it if it doesn't exist, and then place the following files inside:
    * openai.txt: This file should contain the API key for the endpoint specified in AIStoryAzure class' Awake() function.
    * cognitive-services.txt: This file should contain an API key for your Azure Cognitive Services speech service in the region specified in the Speech class' Region field.
    * freesound.txt: This file should contain an API key for your Freesound.org account. You can get a free account at https://freesound.org/apiv2/apply

  * In the Unity editor's Hierarchy view, select the AImagery_OpenSource > Camera > GenerativeAI > AIStory object. Then on the Inspector panel on the right, you will need to assign your openai.txt file to the AIStoryAzure's "AzureOpenAIKeyFile" property. You can do this by dragging-and-dropping the file from the Assets window to the AzureOpenAIKeyFile property in the Inspector.

   * Now select the Camera > GenerativeAI > AISound object, and repeat the same process to assign the freesound.txt file to the "FreesoundKeyFile" property in the Inspector.

   * The AISpeech object doesn't need anything done since it reads from cognitive-services.txt directly.

* Next, you will need to download and add some packages to the Unity project:

    * Download and import to Unity [the Microsoft Azure Cognitive Services API Speech SDK](https://aka.ms/csspeech/unitypackage). More details about building & running speech to build & run in different platforms can be found [here](https://github.com/Azure-Samples/cognitive-services-speech-sdk/tree/master/quickstart/csharp/unity/text-to-speech).

## Biofeedback
If you do not have the heart rate sensor you can simulate the sound of a heart beating at the desired pace by checking the box "FakeHR" and changing the Fake HR Value in the Inspector:  
![image (2)](https://github.com/microsoft/AImagery/assets/33366055/e4b12aae-13f9-473b-a9ac-da5eb00396fa)



If using Mionix Naos QG Mouse (only works on Windows):

* Connect mouse to PC and install [Naos QG Firmware V0.16 Firmware](https://support.mionix.net/hc/en-us/articles/115000615446-Naos-QG-Firmware-v0-16)
* Install [Mionix Hub Software v.1.6](https://support.mionix.net/hc/en-us/articles/360000504403-Mionix-Hub-Software-v1-6-Windows-MAC)
* Before testing in Unity, make sure you can see your heart rate information with the hub.
* In Assets > Scripts > HeartBeatSounds.cs: Uncomment the Mionix lines.
 

