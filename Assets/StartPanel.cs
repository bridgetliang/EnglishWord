using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    Button five;
    Button ten;
    Button fifteen;
    Button twenty;
    Button quit;
    Image Rank;
    Text RankText;

    AudioSource audioBGM;

    // Start is called before the first frame update
    void Awake()
    {
        audioBGM = GetComponent<AudioSource>();
        five = transform.Find("five").GetComponent<Button>();
        ten = transform.Find("ten").GetComponent<Button>();
        fifteen = transform.Find("fifteen").GetComponent<Button>();
        twenty = transform.Find("twenty").GetComponent<Button>();
        quit = transform.Find("quit").GetComponent<Button>();
        Rank = transform.Find("Rank").GetComponent<Image>();
        RankText = transform.Find("Rank/Text").GetComponent<Text>();
        GameController.Instance.RefreshRank += RefreshRank;

        five.onClick.AddListener(OnFiveClick);
        ten.onClick.AddListener(OnTenClick);
        fifteen.onClick.AddListener(OnFifteenClick);
        twenty.onClick.AddListener(OnTwentyClick);
        quit.onClick.AddListener(OnQuitClick);
    }

    private void OnEnable()
    {
        audioBGM.Play();
    }

    private void OnFiveClick()
    {
        OpenPlayPanel(5);
    }

    private void OnTenClick()
    {
        OpenPlayPanel(10);
    }

    private void OnFifteenClick()
    {
        OpenPlayPanel(15);
    }

    private void OnTwentyClick()
    {
        OpenPlayPanel(20);

    }

    void OpenPlayPanel(int count)
    {
        GameController.Instance.PlayAudioClick();
        gameObject.SetActive(false);
        GameController.Instance.SetPlayPanel(true, count);
    }

    private void OnQuitClick()
    {

        Application.Quit();
    }

    private void RefreshRank(Sprite arg1, string arg2)
    {
        Rank.sprite = arg1;
        RankText.text = arg2;
        Debug.Log("RefreshRank");
    }
}
