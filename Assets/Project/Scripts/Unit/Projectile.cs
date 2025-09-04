using UnityEngine;

public class Projectile : MonoBehaviour
{
    public AudioClip audioBreack;
    private Transform target;
    private int damage;
    private bool isPlayer;

    public float speed = 10f;
    public float arcHeight = 2f; // altura do arco

    private Vector3 startPos;
    private Vector3 targetPos;
    private float travelProgress;

    public void Initialize(Transform target, int damage, bool isPlayer)
    {
        this.target = target;
        this.damage = damage;
        this.isPlayer = isPlayer;

        startPos = transform.position;
        if (target != null) targetPos = target.position;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // avança no "tempo" do trajeto
        travelProgress += Time.deltaTime * speed / Vector3.Distance(startPos, target.position);
        travelProgress = Mathf.Clamp01(travelProgress);

        // interpolação de posição
        Vector3 currentPos = Vector3.Lerp(startPos, target.position, travelProgress);

        // adiciona altura do arco (parábola)
        float height = 4f * arcHeight * travelProgress * (1 - travelProgress);
        currentPos.y += height;

        transform.position = currentPos;

        // chegou no alvo
        if (travelProgress >= 1f)
        {
            var unit = target.GetComponent<UnitController>();
            if (unit != null)
            {
                CombatSystem.AttackRanged(damage, unit);
            }
            else
            {
                var baseAttack = target.GetComponent<Base>();
                if (baseAttack != null)
                    baseAttack.health.TakeDamage(damage);
            }
            
            AudioManager.Play(audioBreack,AudioManager.AudioType.FX);
            DropManager.RockFX(transform);

            Destroy(gameObject);
        }
    }
}