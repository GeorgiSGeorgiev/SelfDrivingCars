using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text TimeBox;
    private float TimeTracker;
    // Start is called before the first frame update
    void Start()
    {
        TimeTracker = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeTracker += Time.deltaTime;
        TimeBox.text = TimeTracker.ToString();
        //Debug.Log($"{ TimeTracker }");
    }

    private void Reset() {
        TimeBox.text = "0";
    }
}
