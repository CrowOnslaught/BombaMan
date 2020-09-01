using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_FollowScript : MonoBehaviour
{
    public GameObject m_target;
    public Vector3 m_offset;
    public float m_speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_target != null)
            this.transform.position = Vector3.Lerp(transform.position, m_target.transform.position + m_offset, m_speed);
    }
}
