using UnityEngine;

public class SpringBone2D : MonoBehaviour
{
    public Transform[] bones; // Os 3 bones do rabo de cavalo
    public float springForce = 10f; // Força da mola para puxar de volta
    public float damping = 0.8f; // Amortecimento para suavizar o movimento
    public float maxDistance = 0.2f; // Máxima distância que os bones podem se afastar
    public Vector3[] originalPositions; // Posições originais dos bones
    private Vector3[] velocities; // Velocidades atuais dos bones

    void Start()
    {
        // Armazena as posições originais dos bones
        originalPositions = new Vector3[bones.Length];
        velocities = new Vector3[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            originalPositions[i] = bones[i].position;
        }
    }

    void Update()
    {
        // Aplica o efeito de mola para cada bone
        for (int i = 0; i < bones.Length; i++)
        {
            // Calcula o deslocamento em relação à posição original
            Vector3 displacement = bones[i].position - originalPositions[i];
            float distance = displacement.magnitude;

            // Limita a distância máxima que o bone pode se afastar
            if (distance > maxDistance)
            {
                displacement = displacement.normalized * maxDistance;
                bones[i].position = originalPositions[i] + displacement;
            }

            // Calcula a força da mola
            Vector3 springForceVector = -displacement * springForce;
            velocities[i] += springForceVector * Time.deltaTime;

            // Aplica a velocidade calculada ao bone
            bones[i].position += velocities[i] * Time.deltaTime;

            // Aplica amortecimento
            velocities[i] *= damping;
        }
    }
}
