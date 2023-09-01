using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreen : UIScreen
{
    [SerializeField] Slider m_musicSlider;
    [SerializeField] Slider m_soundSlider;

    public void OnHomeButtonPressed()
    {

    }
    public void OnReplayButtonPressed()
    {

    }
    public void OnMusicButtonPressed()
    {
        m_musicSlider.value = (m_musicSlider.value == 0) ? 1 : 0;
    }
    public void OnSoundButtonPressed()
    {
        m_soundSlider.value = (m_soundSlider.value == 0) ? 1 : 0;
    }
    public void OnMusicSliderValueChange()
    {

    }
    public void OnSoundSliderValueChange()
    {

    }
}
