using UnityEngine;

public class Enemy : MonoBehaviour
{
    public LevelSystem levelSystem; // Kéo vào từ Inspector

    private void OnDestroy() // Khi quái vật chết
    {
        if (levelSystem != null)
        {
            int expGained = Random.Range(100, 200); // EXP ngẫu nhiên từ 100-200
            levelSystem.GainExp(expGained);
        }
    }
}