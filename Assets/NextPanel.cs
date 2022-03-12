using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextPanel : MonoBehaviour
{
    Button next; Text tig;
    // Start is called before the first frame update
    void Awake()
    {
        next = transform.GetComponentInChildren<Button>();
        next.onClick.AddListener(OnNextClick);
        tig = transform.GetComponentInChildren<Text>();
    }


    public void SetTip(string s= "答对了\n进入下一轮")
    {
        tig.text = s;
    }

    private void OnNextClick()
    {
        gameObject.SetActive(false);
        GameController.Instance.NextClick();
    }

   
}
