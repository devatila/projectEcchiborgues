using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public bool GUN_FACING_RIGHT = true;
    public float tolerance = 1f;
    public float CorrectAngle = 90f;
    public GameObject masterParent;

    public Transform secondBone;
    public float secondBoneCorrectAngle = 70f;
    public float minRotation = -45f;
    public float maxRotation = 45f;

    public Transform hands;

    private Transform secondBoneTransform;
    // Start is called before the first frame update  // 30, -50
    void Start()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Verifica se o mouse está à esquerda ou à direita do player
        bool isMouseOnRightSide = mousePosition.x > transform.position.x;

        secondBoneTransform = secondBone;
        Debug.Log(secondBoneTransform.localRotation);
    }

    // Update is called once per frame
    void Update()
    {
        MouseLooker();

    }

    private void MouseLooker()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Verifica se o mouse está à esquerda ou à direita do player
        bool isMouseOnRightSide = mousePosition.x > transform.position.x;

        // Se o mouse estiver à direita do player, limitamos o ângulo entre -45 e 45
        if (isMouseOnRightSide)
        {
            // Limita o ângulo entre -45 e 45 para a direita
            float clampedAngle = Mathf.Clamp(angle, minRotation, maxRotation);
            transform.localEulerAngles = new Vector3(0, 0, clampedAngle);
            SecondBoneLooker(angle, secondBone.gameObject, isMouseOnRightSide);
            
        }
        else
        {
            // Para a esquerda, ajustamos os ângulos para refletir uma rotação correta, e refletir tambem o que eu to fazendo da minha vida meu Jesus!
            float invertedAngle = angle + 180f;

        
            if (invertedAngle > 180) invertedAngle -= 360f;
            float clampedAngle = Mathf.Clamp(invertedAngle * -1, minRotation, maxRotation);
            transform.localEulerAngles = new Vector3(0, 0, clampedAngle);
            SecondBoneLooker(angle, secondBone.gameObject, isMouseOnRightSide);
            
        }
        // ENFIM ESSA DESGRAÇA FUNCIOOOOOO, DUAS TARDE PRA FAZER MAS A GLÓRIA É ETERNA

        //HandsLook(angle, hands);

        //GunFlipController(mousePosition);
    }

    void GunFlipController(Vector3 mousePos)
    {
        if (mousePos.x < transform.position.x && GUN_FACING_RIGHT)
        {
            GunFlip();
        }
        else if (mousePos.x > transform.position.x && !GUN_FACING_RIGHT)
        {
            GunFlip();
        }
    }

    void GunFlip()
    {
        GUN_FACING_RIGHT = !GUN_FACING_RIGHT;
        masterParent.transform.localScale = new Vector3(masterParent.transform.localScale.x * -1, masterParent.transform.localScale.y , masterParent.transform.localScale.z);
        //masterParent.transform.Rotate(0, 180, 0);
    }

    void SecondBoneLooker(float angle, GameObject obj, bool isInRight)
    {
        // Por favor...que dessa vez eu não leve duas tardes pra fazer um simples negocio ;-;
        if (isInRight) CorrectAngle = 60;
        else CorrectAngle = 120;

      
        obj.transform.rotation = Quaternion.Euler(0, 0, angle + CorrectAngle);
    }
    void HandsLook(float angle, Transform handObject)
    {
        handObject.rotation = Quaternion.Euler(0, 0, angle);
    }


}
