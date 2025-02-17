using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrackerForNewPlayer : MonoBehaviour
{
    public Transform target, center;
    public float distance = 3f;

    [HideInInspector] public bool canTrack = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canTrack) return;
        MouseTrackerPosition();

    }

    private void MouseTrackerPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;


        Vector3 direction = mousePos - center.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.position = center.position + Quaternion.Euler(0, 0, angle) * new Vector3(distance, 0, 0);
    }

    public void SwitchTrackAble(bool canIt)
    {
        canTrack = canIt;
    }

    public void SwitchTrackEnable()
    {
        canTrack = true;
    }
}
