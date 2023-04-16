using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    public TMPro.TMP_InputField AField;
    public TMPro.TMP_InputField BField;
    public TMPro.TMP_InputField CField;
    
    private Generator generator;

    public Vector3 Scale { get; set; }

    public Transform plane;
    public Transform corner;

    private void Start()
    {
        Scale = new Vector3(9f, 9f, 9f);
        generator = GetComponent<Generator>();
        UpdateScale();
    }

    private void parseValue(string s, out float f, float defaultValue = 9f)
    {
        if (!float.TryParse(s, out f))
        {
            f = defaultValue;
        }
    }

    public void UpdateScale()
    {
        parseValue(AField.text, out var A);
        parseValue(BField.text, out var B);
        parseValue(CField.text, out var C);
        AField.text = A.ToString();
        BField.text = B.ToString();
        CField.text = C.ToString();
        plane.localPosition = new Vector3(0, -B/2, 0);
        plane.localScale = new Vector3(A, 0.001f, C);
        corner.localPosition = new Vector3(A/2 - A/80, 0, C/2 - C/80);
        corner.localScale = new Vector3(A/40, B/2, C/40);
        Scale = new Vector3(A, B, C);
        generator.ResetCubeAmount();
    }
}
