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

    public void StartSimulation()
    {
        isRunning = true;
    }

    public bool NextFrame()
    {
        parseValue(TField.text, out var T);
        if (T <= 0)
        {
            return false;
        }
        generator.RandomSubstitution();
        TField.text = (T - 1).ToString();
        return true;
    }

    public void RunSimulation()
    {
        StartCoroutine(RunningCoroutine());
    }

    IEnumerator RunningCoroutine()
    {
        while (true)
        {
            if(!NextFrame()){break;}
            yield return new WaitForSeconds(1.0f);
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
