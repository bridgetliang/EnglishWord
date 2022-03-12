using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class PlayPanel : MonoBehaviour
{
    Text combineText;
    GridLayoutGroup big;
    Transform small;
    Button tipButton;
    Button giveUpButton;
    Image tipTextrue;
    Text tipText;
    Button back;
    AudioSource audioBGM;

    List<BigItem> bigItemsList = new List<BigItem>();
    List<BigItem> bigItemsListConbine = new List<BigItem>();
    GameObject bigItem;
    GameObject smallItem;
    RectTransform MouseFollow;
    private Vector3 pos;
    bool isPlay;
    public WordData curWorld;
    // Start is called before the first frame update
    void Awake()
    {
        combineText = transform.Find("combineText").GetComponent<Text>();
        big = transform.Find("big").GetComponent<GridLayoutGroup>(); ;
        small = transform.Find("small");
        tipButton = transform.Find("tipButton").GetComponent<Button>();
        giveUpButton = transform.Find("giveUpButton").GetComponent<Button>();
        tipTextrue = transform.Find("tipTextrue").GetComponent<Image>();
        tipText = transform.Find("tipTextrue/Text").GetComponent<Text>();
        back = transform.Find("back").GetComponent<Button>();
        audioBGM = GetComponent<AudioSource>();
        tipTextrue.gameObject.SetActive(false);
        bigItem = Resources.Load<GameObject>("BigItem");
        smallItem = Resources.Load<GameObject>("SmallItem");


        tipButton.onClick.AddListener(ShowTip);
        giveUpButton.onClick.AddListener(GiveUp);
        back.onClick.AddListener(OnBackClick);
    }

    private void OnBackClick()
    {
        GameController.Instance.PlayAudioClick();
        gameObject.SetActive(false);
        GameController.Instance.SetStartPanel(true);
    }

    private void GiveUp()
    {
        isPlay = false;
        GameController.Instance.PlayAudioClick();
        combineText.text = curWorld.english;
        StartCoroutine(PlayTrueAnim(false, $"正确答案\n{curWorld.english}\n{curWorld.mean}"));

    }

    private void ShowTip()
    {
        GameController.Instance.PlayAudioClick();
        tipTextrue.sprite = Resources.Load<Sprite>($"word/{SpriteName(curWorld)}");
        tipText.text = curWorld.mean;
        tipTextrue.gameObject.SetActive(true);
    }

    string SpriteName(WordData wordData)
    {
        string[] tmp = wordData.mean.Split('.');
        Debug.Log($"{tmp[1]}-{wordData.english}");
        return $"{tmp[1]}-{wordData.english}";
    }

    private void OnEnable()
    {
        audioBGM.Play();

    }

    bool isNeedRelease = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && isPlay && !isNeedRelease)
        {

            if (MouseFollow == null)
            {
                MouseFollow = Instantiate(Resources.Load<GameObject>("MouseFollow"), transform, false).GetComponent<RectTransform>();

            }
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(MouseFollow, Input.mousePosition, Camera.main, out pos))
            {

                MouseFollow.transform.position = pos;
                if (!MouseFollow.gameObject.activeSelf)
                {
                    MouseFollow.gameObject.SetActive(true);
                }
            }
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Count > 0)
            {
                if (results[0].gameObject.name.Contains("BigItem"))
                {
                    BigItem bigItem = results[0].gameObject.GetComponent<BigItem>();
                    bigItem.gameObject.SetActive(false);
                    combineText.text += bigItem.text.text;
                    bigItemsList.Remove(bigItem);
                    bigItemsListConbine.Add(bigItem);
                    GameController.Instance.PlayAudioClick();

                }
                if (bigItemsList.Count <= 0)
                {
                    FinishCombine();
                    isPlay = false;
                    Debug.Log(isPlay);
                    isNeedRelease = true;
                }
            }


        }
        else
        {
            if (MouseFollow != null && MouseFollow.gameObject.activeSelf)
            {
                MouseFollow.gameObject.SetActive(false);
            }
        }
        if (Input.GetMouseButtonUp(0) && isNeedRelease)
        {
            isNeedRelease = false;
        }



    }

    private void FinishCombine()
    {
        string tmp = string.Empty;
        for (int i = 0; i < bigItemsListConbine.Count; i++)
        {
            tmp += bigItemsListConbine[i].text.text;
        }
        if (curWorld.english == tmp)
        {
            StartCoroutine(PlayTrueAnim(true));

        }
        else
        {
            GameController.Instance.PlayAudioLoseClip();
            ReStart();
        }
    }

    IEnumerator PlayTrueAnim(bool active, string s = "答对了\n进入下一轮")
    {
        giveUpButton.interactable = false;

        if (active)
        {
            for (int i = 0; i < bigItemsListConbine.Count; i++)
            {
                RectTransform rt = bigItemsListConbine[i].GetComponent<RectTransform>();
                rt.gameObject.SetActive(true);
                rt.DOSizeDelta(new Vector2(80, 80), 1);
                rt.DOMove(small.GetChild(i).transform.position, 1);
            }
            yield return new WaitForSeconds(1);

        }
        else
        {
            var chars = curWorld.english.ToCharArray();
            foreach (var item in bigItemsList)
            {
                bigItemsListConbine.Add(item);
            }
            for (int i = 0; i < chars.Length; i++)
            {
                for (int j = 0; j < bigItemsListConbine.Count; j++)
                {
                    if (bigItemsListConbine[j].text.text == chars[i].ToString())
                    {
                        RectTransform rt = bigItemsListConbine[j].GetComponent<RectTransform>();
                        rt.gameObject.SetActive(true);
                        rt.DOScale(new Vector3(0.47f, 0.47f, 0.47f), 1);
                        rt.DOMove(small.GetChild(i).transform.position, 1);
                        bigItemsListConbine.Remove(bigItemsListConbine[j]);
                        break;
                    }

                }
            }
            ShowTip();
            yield return new WaitForSeconds(3);
        }


        giveUpButton.interactable = true;
        GameController.Instance.FinishCombine(active, s);
    }

    void ReStart()
    {
        CreatBigItems(curWorld);
    }

    public void CreatBigItems(WordData curWorld)
    {
        Clear();
        this.curWorld = curWorld;
        big.enabled = true;
        char[] chars=  curWorld.english.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char temp = chars[i];
            int randomIndex = UnityEngine.Random.Range(0, chars.Length);
            chars[i] = chars[randomIndex];
            chars[randomIndex] = temp;
        }
        for (int i = 0; i < chars.Length; i++)
        {
           
            if (i < big.transform.childCount)
            {
                GameObject go = big.transform.GetChild(i).gameObject;
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
                bigItemsList.Add(go.GetComponent<BigItem>().SetData(SetColor(i, curWorld.english.Length), chars[i].ToString()));
            }
            else
            {
                bigItemsList.Add(Instantiate(bigItem, big.transform, false).GetComponent<BigItem>().SetData(SetColor(i, curWorld.english.Length), chars[i].ToString()));

            }

            if (i < small.childCount)
            {
                small.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                Instantiate(smallItem, small, false);
            }
        }

      
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {

        yield return new WaitForEndOfFrame();
        big.enabled = false;
        isPlay = true;
        Debug.Log(isPlay);
    }


    void Clear()
    {
        //Debug.Log(big);
        for (int i = big.transform.childCount - 1; i >= 0; i--)
        {
            big.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = small.childCount - 1; i >= 0; i--)
        {
            small.GetChild(i).gameObject.SetActive(false);
        }
        bigItemsList.Clear();
        bigItemsListConbine.Clear();
        tipTextrue.gameObject.SetActive(false);
        combineText.text = "";


    }
    [Range(0, 1)]
    public float s = 1;
    [Range(0, 1)]
    public float v = 1;
    Color SetColor(int index, float count)
    {
        return Color.HSVToRGB((index + 1) / (count + 2), s, v);
    }
}
