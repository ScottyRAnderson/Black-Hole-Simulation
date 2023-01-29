using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI counter;
    [SerializeField]
    private string prefix = "fps:";
    [SerializeField]
    private int numDP = 1;
    [SerializeField][Tooltip("Seconds")]
    private float updateInterval = 2f;

    private float fps;
    private string dpString;
    private float intervalTimer;

    public float FPS { get { return fps; } }

    private void OnValidate()
    {
        numDP = Mathf.Max(numDP, 0);
        updateInterval = Mathf.Max(updateInterval, 0);
    }

    private void Awake(){
        dpString = "F" + numDP.ToString();
    }

    private void Update()
    {
        fps = 1f / Time.unscaledDeltaTime;
        if (intervalTimer > 0f){
            intervalTimer -= Time.deltaTime;
        }
        else
        {
            counter.text = prefix + " " + fps.ToString(dpString);
            intervalTimer = updateInterval;
        }
    }
}