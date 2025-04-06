using UnityEngine;
using UnityEngine.UI; // For Slider
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToNextLevel;
    public int statPoints; 
    public int attack;
    public int hp;
    public HealthSystem healthSystem;
    public static bool isGameRestarted = false;

    public TextMeshProUGUI levelText;
    public Slider expSlider; 
    public TextMeshProUGUI statPointsText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI hpText;
    public GameObject skillPointPanel; 
    public static LevelSystem Instance;

    void Start()
    {
        LoadLevelData();
        UpdateUI();
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        SaveLevelData();
        UpdateUI();
    }

    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        expToNextLevel *= 2;
        statPoints += 10; // Nhận 10 điểm kỹ năng khi lên cấp

        skillPointPanel.SetActive(true); // Mở bảng cộng điểm kỹ năng khi lên cấp

        SaveLevelData();
    }

    public void IncreaseAttack()
    {
        if (statPoints > 0)
        {
            attack += 3;
            statPoints--;
            UpdateUI();
            SaveLevelData();
        }
    }

    public void IncreaseHP()
    {
        if (statPoints > 0)
        {
            hp += 10;
            statPoints--;
            healthSystem.UpdateMaxHP(hp); // Cập nhật HP cho thanh máu
            UpdateUI();
            SaveLevelData();
        }
    }

    public void UpdateUI()
    {
        levelText.text = "" + level;
        // Update the experience slider
        expSlider.value = (float)currentExp / expToNextLevel; // Cập nhật slider dựa trên EXP hiện tại
        statPointsText.text = "Stat Points: " + statPoints;
        attackText.text = "Attack: " + attack;
        hpText.text = "HP: " + hp;
    }

    public void SaveLevelData()
    {
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("CurrentExp", currentExp);
        PlayerPrefs.SetInt("ExpToNextLevel", expToNextLevel);
        PlayerPrefs.SetInt("StatPoints", statPoints);
        PlayerPrefs.SetInt("Attack", attack);
        PlayerPrefs.SetInt("HP", hp);
        PlayerPrefs.Save();
    }

    private void LoadLevelData()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            level = PlayerPrefs.GetInt("Level");
            currentExp = PlayerPrefs.GetInt("CurrentExp");
            expToNextLevel = PlayerPrefs.GetInt("ExpToNextLevel");
            statPoints = PlayerPrefs.GetInt("StatPoints");
            attack = PlayerPrefs.GetInt("Attack", 10);
            hp = PlayerPrefs.GetInt("HP", 100);
            Debug.Log("Loaded data: Level=" + level + ", EXP=" + currentExp);
        }
        else
        {
            Debug.Log("No save data found, resetting.");
            ResetLevelData();
        }
    }

    public void ResetLevelData()
    {
        level = 1;
        currentExp = 0;
        expToNextLevel = 500;
        statPoints = 0;
        attack = 10;
        hp = 100;
        SaveLevelData();
    }

    public void ResetGame()
    {
        ResetLevelData();
        UpdateUI();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll(); // Xóa toàn bộ dữ liệu khi thoát game
        PlayerPrefs.Save(); // Đảm bảo dữ liệu bị xóa
    }
}
