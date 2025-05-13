using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referência ao Player")]
    [Tooltip("Arraste aqui o Transform do player")]
    public Transform target;

    [Header("Offset")]
    [Tooltip("Posição da câmera em relação ao player")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Suavização")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;

        // posição desejada = player + offset
        Vector3 desiredPosition = target.position + offset;
        // suaviza a transição
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}