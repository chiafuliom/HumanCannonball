using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreLabel;

    public int Score { get; private set; } = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optionally DontDestroyOnLoad(gameObject);
        UpdateLabel();
    }

    public void AddPoint(int amount = 1)
    {
        Score += amount;
        UpdateLabel();
    }

    public void ResetScore()
    {
        Score = 0;
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (scoreLabel != null)
            scoreLabel.text = $"Score: {Score}";
    }
}

