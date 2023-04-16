using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<GameObject> cubes = new List<GameObject>();
    public TMPro.TMP_InputField NField;
    public TMPro.TMP_Text AverageText;
    private Scaler scaler;
    public LineGraph lineGraph;

    private float averageDistance = 0f;
    
    
    private void Start()
    {
        scaler = GetComponent<Scaler>();
    }
    
    private void parseValue(string s, out float f, float defaultValue = 10f)
    {
        if (!float.TryParse(s, out f))
        {
            f = defaultValue;
        }
    }
    
    public void UpdateAverageDistance()
    {
        var sum = 0f;
        for (int i = 0; i < cubes.Count; i++)
        {
            for (int j = i + 1; j < cubes.Count; j++)
            {
                sum += Vector3.Distance(cubes[i].transform.position, cubes[j].transform.position);
            }
        }
        averageDistance = sum / (cubes.Count * (cubes.Count - 1) / 2);
        AverageText.text = $"Average Distance: {averageDistance:F2}";
    }
    
    public void ResetCubeAmount()
    {
        parseValue(NField.text, out var N, 0);
        var diff = N - cubes.Count;
        if (N - cubes.Count > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(
                    Random.Range(-scaler.Scale.x/2, scaler.Scale.x/2), 
                    Random.Range(-scaler.Scale.y/2, scaler.Scale.y/2), 
                    Random.Range(-scaler.Scale.z/2, scaler.Scale.z/2));
                cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                cube.transform.parent = transform;
                cubes.Add(cube);
            }
        }
        else
        {
            for (int i = 0; i < -diff; i++)
            {
                Destroy(cubes[^1]);
                cubes.RemoveAt(cubes.Count - 1);
            }
        }
        UpdateAverageDistance();
    }

    public void RandomSubstitution()
    {
        cubes[Random.Range(0, cubes.Count)].transform.position = new Vector3(
            Random.Range(-scaler.Scale.x/2, scaler.Scale.x/2), 
            Random.Range(-scaler.Scale.y/2, scaler.Scale.y/2), 
            Random.Range(-scaler.Scale.z/2, scaler.Scale.z/2));
        UpdateAverageDistance();
        lineGraph.dataPoints.Add(averageDistance);
        lineGraph.ChangeGraph();
    }
}
