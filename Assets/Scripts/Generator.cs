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
    public float maxRandomSubstitution = 1f;
    private readonly List<float> averageDistances = new List<float>();


    private void Awake()
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
        Transform cube = cubes[Random.Range(0, cubes.Count)].transform;
        var position = cube.position;
        position = new Vector3(
            Random.Range(position.x - maxRandomSubstitution, position.x + maxRandomSubstitution), 
            Random.Range(position.y - maxRandomSubstitution, position.y + maxRandomSubstitution), 
            Random.Range(position.z - maxRandomSubstitution, position.z + maxRandomSubstitution));
        
        position = ClampVector3(position, -scaler.Scale / 2, scaler.Scale / 2);
        cube.position = position;
        UpdateAverageDistance();
        averageDistances.Add(averageDistance);
    }

    public void ShowGraph()
    {
        lineGraph.ShowGraph(averageDistances);
    }
    
    private Vector3 ClampVector3(Vector3 v, Vector3 min, Vector3 max)
    {
        return new Vector3(
            Wrap(v.x, min.x, max.x),
            Wrap(v.y, min.y, max.y),
            Wrap(v.z, min.z, max.z));
    }

    float Wrap(float value, float min, float max)
    {
        float range = max - min;
        if (range <= 0f)
            return value;
        float wrappedValue = ((value - min) % range + range) % range + min;
        return wrappedValue;
    }
}
