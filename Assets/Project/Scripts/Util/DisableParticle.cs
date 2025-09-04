using UnityEngine;

public class DisableParticle : MonoBehaviour
{
    public float timeDisabled = 1;
    
    private void OnEnable()
    {
        Invoke("DisabledObject",timeDisabled);
    }

    private void DisabledObject() => gameObject.SetActive(false);
}
