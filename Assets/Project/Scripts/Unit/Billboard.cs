using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Pega a câmera principal
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;
        
        Vector3 lookPos = transform.position + mainCamera.transform.rotation * Vector3.forward;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
    }
}
