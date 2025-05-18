using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float elapsedTime = 0;

    private void Awake()
    {
        elapsedTime = 0;
    }

    void Start()
    {
        elapsedTime = 0;
    }
    
    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        Console.WriteLine(string.Format("{0:00}:{1:00}", minutes, seconds));


        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }  
}
