using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource sfx;
    public AudioSource music;

    public Transform loadingIcon;

    float loadingTime = 0.75f;
    bool fadeUp = false;

    public RawImage fadeRI;
    public GameObject loadingUI;

    public Transform rotPivot;

    void Start()
    {

    }

    void Update()
    {
        loadingIcon.Rotate(Vector3.forward * Time.unscaledDeltaTime * -670f);
        if(loadingTime > 0) {
            loadingTime -= Time.unscaledDeltaTime;
            if(loadingTime <= 0) {
                fadeUp = true;
            }
        }

        rotPivot.Rotate(Vector3.up * Time.unscaledDeltaTime * 20);

        if (fadeUp) {
            fadeRI.color = Color.Lerp(fadeRI.color, Color.clear, Time.unscaledDeltaTime * 6f);
            if (fadeRI.color.a < 0.01f)
                fadeRI.color = Color.clear;
            loadingUI.SetActive(false);
        }
    }

    public void B_Play() {
        ButtonPress();

    }

    public void B_Quit() {
        ButtonPress();
        Application.Quit();
    }

    public void B_Options() {
        ButtonPress();

    }

    public void B_Credits() {
        ButtonPress();

    }

    public void B_Level_1() {
        ButtonPress();
        SceneManager.LoadScene(1);
    }

    public void B_Level_2() {
        ButtonPress();
        SceneManager.LoadScene(2);
    }

    public void B_MuteToggle() {
        ButtonPress();
        StaticGameParams.muted = !StaticGameParams.muted;
        music.mute = StaticGameParams.muted;
    }

    public void ButtonPress() {
        sfx.Play();
    }


}
