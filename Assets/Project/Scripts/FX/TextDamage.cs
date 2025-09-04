using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextDamage : MonoBehaviour
{
    public TextMeshPro textMesh;
    private Tween moveTween, scaleTween;
    
    public void Initialize(int damage,bool isPlayer ,Vector3 startPos, Color color, float duration = 0.7f, float height = 1f)
    {
        moveTween?.Kill();
        scaleTween?.Kill();

        textMesh.text = damage.ToString();
        textMesh.color = color;

        transform.position = startPos;

        float randomX = isPlayer ? Random.Range(-1f, -0.5f) : Random.Range(0.5f, 1f);
        float randomZ = Random.Range(-0.5f, 0.5f);
        Vector3 endPos = new Vector3(startPos.x + randomX, startPos.y, startPos.z + randomZ);

        // Movimento em parábola (como bolinha)
        moveTween = DOTween.To(
            () => 0f,
            t =>
            {
                // Movimento linear em XZ
                Vector3 pos = Vector3.Lerp(startPos, endPos, t);

                // Curva parabólica no Y: sobe e depois cai
                float parabola = 4 * height * (t - t * t); // 0→1→0
                pos.y += parabola;

                transform.position = pos;
            },
            1f, duration
        ).SetEase(Ease.Linear);

        // Escala inicial aleatória (variação no tamanho da bolinha)
        float randomScale = Random.Range(0.15f, 0.25f);
        transform.localScale = Vector3.one * randomScale;

        // Tween para encolher até sumir
        scaleTween = transform.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}
