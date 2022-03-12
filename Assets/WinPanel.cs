using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    Button back;
    Image image;
    // Start is called before the first frame update
    void Awake()
    {
        back = transform.GetComponentInChildren<Button>();
        back.onClick.AddListener(OnNextClick);
        image= transform.Find("Image/Image").GetComponent<Image>();
    }

    private void OnNextClick()
    {
        gameObject.SetActive(false);
        GameController.Instance.SetStartPanel(true);
        GameController.Instance.SetPlayPanel(false);
    }

    public void SetImage(Sprite s)
    {
        if (image==null)
        {
            image = transform.Find("Image/Image").GetComponent<Image>();
        }
        image.sprite = s;
    }
}
