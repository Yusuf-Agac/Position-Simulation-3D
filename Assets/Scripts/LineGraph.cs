using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGraph : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public List<float> dataPoints;
    public Vector3 lineGraphOffset;

    private void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    public void ChangeGraph()
    {
        float lineAmount = dataPoints.Count;
        lineRenderer.positionCount = (int)lineAmount;
        for (int i = 0; i < lineAmount; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(lineGraphOffset.x + (i * 150 / lineAmount), lineGraphOffset.y + dataPoints[i], lineGraphOffset.z));
        }
    }
}