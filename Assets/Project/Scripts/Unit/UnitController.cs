using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AttackType
{
    Melee,
    Ranged,
    Big
}

public class UnitController : MonoBehaviour
{
    public bool isPlayer;
    public Unit currentUnit;
    public Transform currentTarget;

    [Header("Attack Settings")]
    public AttackType attackType = AttackType.Melee;
    public GameObject projectilePrefab; // prefab da pedra
    public Transform firePoint;         // onde sai o projétil

    [HideInInspector] public Health health;
    public EnemySpawner enemySpawner;
    public PlayerSpawner playerSpawner;
    public UIHeathBar uIHeathBar;
    private UnitView _unitView;
    private NavMeshAgent _agent;

    private float lastAttackTime = 0f;
    public bool isOnZoneAttacking = false;
    public bool isAttacking = false;
    public bool isDead = false;
    
    private Health targetHealth;
    private Base baseAttack;

    private void Awake()
    {
        currentUnit = GetComponent<Unit>();
        health = GetComponent<Health>();
        uIHeathBar.isPlayer = isPlayer;
        _unitView  = GetComponent<UnitView>();
        _agent = GetComponent<NavMeshAgent>();
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        playerSpawner = FindFirstObjectByType<PlayerSpawner>();

        health.onDie += OnDie;
        if (!isPlayer) health.onDie += DropCurrency;
    }

    private void Update()
    {
        if (currentTarget == null || isDead) return;

        var distance = Vector3.Distance(transform.position, currentTarget.position);

        float velocity = _agent.velocity.magnitude;
        float normalized = velocity / _agent.speed;
        
        _unitView.SpeedAnimation(normalized);
        
        if (distance <= currentUnit.attackRange)
        {
            _agent.isStopped = true; 
            TryAttack();
        }
        else
        {
            _agent.isStopped = false;
            _agent.SetDestination(currentTarget.position); 
        }
    }

    public void OnEnable()
    {
        FindNewTarget();
        _agent.enabled = true;
        isDead = false;
        isAttacking = false;
        isOnZoneAttacking = false;
        _unitView.StartAnimation();
    }
    
    private void TryAttack()
    {
        if (!(Time.time >= lastAttackTime + currentUnit.attackSpeed) || isDead) return;
        lastAttackTime = Time.time;
        isAttacking = true;
        _unitView.CallAttack();
        Invoke(nameof(DoAttack), currentUnit.attackDelay);
    }

    private void DoAttack()
    {
        var targetUnit = currentTarget.GetComponent<UnitController>();
        baseAttack = null;
        
        if (currentTarget.name == "HitPoint")
        {
            baseAttack  = isPlayer ? enemySpawner.baseSpawner : playerSpawner.baseSpawner;
        }
        
        if (targetUnit != null)
        {
            if (targetUnit.health.IsDead)
            {
                isAttacking = false;
                targetHealth = null;
                FindNewTarget();
                return;
            }
            
            targetHealth = targetUnit.health;

            if (attackType == AttackType.Melee)
            {
                CombatSystem.Attack(this, targetUnit);
            }
            else if (attackType == AttackType.Ranged)
            {
                ShootProjectile(targetUnit.transform);
            }

            Invoke(nameof(CheckEnemy), 0.1f);
            Debug.Log("ON UNIT");
        }
        else if (baseAttack != null)
        {
            if (attackType == AttackType.Melee)
            {
                targetHealth = baseAttack.health;
                targetHealth.TakeDamage(currentUnit.attackDamage);
            }
            else if (attackType == AttackType.Ranged)
            {
                ShootProjectile(baseAttack.transform);
            }
            
            Debug.Log("ON BASE");
        }

        AudioManager.Play(_unitView.audioHit, AudioManager.AudioType.FX);
        isAttacking = false;
    }

    private void ShootProjectile(Transform target)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();
        p.Initialize(target, currentUnit.attackDamage, isPlayer);
    }

    private void CheckEnemy()
    {
        if (!targetHealth.IsDead || isDead) return;

        targetHealth = null;
        FindNewTarget();
    }

    public void FindNewTarget()
    {
        if (!gameObject.activeSelf || isAttacking) return;
        UnitController[] playerUnits = FindObjectsOfType<UnitController>(); 
        float closestDistance = Mathf.Infinity;
        UnitController closestUnit = null;
        isAttacking = false;
        isOnZoneAttacking = false;
        
        foreach (UnitController unit in playerUnits)
        {
            if (isPlayer)
            {
                if (!unit.CompareTag("EnemyUnit")) continue;
                float distance = Vector3.Distance(transform.position, unit.transform.position);
                if (!(distance < closestDistance)) continue;
                if (unit.isDead) continue;
                closestDistance = distance;
                closestUnit = unit;
            }
            else
            {
                if (!unit.CompareTag("PlayerUnit")) continue;
                float distance = Vector3.Distance(transform.position, unit.transform.position);
                if (!(distance < closestDistance)) continue;
                if (unit.isDead) continue;
                closestDistance = distance;
                closestUnit = unit; 
            }
        }

        if (closestUnit != null)
        {
            currentTarget = closestUnit.transform;
        }
        else 
        {
            currentTarget = isPlayer ? enemySpawner.hitPoint : playerSpawner.hitPoint;
        }
        
        _agent.SetDestination(currentTarget.position);
    }

    private void OnDie() => StartCoroutine(IE_Dead());

    private IEnumerator IE_Dead()
    {
        isDead = true;
        _agent.enabled = false;
        _unitView.Dead();
        if(isPlayer) enemySpawner.ReFindUnits();
        else playerSpawner.ReFindUnits();
        
        yield return new WaitForSeconds(1f);
        DropManager.SmokeDie(transform);
        gameObject.SetActive(false);
    }

    private void DropCurrency() 
    {
        // Determina se é inimigo elite baseado no tipo de ataque
        bool isElite = attackType == AttackType.Big;
        DropManager.DropCurrency(transform, isElite);
        
        // Atualiza conquistas se for inimigo morto pelo jogador (temporariamente desabilitado para WebGL)
        // if (!isPlayer && AchievementManager.Instance != null)
        // {
        //     AchievementManager.Instance.UpdateProgress(AchievementType.KillEnemies);
        //     
        //     // Atualiza kill streak
        //     if (DropManager.currentKillStreak >= 20)
        //     {
        //         AchievementManager.Instance.UpdateProgress(AchievementType.KillStreak, DropManager.currentKillStreak);
        //     }
        // }
    }
}
