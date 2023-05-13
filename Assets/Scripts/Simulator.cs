using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Simulator : MonoBehaviour
{
    Generator generator;
    public TMPro.TMP_InputField TField;
    public TMPro.TMP_Text PlayButtonText;
    
    public bool isRunning = false;
    public float maxRandomSubstitution = 10f;

    private void Start()
    {
        generator = GetComponent<Generator>();
    }
    
    public void NextFrame()
    {
        parseValue(TField.text, out var T);
        if (T <= 0)
        {
            SimulationFinished();
            return;
        }
        generator.RandomSubstitution();
        TField.text = (T - 1).ToString();
    }

    public void SimulationFinished()
    {
        isRunning = false;
        generator.ShowGraph();
    }

    public void RunSimulation()
    {
        if (isRunning)
        {
            isRunning = false;
            generator.ShowGraph();
            PlayButtonText.text = "Play";
        }
        else
        {
            isRunning = true;
            generator.HideGraph();
            PlayButtonText.text = "Pause";
        }
    }

    public void Update()
    {
        if (isRunning)
        {
            NextFrame();
        }
    }

    private void parseValue(string s, out float f, float defaultValue = 10f)
    {
        if (!float.TryParse(s, out f))
        {
            f = defaultValue;
        }
    }
}
