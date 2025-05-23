using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private GameObject fxPrefab;
    [SerializeField] private Transform fxSpawnPoint;
    public int count = 0;
    public bool canMove = true; // Cờ cho phép di chuyển
    private bool canAttack = true;
    public PlayerController playerController;

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
        animator.SetBool("Attack", true);
        yield return new WaitForSeconds(1f);

        if (fxPrefab != null && fxSpawnPoint != null)
        {
            GameObject spawn = Instantiate(fxPrefab, fxSpawnPoint.position, Quaternion.identity);
            Destroy(spawn, 1.5f);
        }

        yield return new WaitForSeconds(0.2f);

        animator.SetBool("Attack", false);

  
        yield return new WaitForSeconds(0.2f);

        canAttack = true;
    }
}
