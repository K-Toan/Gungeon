using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimerUI : MonoBehaviour
{
    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime = 120f;

    private void Start()
    {
        timerText = GameObject.Find("TextTime").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            timerText.color = Color.red;
            ShowLoseMessage();
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void ShowLoseMessage()
    {
        StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(2); 
        SceneManager.LoadScene("EndGameScene");
    }
}
