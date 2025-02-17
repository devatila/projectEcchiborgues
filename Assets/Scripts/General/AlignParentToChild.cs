using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignParentToChild : MonoBehaviour
{
    public Transform childReferencePoint; // O ponto no filho que será a referência (empunhadura)
    public Transform parentObject;        // O objeto pai que precisa ser realinhado (a arma)
    public Transform targetPositionToChildren;
    private Vector3 correctedPosition;

    void Start()
    {
        FixGunPosition();
        
    }
    

    public void FixGunPosition()
    {
        if (childReferencePoint != null && parentObject != null)
        {
            if (targetPositionToChildren != null) childReferencePoint.position = targetPositionToChildren.position;

            // Pega a diferença entre a posição do pai e do filho
            Vector3 offset = parentObject.position - childReferencePoint.position;

            // Alinha o pai de volta com base na posição do filho (empunhadura)
            parentObject.position = parentObject.position + offset;
        }
    }

    public void GetFixedPosition()
    {
       
        parentObject.position = correctedPosition;
    }
}
// Finalmente um resolvedor de Gambiarras \[T]/
