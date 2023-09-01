using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] int m_number;
    [SerializeField] GameObject m_aim;
    [SerializeField] Rigidbody2D m_rigidbody;
    [SerializeField] CircleCollider2D m_collider;

    public bool IsMerged
    {
        get; set;
    }

    public int Number => m_number;
    public GameObject Aim => m_aim;
    public Rigidbody2D Rigidbody => m_rigidbody;
    public CircleCollider2D Collider => m_collider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Sphere"))
        {
            if (collision.transform.GetComponent<Sphere>().Number == m_number)
            {
                GameManager.Instance.Merge(this, collision.transform.GetComponent<Sphere>());
            }
        }
    }
    private void Update()
    {
        if (m_rigidbody.velocity.y > 0 && transform.position.y > 3.5f)
        {
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, -m_rigidbody.velocity.y);
        }
    }

    public void Ready()
    {
        m_rigidbody.gravityScale = 1;
        m_aim.SetActive(false);
    }
}
