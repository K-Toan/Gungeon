using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    public GameObject QuickStartMenu;
    [SerializeField] private TextMeshProUGUI characterText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI modeText;
    //[SerializeField] private Image ImgChar;

    private void Awake()
    {
        QuickStartMenu.SetActive(false);
        InitializeUIElements();
    }

    private void Start()
    {
        UpdateEndGameUI();
    }

    private void OnEnable()
    {
        QuickStartMenu.SetActive(true);
    }

    public void ExitButton_OnClick()
    {
        Debug.Log("Exit to menu");
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuickStartButton_OnClick()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void InitializeUIElements()
    {   
        
        characterText = GameObject.Find("TextCharUI").GetComponent<TextMeshProUGUI>();
        timeText = GameObject.Find("TextTimeUI").GetComponent<TextMeshProUGUI>();
        killText = GameObject.Find("TextKillUI").GetComponent<TextMeshProUGUI>();
        modeText = GameObject.Find("TextModeUI").GetComponent<TextMeshProUGUI>();
        if (characterText == null) Debug.LogError("Character Text not found!");
        if (timeText == null) Debug.LogError("Time Text not found!");
        if (killText == null) Debug.LogError("Kill Text not found!");
        if (modeText == null) Debug.LogError("Mode Text not found!");
    }

    private void UpdateEndGameUI()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            characterText.text = $"{player.Name}";
        }
        float totalTime = GameManager.Instance.TotalPlayTime;
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);
        modeText.text = $"Survival";
        timeText.text = $"{minutes:D2}:{seconds:D2}";
        killText.text = $"{GameManager.Instance.KillCount}";
    }
}
