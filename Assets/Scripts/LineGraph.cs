using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LineGraph : MonoBehaviour
{
    public Sprite circleSprite;
    private RectTransform graphContainer;
    
    private readonly List<GameObject> oldDotsAndLines = new List<GameObject>();

    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
    }
    
    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        return gameObject;
    }
    
    private void ClearGraph()
    {
        foreach (var item in oldDotsAndLines)
        {
            Destroy(item);
        }
        oldDotsAndLines.Clear();
    }

    public void ShowGraph(List<float> valueList)
    {
        transform.gameObject.SetActive(true);
        ClearGraph();
        var sizeDelta = graphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float yMaximum = valueList.Max() * 1.1f;
        float xSize = sizeDelta.x / (valueList.Count + 1);

        Vector2? lastCirclePosition = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            var position = new Vector2(xPosition, yPosition);
            if (lastCirclePosition != null)
            {
                oldDotsAndLines.Add(CreateDotConnection((Vector2)lastCirclePosition, position));
            }
            lastCirclePosition = position;
        }
    }

    public void HideGraph() => transform.gameObject.SetActive(false);
    
    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
}