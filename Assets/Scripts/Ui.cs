using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Ui : MonoBehaviour
{
    public static Ui instance;
    public Slider slider;

    /// <summary>
    /// On slider value has changed by user
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSliderValueChanged(BaseEventData eventData)
    {
        MusicPlayer.instance.SetMusicPositionByOffset(slider.value);
    }

    /// <summary>
    /// On click start stop button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnClickStartStop(BaseEventData eventData)
    {
        if (MusicPlayer.instance.IsPlaying())
        {
            MusicPlayer.instance.StopPlay();
        }
        else
        {
            MusicPlayer.instance.StartPlay();
        }
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        instance = this;
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (MusicPlayer.instance)
        {
            // update slider position by music position
            slider.value = MusicPlayer.instance.GetMusicPosition();
        }
    }
}
