# AImagery: AI-driven Guided Imagery with Biofeedback

AImagery is an AI-powered multisensory relaxation system designed to reduce anxiety by providing personalized immersive experiences. The system uses AI to create guided imagery based on individual preferences and physiological feedback, incorporating elements like auditory :ear: and olfactory stimuli :nose:. In a study with 32 participants, AImagery demonstrated significant anxiety reduction in those with moderate to high anxiety levels. The system takes user inputs such as heart rate :anatomical_heart:, self-reported mood :slightly_smiling_face:, and preferred scenery:desert_island:, and generates a personalized relaxation story, complemented by calming scents and sounds. The output is a tailored multisensory experience that aims to enhance relaxation and reduce stress.

## Building and Configuration

This is a Unity project developed using the [Unity Editor 2021.3.11f1](https://unity3d.com/unity/whats-new/2021.3.11). Please see [BUILDING.md](BUILDING.md) for instructions.

![screencapture](https://github.com/microsoft/AImagery/assets/33366055/7e54ab9b-c738-416e-865c-c0953539bb13)


## :exclamation:Safety and Red-Teaming ##
We collaborated with the AI Red Team at Microsoft to assess safety risks posed by AImagery in both average and adversarial user scenarios. The user study was conducted in July 2023, thus the model we had access was an older GA model (gpt-4-32k), with 32,768 output tokens, with training data (up to) Sep 2021. The prompts generated at that time were all safe and supervised in real-time by the experimenter, who was at control to stop the experience at any time. The system was re-evaluated on June 2024 from the responsible AI perspective using the OpenAI model GPT-4-Turbo to ensure it doesn't generate harmful content ensuring the safety of users. The assessment was done in predetermined risk categories (violence, self-harm, and sexual content) with automated scoring followed by thorough manual reviews of the model responses. The results with other models may vary.

## Remarks
This project is still in early research stages. Please be expectant of errors when you play with the framework. If you decide to use this project, this is the script we used to describe the system to participants:

_Guided imagery is a type of focused relaxation or meditation. It is a therapeutic approach that has been used in the past to help lower stress and anxiety levels. To do guided imagery, you need to focus on something that makes you feel calm and happy. You might close your eyes and imagine a beautiful place or a positive situation. This might help you to be more aware and relaxed. Your thoughts affect how your body feels. For instance, when you worry about something, you might feel nervous and tense. Your heart might beat faster, and you might have trouble concentrating. But when you think about something nice, you tend to feel more relaxed and peaceful. Your muscles might loosen up, and your mind might clear up. This can help you deal with different kinds of stress better. Today, we will practice guided imagery driven by an AI. You can simply close your eyes and rest. A voice will guide you through the relaxing scenery of your choice, you just need to relax and enjoy the sounds and scent, which might help you relax and fall asleep. You will also hear the sound of a heart beating, that is your real-time heart rate. We will do this for 10 minutes and then we will proceed with the final round of questionnaires. If you fall asleep, don’t worry I will wake you up. You are also welcome to stay for longer if you like. Let me know if you have any questions. Before we get started, it would be helpful if you want to share how you are currently feeling and your preferred scenery that might help you relax and fall asleep. This information will be used by the AI to customize the experience and come up with a story based on how you are feeling, your current heart rate and your preferences. For example, I am currently feeling anxious about a deadline, and my preferred scenery would be a beach in the mexican caribbean with my family, friends and my dog.You can be as specific as you want, if you don’t feel comfortable sharing how you are currently feeling you don’t need to do so._  

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.

## Reporting Security Issues
The current standard to report security issues can be found [here](SECURITY.md).
You can also visit this link: [Reporting Security Issues](https://docs.opensource.microsoft.com/releasing/securing-content/reporting-security-issues/)

## Data Privacy Notice
Please see: [Data Privacy Notice](https://privacy.microsoft.com/en-US/data-privacy-notice)

## Disclaimer
When using AImagery, the user interacts with an AI system that generates and reads text, and only supports English. AImagery’s outputs do not reflect the opinions of Microsoft and may include factual errors. The system does not provide medical or clinical opinions and is not designed to replace the role of qualified medical professionals in appropriately identifying assessing diagnosing or managing medical conditions. This system is only intended to be used in a research setting after conducting the appropriate ethics reviews.


