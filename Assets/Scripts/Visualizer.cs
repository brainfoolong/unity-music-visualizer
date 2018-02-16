using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    public GameObject cubePrefab;

    Dictionary<int, GameObject> cubes = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> pcubes = new Dictionary<int, GameObject>();
    Dictionary<int, SpectrumData> spectrumDataList = new Dictionary<int, SpectrumData>();

    int[][] spectrumRanges;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        // define which aduio hz ranges we want to use
        spectrumRanges = new int[31][]
        {
            new int[2] {40, 60},
            new int[2] {61, 80},
            new int[2] {81, 100},
            new int[2] {101, 120},
            new int[2] {121, 140},
            new int[2] {141, 160},
            new int[2] {161, 180},
            new int[2] {181, 200},
            new int[2] {201, 220},
            new int[2] {221, 240},
            new int[2] {241, 260},
            new int[2] {261, 280},
            new int[2] {281, 300},
            new int[2] {301, 350},
            new int[2] {351, 400},
            new int[2] {401, 450},
            new int[2] {451, 500},
            new int[2] {501, 550},
            new int[2] {551, 600},
            new int[2] {601, 650},
            new int[2] {651, 700},
            new int[2] {701, 800},
            new int[2] {801, 900},
            new int[2] {901, 1000},
            new int[2] {1001, 1200},
            new int[2] {1201, 1400},
            new int[2] {1401, 1600},
            new int[2] {1601, 1800},
            new int[2] {1801, 2000},
            new int[2] {2001, 2250},
            new int[2] {2251, 2500}
        };
        // create data objects for each range
        for (int i = 0; i < spectrumRanges.Length; i++)
        {
            var group = new SpectrumData
            {
                index = i
            };
            spectrumDataList.Add(i, group);
        }
        // initialize the spectrum data list
        UpdateSpectrumDataList();
        var count = 0;

        // create the cubes for each data range
        foreach (var item in spectrumDataList)
        {
            var group = item.Value;
            var cube = Instantiate(cubePrefab);
            cube.transform.SetParent(transform);
            cube.name = group.index.ToString();
            var rotate = 360f / spectrumDataList.Count * count;
            cube.transform.position = Vector3.zero;
            cube.transform.Rotate(new Vector3(0, rotate, 0));
            cube.transform.Translate(new Vector3(6f, 0, 0), Space.Self);
            cube.SetActive(true);

            // set cube color
            var mat = cube.GetComponent<MeshRenderer>().material;
            var color = Color.HSVToRGB(1f / spectrumDataList.Count * count, 1, 1);
            mat.color = color;

            cubes.Add(group.index, cube);

            // create tiny physical cube
            var pos = cube.transform.position;
            pos.y = 20f;
            var physicsCube = Instantiate(cubePrefab);
            physicsCube.name = group.index + " Tiny";
            physicsCube.transform.SetParent(transform);
            physicsCube.transform.position = pos;
            physicsCube.transform.rotation = cube.transform.rotation;
            physicsCube.transform.localScale = Vector3.one * 0.5f;
            physicsCube.SetActive(true);

            // set rigibody, make it never sleep to dont slip through
            var rig = physicsCube.AddComponent<Rigidbody>();
            rig.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rig.sleepThreshold = 0;

            // set cube color
            mat = physicsCube.GetComponent<MeshRenderer>().material;
            mat.color = color;

            pcubes.Add(group.index, physicsCube);
            count++;
        }
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        UpdateSpectrumDataList();
        SpectrumData prevGroup = spectrumDataList[spectrumRanges.Length - 1];
        // update very cube based on the spectrum data for a specific range
        foreach (var item in spectrumDataList)
        {
            var group = item.Value;
            var norm = group.dataNormalized;
            var cube = cubes[group.index];        
            var scale = Vector3.one;
            scale.y = norm * 6f;
            // just lerp to smooth things out a bit
            scale = Vector3.Lerp(cube.transform.localScale, scale, 0.4f);
            cube.transform.localScale = scale;
            prevGroup = group;
        }
    }

    /// <summary>
    /// Update the spectrum data list for current music position
    /// </summary>
    public void UpdateSpectrumDataList()
    {
        // how much hz does each spectrum data sample represent
        var hzPerSample = (AudioSettings.outputSampleRate / 8192f);

        // get min/max data range to be able to normalize it later
        var min = Mathf.Infinity;
        var max = -Mathf.Infinity;
        var spectrumData = new float[8192];

        // get spectrum data from the audio listener with the best quality possible
        AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < spectrumRanges.Length - 1; i++)
        {
            var range = spectrumRanges[i];
            var group = spectrumDataList[i];
            group.data = 0;

            // get the minimum and maxium data index that as valid in this range chunk
            var minIndex = Mathf.FloorToInt(range[0] / hzPerSample);
            var maxIndex = Mathf.CeilToInt(range[1] / hzPerSample);

            // add data to the range group
            for (var si = minIndex; si <= maxIndex; si++)
            {
                group.AddData(spectrumData[si]);
            }

            // get min/max data for later normalize process
            if (min > group.data)
            {
                min = group.data;
            }
            if (max < group.data)
            {
                max = group.data;
            }
        }
        // normalize data between 0 and 1
        foreach (var item in spectrumDataList)
        {
            item.Value.dataNormalized = Mathf.Clamp((item.Value.data - min) / (max - min), 0.01f, 1f);
        }
    }
}
