using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button bonusButton1;
    [SerializeField] private Button bonusButton2;
    [SerializeField] private Button bonusButton3;
    [SerializeField] private TMP_Text bonusText1;
    [SerializeField] private TMP_Text bonusText2;
    [SerializeField] private TMP_Text bonusText3;

    private LevelManager.LevelBonus[] bonusOptions = new LevelManager.LevelBonus[3];
    private static LevelUpUIManager instance;

    public static LevelUpUIManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 确保按钮都有正确的点击事件
        if (bonusButton1 != null)
        {
            bonusButton1.onClick.RemoveAllListeners();
            bonusButton1.onClick.AddListener(() => SelectBonus(0));
        }
        if (bonusButton2 != null)
        {
            bonusButton2.onClick.RemoveAllListeners();
            bonusButton2.onClick.AddListener(() => SelectBonus(1));
        }
        if (bonusButton3 != null)
        {
            bonusButton3.onClick.RemoveAllListeners();
            bonusButton3.onClick.AddListener(() => SelectBonus(2));
        }

        Debug.Log("LevelUpUIManager initialized!");
    }

    public void SetBonusOptions(System.Collections.Generic.List<LevelManager.LevelBonus> bonuses)
    {
        Debug.Log("Setting bonus options: " + bonuses.Count);

        if (levelText != null)
        {
            levelText.text = "Level Up!\nLevel " + LevelManager.Instance.CurrentLevel;
        }

        // 重置所有选项
        for (int i = 0; i < 3; i++)
        {
            bonusOptions[i] = null;
        }

        // 分配新选项
        for (int i = 0; i < bonuses.Count && i < 3; i++)
        {
            bonusOptions[i] = bonuses[i];
            if (i == 0 && bonusText1 != null)
            {
                bonusText1.text = bonuses[i].description;
                if (bonusButton1 != null) bonusButton1.gameObject.SetActive(true);
            }
            if (i == 1 && bonusText2 != null)
            {
                bonusText2.text = bonuses[i].description;
                if (bonusButton2 != null) bonusButton2.gameObject.SetActive(true);
            }
            if (i == 2 && bonusText3 != null)
            {
                bonusText3.text = bonuses[i].description;
                if (bonusButton3 != null) bonusButton3.gameObject.SetActive(true);
            }
        }

        Debug.Log("Bonus options set!");
    }

    private void SelectBonus(int index)
    {
        Debug.Log("Selected bonus index: " + index);

        if (index >= 0 && index < bonusOptions.Length && bonusOptions[index] != null)
        {
            LevelManager.Instance.ApplyBonus(bonusOptions[index]);
        }
        else
        {
            Debug.LogWarning("Invalid bonus selected or bonus is null!");
            // 如果选择无效，自动选择第一个
            if (bonusOptions[0] != null)
            {
                LevelManager.Instance.ApplyBonus(bonusOptions[0]);
            }
            else
            {
                Time.timeScale = 1f;
                LevelManager.Instance.HideLevelUpPanel();
            }
        }
    }
}
