using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MusicPlayer : MonoBehaviour
{
    /// <summary>
    /// Self instance
    /// </summary>
    public static MusicPlayer instance;

    /// <summary>
    /// Start
    /// </summary>
    public void Start()
    {
        instance = this;
    }

    /// <summary>
    /// Get current music position between 0 and 1
    /// </summary>
    /// <returns></returns>
    public float GetMusicPosition()
    {
        var source = GetComponent<AudioSource>();
        return Mathf.Clamp(source.time / source.clip.length, 0f, 1f);
    }

    /// <summary>
    /// Start play
    /// </summary>
    public void StartPlay()
    {
        var source = GetComponent<AudioSource>();
        source.Play();
    }

    /// <summary>
    /// Stop play
    /// </summary>
    public void StopPlay()
    {
        var source = GetComponent<AudioSource>();
        source.Stop();
    }

    /// <summary>
    /// Set music position by offset between 0 and 1
    /// </summary>
    /// <param name="offset"></param>
    public void SetMusicPositionByOffset(float offset)
    {
        var source = GetComponent<AudioSource>();
        source.time = (source.clip.length - 0.1f) * offset;
    }

    /// <summary>
    /// Check if music is playing
    /// </summary>
    /// <returns></returns>
    public bool IsPlaying()
    {
        var source = GetComponent<AudioSource>();
        if (source && source.isPlaying)
        {
            return true;
        }
        return false;
    }
}
