using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Button btnOpen;
    [SerializeField] private Text txtAlive;
    [SerializeField] private Button btnQuit;
    [SerializeField] private GameObject Setting;
    [SerializeField] private AudioSource soundGame;
    [SerializeField] private AudioSource soundSword;
    private int switchState = 1;
    public GameObject switchBtnSound;
    // Start is called before the first frame update
    private void Awake()
    {
        if (PlayerPrefs.GetInt("sound") != 0)
        {
            switchState = PlayerPrefs.GetInt("sound");
        }
        switchBtnSound.transform.DOLocalMoveX(switchState * switchBtnSound.transform.localPosition.x, 0f);
        PlaySoundGame();
        InvokeRepeating("updateAlive", 0f, 1f);
        btnOpen.onClick.AddListener(() => {
            StartCoroutine(OpenSetting());
        });
        btnQuit.onClick.AddListener(() => {
            Application.Quit();
        });
    }

    public void PlaySoundGame()
    {
        if (switchState == 1)
        {
            soundGame.enabled = true;
            soundSword.enabled = true;
        }
        else
        {
            soundGame.enabled = false;
            soundSword.enabled = false;
        }
    }
    private void Update()
    {
        if (GameEvents.Instance.listAlive.Count == 0)
        {
            GameEvents.Instance.showWinPanel(1);
        }
    }
    public void updateAlive()
    {
        txtAlive.text = "Alive: " + GameEvents.Instance.listAlive.Count;
    }
    public IEnumerator OpenSetting()
    {
        Setting.transform.DOScale(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
    }
    public void CloseSetting()
    {
        Time.timeScale = 1;
        Setting.transform.DOScale(0, 0.5f);
    }

    public void OnSwitchButtonSoundClicked()
    {
         Tween myTween=switchBtnSound.transform.DOLocalMoveX(-switchBtnSound.transform.localPosition.x, 0.2f);
        myTween.SetUpdate(true);
        switchState = Math.Sign(-switchBtnSound.transform.localPosition.x);
        PlayerPrefs.SetInt("sound", switchState);
        PlaySoundGame();
    }
}
