using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class EnemyHealth : MonoBehaviour
{
    public Slider healthBar; // Kéo thả thanh máu vào đây
    public float maxHealth = 100f;
    private float currentHealth;
    public GameObject damageTextPrefab; // Kéo thả Prefab vào Inspector
    private Animator animator;

    // Sát thương mặc định của các chiêu
    public float baseDame1 = 100f; 
    public float baseDame2 = 60f;
    public float baseDame3 = 70f;
    public float baseDame4 = 50f;
    public float baseDame5 = 0f; // Chiêu 5 chỉ dùng attack

    // Tham chiếu đến LevelSystem để lấy attack
    public LevelSystem levelSystem;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu vật thể va chạm có tag là "chieu1", "chieu2", "chieu3" hoặc "chieu4"
        if (other.CompareTag("Chieu1"))
        {
            StartCoroutine(DameChieu1());
        }
        if (other.CompareTag("Chieu2"))
        {
            StartCoroutine(DameChieu2());
        }
        if (other.CompareTag("Chieu3"))
        {
            StartCoroutine(DameChieu3());
        }
        if (other.CompareTag("Chieu4"))
        {
            StartCoroutine(DameChieu4());
        }
        if (other.CompareTag("Chieu5"))
        {
            StartCoroutine(DameChieu5());
        }
    }

    IEnumerator DameChieu1()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Chung chiêu 1");
        StartCoroutine(hit());
        TakeDamage(baseDame1 + levelSystem.attack); // Cộng thêm attack vào chiêu 1
    }

    IEnumerator DameChieu2()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Chung chiêu 2");
        StartCoroutine(hit());
        TakeDamage(baseDame2 + levelSystem.attack); // Cộng thêm attack vào chiêu 2
    }

    IEnumerator DameChieu3()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Chung chiêu 3");
        StartCoroutine(hit());
        TakeDamage(baseDame3 + levelSystem.attack); // Cộng thêm attack vào chiêu 3
    }

    IEnumerator DameChieu4()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Chung chiêu 4");
        StartCoroutine(hit());
        TakeDamage(baseDame4 + levelSystem.attack); // Cộng thêm attack vào chiêu 4
    }

    IEnumerator DameChieu5()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Chung chiêu 5");
        StartCoroutine(hit());
        TakeDamage(levelSystem.attack); // Chiêu 5 chỉ dùng sát thương từ attack
    }

    IEnumerator hit()
    {
        animator.SetBool("Hit1", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Hit1", false);
    }

    IEnumerator Death()
    {
        animator.SetBool("Death1", true);
    
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Chờ animation chết chạy xong
    
        Destroy(gameObject); // Xóa Enemy sau khi animation chết hoàn thành
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        ShowDamageText(damage);

        if (currentHealth <= 0)
        {
            StartCoroutine(Death()); // Gọi coroutine Death(), chờ animation rồi mới xóa
        }
    }

    // Hàm tạo text sát thương
    void ShowDamageText(float damage)
    {
        if (damageTextPrefab != null)
        {
            // Chuyển vị trí quái từ World Space -> Screen Space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1f, 0));

            // Tạo DamageText
            GameObject text = Instantiate(damageTextPrefab, GameObject.Find("Canvas").transform);
            text.GetComponent<DamageText>().Setup((int)damage, this.transform);
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " đã bị tiêu diệt!");
        Destroy(gameObject); // Xóa quái khi hết máu (có thể thay bằng animation chết)
    }
}
