using System.Collections;
using UnityEngine;

public class BossControllerTwo : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float attackRange = 5f;
    public float rangedAttackRange = 8f;
    public float meleeAttackCooldown = 2f;
    public float rangedAttackCooldown = 4f;
    public int attackDamage = 10;
    public float jumpBackDistance = 2f;
    public float jumpSpeed = 4f;

    public Transform[] teleportPoints;
    public float teleportCooldown = 5f;

    public GameObject attackProjectile;
    public Transform attackSpawnPoint;

    public float dashCooldown = 8f;

    private bool isAttacking = false;
    private bool isFiring = false;

    private Animator animator;
    private float lastMeleeAttackTime;
    private float lastRangedAttackTime;
    private float lastTeleportTime;
    private float lastDashTime;

    private enum BossState { Idle, Chasing, Attacking, RangedAttacking, Dashing, Teleporting, Patrolling }
    private BossState currentState = BossState.Idle;

    private EnemyHealthTwo enemyHealthTwo;
    private int patrolIndex = 0;
    private float thinkDelay = 0.5f;
    private float nextThinkTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyHealthTwo = GetComponent<EnemyHealthTwo>();
        UpdateAnimator();
    }

    void Update()
    {
        if (Time.time < nextThinkTime) return; // Boss đang "suy nghĩ"

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isAttacking || isFiring) return;

        if (enemyHealthTwo != null && enemyHealthTwo.currentHealth <= enemyHealthTwo.maxHealth / 2 && Time.time - lastTeleportTime >= teleportCooldown)
        {
            // Nếu máu còn dưới 50% => ưu tiên teleport nhiều hơn
            currentState = BossState.Teleporting;
            StartCoroutine(TeleportRandomly());
            lastTeleportTime = Time.time;
        }
        else if (distanceToPlayer <= attackRange && Time.time - lastMeleeAttackTime >= meleeAttackCooldown)
        {
            currentState = BossState.Attacking;
            Flip(player.position.x);
            StartCoroutine(ApproachAndAttack());
        }
        else if (distanceToPlayer <= rangedAttackRange && Time.time - lastRangedAttackTime >= rangedAttackCooldown)
        {
            currentState = BossState.RangedAttacking;
            Flip(player.position.x);
            StartCoroutine(FireMultipleProjectiles());
        }
        else if (distanceToPlayer <= 10f && Time.time - lastDashTime >= dashCooldown && enemyHealthTwo.currentHealth > enemyHealthTwo.maxHealth * 0.5f)
        {
            currentState = BossState.Dashing;
            Flip(player.position.x);
            StartCoroutine(DashToPlayer());
            lastDashTime = Time.time;
        }
        else if (distanceToPlayer > rangedAttackRange)
        {
            currentState = BossState.Patrolling;
            Patrol();
        }
        else
        {
            currentState = BossState.Idle;
        }

        nextThinkTime = Time.time + thinkDelay; // Đợi thêm 0.5s mới suy nghĩ tiếp
        UpdateAnimator();
    }

    void Flip(float targetX)
    {
        if (targetX > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void ChasePlayer()
    {
        Flip(player.position.x);
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void Patrol()
    {
        Transform patrolPoint = teleportPoints[patrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, patrolPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolPoint.position) < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % teleportPoints.Length;
        }
    }

    void UpdateAnimator()
    {
        animator.SetBool("Run", currentState == BossState.Chasing || currentState == BossState.Dashing || currentState == BossState.Teleporting || currentState == BossState.Patrolling);

        if (currentState == BossState.Idle)
        {
            animator.SetBool("Run", false);
        }
    }

    IEnumerator ApproachAndAttack()
    {
        isAttacking = true;
        animator.SetBool("Run", true);

        while (Vector2.Distance(transform.position, player.position) > 1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("Run", false);
        ResetAttackAnimations();

        int randomAttack = Random.Range(0, 3);
        switch (randomAttack)
        {
            case 0: animator.SetBool("Attack", true); break;
            case 1: animator.SetBool("Attack1", true); break;
            case 2: animator.SetBool("Attack2", true); break;
        }

        yield return new WaitForSeconds(0.3f);

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.3f);
        ResetAttackAnimations();

        // Sau khi tấn công thì nhảy lùi ngay
        yield return StartCoroutine(JumpBack());

        lastMeleeAttackTime = Time.time;
        isAttacking = false;
    }

    IEnumerator FireMultipleProjectiles()
    {
        isFiring = true;
        animator.SetBool("Attack", true);

        for (int i = 0; i < 2; i++) // Bắn 2 phát liên tiếp
        {
            yield return new WaitForSeconds(0.2f);

            GameObject projectile = Instantiate(attackProjectile, attackSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * 10f;
        }

        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Attack", false);

        lastRangedAttackTime = Time.time;
        isFiring = false;
    }

    IEnumerator JumpBack()
    {
        animator.SetBool("Jump", true);

        float direction = transform.position.x > player.position.x ? 1f : -1f;
        Vector3 jumpTarget = new Vector3(transform.position.x + direction * jumpBackDistance, transform.position.y, transform.position.z);

        while (Vector3.Distance(transform.position, jumpTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, jumpTarget, jumpSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("Jump", false);
    }

    IEnumerator TeleportRandomly()
    {
        Transform targetPoint = teleportPoints[Random.Range(0, teleportPoints.Length)];
        animator.SetBool("Jump", true);
        yield return new WaitForSeconds(0.3f);

        transform.position = targetPoint.position;
        Flip(player.position.x);
        animator.SetBool("Jump", false);
    }

    IEnumerator DashToPlayer()
    {
        isAttacking = true;

        Vector2 target = player.position;
        float dashSpeed = 10f;

        while (Vector2.Distance(transform.position, target) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
            yield return null;
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            yield return StartCoroutine(ApproachAndAttack());
        }

        isAttacking = false;
    }

    void ResetAttackAnimations()
    {
        animator.SetBool("Attack", false);
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        animator.SetBool("Jump", false);
    }
}
