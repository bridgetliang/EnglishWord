using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigItem : MonoBehaviour
{
    Image image;
   public Text text;
    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        text=transform. GetComponentInChildren<Text>();
    }

    // Update is called once per frame
   public BigItem SetData(Color color ,string s)
    {
        image.color = color;
        text.text = s;
        return this;
      
    }
}
