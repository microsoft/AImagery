// Copyright (c) Microsoft Corporation.
// Licensed under the Microsoft Research License Terms.

using UnityEngine;
using TMPro;
using UnityEngine.UI;

//This script obtains the HR from the MionixQG mouse and changes the text and plays a sound for every heart beat.

public class HeartBeatSound : MonoBehaviour {

    //Uncomment if you want to use the MionixQG mouse to get the HR.
    //private Libraries.MionixQG.Monitor Mionix;

    public AudioClip HeartBeatClip;

    private AudioSource source;
    private float soundOffset;
    private float accumulator;

    private string currentStatus, textHR = "HR";
    public Text hrValueText;
    public bool fakeHR = false;
    public bool sensorDetected = false;

    public int fakeHRValue = 40;
    static public float roundHR;

    void Start ()
    {
        source = GetComponent<AudioSource>();
        //Uncomment these lines if you want to use the MionixQG mouse to get the HR.

        /*Mionix = Libraries.MionixQG.Monitor.Instance;
        Mionix.OnBioMetrics += Mionix_OnBioMetrics;
        Mionix.Connect();*/
    }

    //Uncomment these lines if you want to use the MionixQG mouse to get the HR.
    /*void Mionix_OnBioMetrics(object sender, Libraries.MionixQG.BioMetricsEventArgs e)
    {
        var bioMetrics = e.BioMetrics;
        currentStatus = bioMetrics.heartRateState;
        soundOffset = 1 / (bioMetrics.heartRate / 60f);
        roundHR = Mathf.Round(bioMetrics.heartRate);
        textHR = "" + roundHR;
    }*/

    void Update ()
    {
        // if MioniX is not connected, and the user turns on the toggle, then simulate the HR
        if (currentStatus == "active") {
            sensorDetected = true;
            fakeHR = false;

        }else{
            
            sensorDetected = false;

            if (fakeHR == true){
                //By default the HR is 40.
                roundHR = fakeHRValue;
            }
        
        }
        
        textHR = "" + roundHR;
        soundOffset = 1 / (roundHR / 60f);

        accumulator += Time.deltaTime;

        hrValueText.text = textHR;

        if (accumulator >= soundOffset)
        {
            accumulator = 0;
            source.PlayOneShot(HeartBeatClip);
        }
       
	}

    void OnDestroy()
    {
        //Uncomment if you want to use the MionixQG mouse to get the HR.
        //Mionix.Disconnect();
    }
}
