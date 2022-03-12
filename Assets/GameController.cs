using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum E_GameModel
{
    five,
    ten,
    fifteen,
    twenty,
}

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public const string FinishPlayerPrefs = "FinishPlayerPrefs";
    public Action<Sprite, string> RefreshRank;
    public TextAsset words;
    public List<Sprite> leves;
    StartPanel startPanel;
    WinPanel winPanel;
    LosePanel losePanel;
    PlayPanel playPanel;
    NextPanel nextPanel;
    private E_GameModel gameModel;
    List<WordData> wordDatasList = new List<WordData>();
    AudioSource audio;

   public List<WordData> finishList ;
    public bool isClear;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        words = Resources.Load<TextAsset>("txt/EnglishWords");
        audio = Camera.main.GetComponent<AudioSource>();

        Loadwords();

        startPanel = transform.Find("StartPanel").GetComponent<StartPanel>();
        winPanel = transform.Find("WinPanel").GetComponent<WinPanel>();
        losePanel = transform.Find("LosePanel").GetComponent<LosePanel>();
        playPanel = transform.Find("PlayPanel").GetComponent<PlayPanel>();
        nextPanel = transform.Find("NextPanel").GetComponent<NextPanel>();
        startPanel.gameObject.SetActive(true);
        playPanel.gameObject.SetActive(false);

        if (isClear)
        {
            PlayerPrefs.DeleteAll();
        }

        if (PlayerPrefs.HasKey(FinishPlayerPrefs))
        {
            finishList= JsonConvert.DeserializeObject<List<WordData>>(PlayerPrefs.GetString(FinishPlayerPrefs));
            Debug.Log(PlayerPrefs.GetString(FinishPlayerPrefs));
        }
        else
        {
            finishList = new List<WordData>();
        }
        foreach (var item in finishList)
        {
            Debug.Log("通过的you  " + item.english);
        }
        SetRank();
    }

    void Loadwords()
    {
        if (words != null)
        {
            //Debug.Log(words.text);
            string[] lines = words.text.Split('\n');
            foreach (var item in lines)
            {
                string tmp = item.Replace("  ", "-");
                string[] word = tmp.Split('-');
               // Debug.Log(item);
                wordDatasList.Add(new WordData(word[0], word[1]));
            }


        }
    }




    public void SetStartPanel(bool active)
    {
        startPanel.gameObject.SetActive(active);
    }
    public void SetWinPanel(bool active)
    {
        
        winPanel.gameObject.SetActive(active);
    }
    public void SetLosePanel(bool active)
    {
        losePanel.gameObject.SetActive(active);
    }

    public void SetNextPanel(bool active,string s = "答对了\n进入下一轮")
    {
        nextPanel.gameObject.SetActive(active);
        if (active)
        {
            nextPanel.SetTip(s);

        }
    }
    public void SetPlayPanel( bool active,int total=0)
    {
        playPanel.gameObject.SetActive(active);

        if (active)
        {
            curIndex=0;
            curTotal= total;
            failCount=0;
            playPanel.CreatBigItems(RangWord());
        }

    }

    AudioClip tipClip;
    public void PlayAudioClick()
    {
        if (tipClip == null)
        {
            tipClip = Resources.Load<AudioClip>("Audio/提示音效 (2)");
        }
        audio.PlayOneShot(tipClip);
    }


    AudioClip loseClip;
    public void PlayAudioLoseClip()
    {
        if (loseClip == null)
        {
            loseClip = Resources.Load<AudioClip>("Audio/错误音效");
        }
        audio.PlayOneShot(loseClip);
    }

    /// <summary>
    /// 下一轮
    /// </summary>
    public void NextClick()
    {
        PlayAudioClick();
        playPanel.CreatBigItems(RangWord());
    }

    

    int curIndex;
    int curTotal;
    int failCount;

    public E_GameModel GameModel { get => gameModel; set => gameModel = value; }

    public void FinishCombine(bool active, string s = "答对了\n进入下一轮")
    {
        curIndex++;
        if (!active)
        {
            failCount++;
            PlayAudioLoseClip();
        }
        else
        {
            Save();
        }

        float errorRate =failCount / (float)curTotal;
        if (errorRate > 0.4f)
        {
            SetLosePanel(true);
        }
        else if (curIndex >= curTotal)
        {
            if (1-errorRate>=1f)
            {
                winPanel.SetImage(Resources.Load<Sprite>("皇冠"));
            }
            else if (1 - errorRate >= 0.9f)
            {
                winPanel.SetImage(Resources.Load<Sprite>("玩偶"));
            }
            else if (1 - errorRate >= 0.8f)
            {
                winPanel.SetImage(Resources.Load<Sprite>("冰淇淋"));
            }
            else if (1 - errorRate >= 0.7f)
            {
                winPanel.SetImage(Resources.Load<Sprite>("棒棒糖"));
            }
            else if (1 - errorRate >= 0.6f)
            {
                winPanel.SetImage(Resources.Load<Sprite>("小红花"));
            }
            SetWinPanel(true);
        }
        else
        {
            SetNextPanel(true,s);
        }
    }

    private void Save()
    {
        if (!finishList.Contains(playPanel.curWorld))
        {
            finishList.Add(playPanel.curWorld);

            SetRank();
        }
    }

    void SetRank()
    {
        float rate = finishList.Count / (float)wordDatasList.Count;
        Debug.Log("SetRank "+ rate);

        if (rate >= 1)
        {
            RefreshRank(Resources.Load<Sprite>("最强王者"), "最强王者");
        }
        else if (rate >= 0.9f)
        {
            RefreshRank(Resources.Load<Sprite>("王者"), "王者");
        }
        else if (rate >= 0.8f)
        {
            RefreshRank(Resources.Load<Sprite>("钻石"), "钻石");
        }
        else if (rate >= 0.7f)
        {
            RefreshRank(Resources.Load<Sprite>("铂金"), "铂金");
        }
        else
        {
            RefreshRank(Resources.Load<Sprite>("青铜"), "青铜");
        }

    }
    private void OnApplicationQuit()
    {
        
       // Debug.Log(JsonConvert.SerializeObject(finishList));
        PlayerPrefs.SetString(FinishPlayerPrefs, JsonConvert.SerializeObject(finishList));
    }
    public WordData RangWord()
    {
        return wordDatasList[UnityEngine.Random.Range(0, wordDatasList.Count)];
    }



    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        SetRank();
    //    }
    //}
}

public class WordData
{
    public string english;
    public string mean;
    public WordData(string english, string mean)
    {
        this.english = english;
        this.mean = mean;
    }

}
