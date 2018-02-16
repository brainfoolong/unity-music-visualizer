using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hold some spectrum data
/// </summary>
public class SpectrumData
{
    public int index;
    public float data;
    public float dataNormalized;

    /// <summary>
    /// Add data to the list and count up
    /// </summary>
    /// <param name="data"></param>
    public void AddData(float data)
    {
        this.data += data;
    }
}
