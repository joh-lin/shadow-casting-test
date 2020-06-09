using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testsorting : MonoBehaviour
{
    private void Start()
    {
        foreach (float value in Sort(new List<float> { -10f, -4.5f, 5f, -3f, 2.5f }))
        {
            Debug.Log(value);
        }
    }

    List<float> Sort(List<float> list)
    {
        if (list.Count < 2) { return list; }
        float pivot = list[Random.Range(0, list.Count - 1)];
        List<float> smaller = new List<float>();
        List<float> equal = new List<float>();
        List<float> bigger = new List<float>();

        for (int i = 0; i < list.Count; i++)
        {
            float value = list[i];
            if (value < pivot)
            {
                smaller.Add(value);
            }
            else if (value == pivot)
            {
                equal.Add(value);
            }
            else if (value > pivot)
            {
                bigger.Add(value);
            }
        }
        List<float> result = new List<float>();
        result.AddRange(Sort(smaller));
        result.AddRange(equal);
        result.AddRange(Sort(bigger));
        return result;
    }
}
