using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] int m_number;
    [SerializeField] Color m_color;
    [SerializeField] GameObject m_aim;
    [SerializeField] Rigidbody2D m_rigidbody;
    [SerializeField] CircleCollider2D m_collider;

    private bool m_isFirstTimeDrop = true;
    private float m_thresholdTime = 1f;
    private float m_timeCount = 0f;

    public bool IsMerged
    {
        get; set;
    }

    public int Number => m_number;
    public Color Color => m_color;
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
        if (collision.transform.CompareTag("Bottom"))
        {
            if (m_isFirstTimeDrop && m_rigidbody.velocity.x == 0)
            {
                m_isFirstTimeDrop = false;
                var random = Random.Range(-2f, 2f);
                m_rigidbody.velocity = new Vector2(random, random);

                SoundManager.Instance.PlaySound("Collide");
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Top"))
        {
            if (m_timeCount > m_thresholdTime)
            {
                GameManager.Instance.EndGame();
            }
            else
            {
                m_timeCount += Time.deltaTime;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Top"))
        {
            m_timeCount = 0f;
        }
    }
    private void Update()
    {
        if (m_rigidbody.velocity.y > 2)
        {
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0f);
        }
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
