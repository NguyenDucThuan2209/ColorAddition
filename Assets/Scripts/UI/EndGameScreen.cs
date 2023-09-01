using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreen : UIScreen
{
    [SerializeField] Text m_scoreText;
    [SerializeField] Text m_highScoreText;
    [SerializeField] Slider m_musicSlider;
    [SerializeField] Slider m_soundSlider;

    public override void ShowScreen()
    {
        base.ShowScreen();

        m_musicSlider.value = (SoundManager.Instance.MusicState) ? 0 : 1;
        m_soundSlider.value = (SoundManager.Instance.SoundState) ? 0 : 1;
    }

    public void SetScoreText(int score, int highScore)
    {
        m_scoreText.text = "SCORE  " + score;
        m_highScoreText.text = "BEST  " + highScore;
    }

    public void OnHomeButtonPressed()
    {
        SoundManager.Instance.PlaySound("Click");
        MenuManager.Instance.BackToHome();
    }
    public void OnReplayButtonPressed()
    {
        SoundManager.Instance.PlaySound("Click");
        MenuManager.Instance.ReplayGame();
    }
    public void OnMusicButtonPressed()
    {
        m_musicSlider.value = (m_musicSlider.value == 0) ? 1 : 0;

        SoundManager.Instance.PlaySound("Click");
        SoundManager.Instance.SetMusicState(m_musicSlider.value == 0);
    }
    public void OnSoundButtonPressed()
    {
        m_soundSlider.value = (m_soundSlider.value == 0) ? 1 : 0;

        SoundManager.Instance.PlaySound("Click");
        SoundManager.Instance.SetSoundState(m_soundSlider.value == 0);
    }
    public void OnMusicSliderValueChange()
    {

    }
    public void OnSoundSliderValueChange()
    {

    }
}
