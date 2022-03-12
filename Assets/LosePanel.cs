using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosePanel : MonoBehaviour
{
    Button back;
    // Start is called before the first frame update
    void Awake()
    {
        back = transform.GetComponentInChildren<Button>();
        back.onClick.AddListener(OnNextClick);
    }

    private void OnNextClick()
    {
        gameObject.SetActive(false);
        GameController.Instance.SetStartPanel(true);
        GameController.Instance.SetPlayPanel(false);
    }
}
