using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    
    public Image hpBar; // Thanh máu
    public TextMeshProUGUI hpText; // Hiển thị số HP

    private void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        UpdateHPUI();
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        UpdateHPUI();
    }

    public void UpdateHPUI()
    {
        hpBar.fillAmount = (float)currentHP / maxHP;
        hpText.text = currentHP + " / " + maxHP;
    }

    public void UpdateMaxHP(int newMaxHP)
    {
        maxHP = newMaxHP;
        currentHP = maxHP; // Hồi đầy máu khi tăng chỉ số HP
        UpdateHPUI();
    }
}