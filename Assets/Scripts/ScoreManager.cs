using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int enemyKillScore = 100;
    [SerializeField] private int waveClearBonus = 500;

    private int currentScore = 0;
    private int currentWave = 1;
    private static ScoreManager instance;

    public static ScoreManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(currentScore);
        }
        
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddExp(amount);
        }
    }

    public void AddEnemyKillScore()
    {
        AddScore(enemyKillScore);
    }

    public void AddWaveClearBonus()
    {
        AddScore(waveClearBonus);
    }

    public void SetWave(int wave)
    {
        currentWave = wave;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateWave(currentWave);
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public void ResetScore()
    {
        currentScore = 0;
        currentWave = 1;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(currentScore);
            UIManager.Instance.UpdateWave(currentWave);
        }
    }
}
