using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    public List<GameObject> cubes = new List<GameObject>();
    public TMPro.TMP_InputField NField;
    public TMPro.TMP_Text AverageText;
    public Toggle CollisionToggle;
    private Scaler scaler;
    public LineGraph lineGraph;
    private int distanceCount;
    private float distanceSum;
    public float CubeScale = 0.1f;
    public int numberOfCollisionTests;
    public int numberOfValidCollisions;
    public int numberOfMoves;
    public LayerMask collisionLayerMask;

    private record AverageDistance
    {
        public float Sum { get; set; }
        public float Count { get; set; }
        public float Average
        {
            get => Sum / Count;
        }
    }

    private float averageDistance = 0f;
    public float maxRandomSubstitution = 1f;
    private readonly List<float> averageDistances = new();

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
        distanceCount += cubes.Count * (cubes.Count - 1) / 2;
        distanceSum += sum;
        averageDistance = distanceSum / distanceCount;
        AverageText.text = $"Average Distance: {averageDistance:F2} Number of Collisions: {numberOfCollisionTests} Valid Collisions: {numberOfValidCollisions} Failed Moves: %{(numberOfMoves == 0 ? 0f : (float)numberOfValidCollisions / numberOfMoves) * 100:F2}";
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
                    Random.Range(-scaler.Scale.x / 2, scaler.Scale.x / 2),
                    Random.Range(-scaler.Scale.y / 2, scaler.Scale.y / 2),
                    Random.Range(-scaler.Scale.z / 2, scaler.Scale.z / 2));
                cube.transform.localScale = new Vector3(CubeScale, CubeScale, CubeScale);
                cube.transform.parent = transform;
                cube.layer = LayerMask.NameToLayer("Cubes");
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

    private Vector3 CalculateNewPosition(Vector3 oldPosition, BoxCollider self)
    {
        numberOfMoves++;

        var newPosition = new Vector3(
                    Random.Range(oldPosition.x - maxRandomSubstitution, oldPosition.x + maxRandomSubstitution),
                    Random.Range(oldPosition.y - maxRandomSubstitution, oldPosition.y + maxRandomSubstitution),
                    Random.Range(oldPosition.z - maxRandomSubstitution, oldPosition.z + maxRandomSubstitution));

        newPosition = ClampVector3(newPosition, -scaler.Scale / 2, scaler.Scale / 2);

        if (!CollisionToggle.isOn)
            return newPosition;
        //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = newPosition;
        //cube.transform.localScale = new Vector3(CubeScale, CubeScale, CubeScale);
        //cube.GetComponent<MeshRenderer>().material.color = Color.red;
        //cube.transform.parent = transform;
        var colliders = Physics.OverlapBox(newPosition, new Vector3(CubeScale / 2, CubeScale / 2, CubeScale / 2), Quaternion.identity, collisionLayerMask.value);
        colliders = colliders.Where(x => x != self).ToArray();

        if (colliders.Length == 0)
            return newPosition;

        numberOfCollisionTests++;

        var minDistance = colliders.Select(x => Vector3.Distance(newPosition, x.GetComponent<Transform>().position)).Min();
        if (System.Math.Pow(System.Math.E, -minDistance) >= Random.Range(0f, 1f))
        {
            return newPosition;
        }

        numberOfValidCollisions++;
        return oldPosition;
    }

    public void RandomSubstitution()
    {
        Transform cube = cubes[Random.Range(0, cubes.Count)].transform;
        var position = cube.position;

        position = CalculateNewPosition(position, cube.GetComponent<BoxCollider>());

        cube.position = position;
        UpdateAverageDistance();
        averageDistances.Add(averageDistance);
    }

    public void ShowGraph()
    {
        lineGraph.ShowGraph(averageDistances);
    }

    public void HideGraph()
    {
        lineGraph.HideGraph();
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
