using UnityEngine;
using Cinemachine;

public class CameraFollowMouse : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public CinemachineVirtualCamera virtualCamera; // Referência à Cinemachine Virtual Camera
    public float followStrength = 2f; // Intensidade do deslocamento em direção ao mouse

    private CinemachineFramingTransposer framingTransposer;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (virtualCamera != null)
        {
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        if (framingTransposer == null)
        {
            Debug.LogError("Framing Transposer não encontrado! Certifique-se de que o Body da Virtual Camera está configurado como Framing Transposer.");
        }
    }

    void Update()
    {
        if (player == null || cam == null || framingTransposer == null) return;

        // Pega a posição do mouse no mundo
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = player.position.z;

        // Calcula a direção entre o player e o mouse
        Vector3 directionToMouse = mouseWorldPosition - player.position;

        // Ajusta o offset da câmera para deslocar levemente em direção ao mouse
        framingTransposer.m_TrackedObjectOffset = directionToMouse * followStrength;
    }
}
