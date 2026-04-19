using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int baseExp = 100;
    [SerializeField] private GameObject levelUpPanel;

    private int currentLevel = 1;
    private int currentExp = 0;
    private int expToNextLevel;
    private bool shotgunSelected = false;
    private static LevelManager instance;

    [System.Serializable]
    public class LevelBonus
    {
        public string bonusName;
        public string description;
        public float value;
        public BonusType type;
    }

    public enum BonusType
    {
        Damage, // 增伤
        Health, // 血量
        Speed,  // 移速
        FireRate, // 射击速度
        Shotgun, // 散弹
        IceBullet // 寒冰子弹
    }

    private List<LevelBonus> availableBonuses = new List<LevelBonus>()
    {
        new LevelBonus { bonusName = "Damage Boost", description = "Damage +15%", value = 1.15f, type = BonusType.Damage },
        new LevelBonus { bonusName = "Health Boost", description = "Health +15%", value = 1.15f, type = BonusType.Health },
        new LevelBonus { bonusName = "Speed Boost", description = "Speed +15%", value = 1.15f, type = BonusType.Speed },
        new LevelBonus { bonusName = "Fire Rate Boost", description = "Fire Rate +15%", value = 1.15f, type = BonusType.FireRate },
        new LevelBonus { bonusName = "Ice Bullet", description = "5% chance to freeze enemies", value = 0.05f, type = BonusType.IceBullet },
        new LevelBonus { bonusName = "Shotgun", description = "Shoot 3 bullets at once", value = 1f, type = BonusType.Shotgun }
    };

    public static LevelManager Instance
    {
        get { return instance; }
    }

    public int CurrentLevel { get { return currentLevel; } }
    public int CurrentExp { get { return currentExp; } }
    public int ExpToNextLevel { get { return expToNextLevel; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CalculateExpToNextLevel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        CheckLevelUp();
    }

    private void CalculateExpToNextLevel()
    {
        expToNextLevel = baseExp + (currentLevel * 50); // 简化公式，1级100，2级200，3级250...
    }

    private void CheckLevelUp()
    {
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        CalculateExpToNextLevel();
        ShowLevelUpPanel();
    }

    private void ShowLevelUpPanel()
    {
        if (levelUpPanel != null)
        {
            Time.timeScale = 0f;
            levelUpPanel.SetActive(true);
            GenerateBonusOptions();
        }
        else
        {
            Debug.LogWarning("Level up panel not assigned, skipping level up UI");
            // 自动应用一个随机加成
            if (availableBonuses.Count > 0)
            {
                int randomIndex = Random.Range(0, availableBonuses.Count);
                ApplyBonus(availableBonuses[randomIndex]);
            }
            Time.timeScale = 1f;
        }
    }

    private void GenerateBonusOptions()
    {
        List<LevelBonus> availableOptions = new List<LevelBonus>(availableBonuses);
        
        // 根据等级解锁新天赋
        if (currentLevel < 5)
        {
            // 5级以下，移除寒冰子弹和散弹
            availableOptions.RemoveAll(b => b.type == BonusType.IceBullet || b.type == BonusType.Shotgun);
        }
        else if (currentLevel < 10)
        {
            // 5-9级，只解锁寒冰子弹
            availableOptions.RemoveAll(b => b.type == BonusType.Shotgun);
        }
        else
        {
            // 10级以上，如果散弹已选择则移除散弹选项
            if (shotgunSelected)
            {
                availableOptions.RemoveAll(b => b.type == BonusType.Shotgun);
            }
        }
        
        List<LevelBonus> selectedBonuses = new List<LevelBonus>();
        List<LevelBonus> tempBonuses = new List<LevelBonus>(availableOptions);
        
        for (int i = 0; i < 3; i++)
        {
            if (tempBonuses.Count > 0)
            {
                int index = Random.Range(0, tempBonuses.Count);
                selectedBonuses.Add(tempBonuses[index]);
                tempBonuses.RemoveAt(index);
            }
        }

        if (selectedBonuses.Count > 0 && LevelUpUIManager.Instance != null)
        {
            LevelUpUIManager.Instance.SetBonusOptions(selectedBonuses);
        }
        else
        {
            Debug.LogWarning("LevelUpUIManager not initialized, skipping bonus options");
            Time.timeScale = 1f;
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(false);
            }
        }
    }

    public void ApplyBonus(LevelBonus bonus)
    {
        switch (bonus.type)
        {
            case BonusType.Damage:
                if (PlayerShooter.Instance != null)
                {
                    PlayerShooter.Instance.AddDamageBonus(bonus.value);
                }
                break;
            case BonusType.Health:
                if (PlayerHealth.Instance != null)
                {
                    PlayerHealth.Instance.AddHealthBonus(bonus.value);
                }
                break;
            case BonusType.Speed:
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.AddSpeedBonus(bonus.value);
                }
                break;
            case BonusType.FireRate:
                if (PlayerShooter.Instance != null)
                {
                    PlayerShooter.Instance.AddFireRateBonus(bonus.value);
                }
                break;
            case BonusType.Shotgun:
                if (PlayerShooter.Instance != null)
                {
                    PlayerShooter.Instance.EnableShotgun();
                    shotgunSelected = true;
                }
                break;
            case BonusType.IceBullet:
                if (PlayerShooter.Instance != null)
                {
                    PlayerShooter.Instance.AddIceBulletChance(bonus.value);
                }
                break;
        }

        Time.timeScale = 1f;
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
    }

    public void ResetLevel()
    {
        currentLevel = 1;
        currentExp = 0;
        CalculateExpToNextLevel();
    }

    public void HideLevelUpPanel()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }
}
