using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private GameObject fxPrefab;
    [SerializeField] private Transform fxSpawnPoint;
    public int count = 0;

    private bool canAttack = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack)
        {
            StartCoroutine(AttackSequence());
        }
    }

    IEnumerator AttackSequence()
    {
        canAttack = false; 

        // Bật animation Attack
        animator.SetBool("Attack", true);
        yield return new WaitForSeconds(1f);

        // Tạo hiệu ứng FX
        if (fxPrefab != null && fxSpawnPoint != null)
        {
            GameObject spawn = Instantiate(fxPrefab, fxSpawnPoint.position, Quaternion.identity);
            Destroy(spawn, 1.5f);
        }

        yield return new WaitForSeconds(0.2f);

        // Tắt animation Attack
        animator.SetBool("Attack", false);

        // Đợi cooldown 1.5 giây
        yield return new WaitForSeconds(0.2f);

        canAttack = true; // Cho phép tấn công lại
    }
}
