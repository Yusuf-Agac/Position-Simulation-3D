using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    Generator generator;
    public TMPro.TMP_InputField TField;
    
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
        isRunning = true;
        generator.HideGraph();
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
