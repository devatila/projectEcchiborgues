using UnityEngine;

public class SmoothFolliw : MonoBehaviour
{
    public Transform parentObject; // O objeto pai que o filho deve seguir
    public float followSpeed = 5f; // Velocidade com que o objeto filho segue o pai

    void Update()
    {
        // Interpolação suave da posição do objeto filho para a posição do objeto pai
        transform.position = Vector3.Lerp(transform.position, parentObject.position, followSpeed * Time.deltaTime);
    }
}
