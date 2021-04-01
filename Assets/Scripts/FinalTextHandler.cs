using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalTextHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var text = gameObject.GetComponent<Text>();
        text.text = WinnerHolder.PlayerWined ? "You Win!" : "You Loose!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
