using UnityEngine;
using System;
using System.IO;
using System.Collections;
using TMPro;

public class HelpDesc : MonoBehaviour
{

    StreamReader file;
    private int currentLine = 0;

    [SerializeField] private string[] text;
    [SerializeField] private TextMeshProUGUI target;

    public void changeText(int i)
    {
        Debug.Log("CLICK!");

        currentLine = Math.Max(0, currentLine + i);

        Debug.Log("CLICK!");

        if (currentLine > 5) GameManager.Instance.LoadScene("Title Screen");

        target.SetText(text[currentLine]);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        changeText(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
