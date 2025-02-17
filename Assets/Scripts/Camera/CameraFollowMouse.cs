using UnityEngine;
using Cinemachine;

public class CameraFollowMouse : MonoBehaviour
{
    public Transform player; // Refer�ncia ao jogador
    public CinemachineVirtualCamera virtualCamera; // Refer�ncia � Cinemachine Virtual Camera
    public float followStrength = 2f; // Intensidade do deslocamento em dire��o ao mouse

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
            Debug.LogError("Framing Transposer n�o encontrado! Certifique-se de que o Body da Virtual Camera est� configurado como Framing Transposer.");
        }
    }

    void Update()
    {
        if (player == null || cam == null || framingTransposer == null) return;

        // Pega a posi��o do mouse no mundo
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = player.position.z;

        // Calcula a dire��o entre o player e o mouse
        Vector3 directionToMouse = mouseWorldPosition - player.position;

        // Ajusta o offset da c�mera para deslocar levemente em dire��o ao mouse
        framingTransposer.m_TrackedObjectOffset = directionToMouse * followStrength;
    }
}
