using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Referência ao Player")]
    [Tooltip("Arraste aqui o Transform do player")]
    public Transform target;

    [Header("Offset")]
    [Tooltip("Posição da câmera em relação ao player")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    void LateUpdate()
    {
        if (target == null) return;
        // Sem delay, sem suavização: segue 1:1
        transform.position = target.position + offset;
    }
}
