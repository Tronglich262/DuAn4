using UnityEngine;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToNextLevel;
    public int statPoints; // Điểm cộng khi lên cấp
    public int attack;
    public int hp;
    public HealthSystem healthSystem;


    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI statPointsText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI hpText;
    public GameObject skillPointPanel; // Bảng cộng điểm kỹ năng

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
        levelText.text = "Level: " + level;
        expText.text = "EXP: " + currentExp + " / " + expToNextLevel;
        statPointsText.text = "Stat Points: " + statPoints;
        attackText.text = "Attack: " + attack;
        hpText.text = "HP: " + hp;
    }
    

    private void SaveLevelData()
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
            attack = PlayerPrefs.GetInt("Attack", 10); // Giá trị mặc định 10
            hp = PlayerPrefs.GetInt("HP", 100); // Giá trị mặc định 100
        }
        else
        {
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
        PlayerPrefs.Save();      // Đảm bảo dữ liệu bị xóa
    }


}
