using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;

    [SerializeField] float m_sensitive;
    [SerializeField] Vector2 m_vertical;
    [SerializeField] Vector2 m_horizontal;
    [SerializeField] Transform m_dropPoint;
    [SerializeField] Transform m_sphereHolder;
    [SerializeField] GameObject[] m_spherePrefab;

    private bool m_isAiming = false;

    private int m_level = 1;
    private float m_currentX = 0f;
    private float m_timeCount = 0f;
    private float m_delayAiming = 1f;

    private Sphere m_currentSphere;

    public Vector2 Vertical => m_vertical;
    public Vector2 Horizontal => m_horizontal;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }
    private void Update()
    {
        if (!m_isAiming)
        {
            if (m_timeCount >= m_delayAiming)
            {
                int randomIndex = Random.Range(0, m_level);

                m_timeCount = 0;
                m_isAiming = true;
                m_currentSphere = Instantiate(m_spherePrefab[randomIndex], m_dropPoint.position, Quaternion.identity, m_sphereHolder).GetComponent<Sphere>();
            }
            else
            {
                m_timeCount += Time.deltaTime;
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_currentX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        }
        if (Input.GetMouseButton(0))
        {
            var newX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            var radius = m_currentSphere.Collider.radius + 0.01f;
            var deltaX = (newX - m_currentX) * m_sensitive;
            bool isInBound = (m_horizontal.x + radius < m_currentSphere.transform.position.x + deltaX)
                          && (m_currentSphere.transform.position.x + deltaX < m_horizontal.y - radius);

            if (Mathf.Abs(deltaX) > 0.001f && isInBound)
            {
                m_currentX = newX;
                m_currentSphere.transform.position += new Vector3(deltaX, 0f, 0f);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_currentSphere.Rigidbody.gravityScale = 1;
            m_currentSphere.Aim.SetActive(false);
            m_currentSphere = null;
            m_isAiming = false;
        }
    }

    private int GetLevel(int number)
    {
        int t = 0;
        while (number > 1)
        {
            t++;
            number /= 2;
        }
        return t;
    }
    private void ResetGameData()
    {
        m_level = 3;
    }

    public void StartGame()
    {
        ResetGameData();
    }
    public void Merge(Sphere sphere1, Sphere sphere2)
    {
        if (sphere1.IsMerged || sphere2.IsMerged) return;

        var centerPos = new Vector3((sphere1.transform.position.x + sphere2.transform.position.x) / 2f,
                                    (sphere1.transform.position.y + sphere2.transform.position.y) / 2f,
                                    (sphere1.transform.position.z + sphere2.transform.position.z) / 2f);

        var velocity = (sphere1.Rigidbody.velocity == Vector2.zero || sphere2.Rigidbody.velocity == Vector2.zero) ?
                       ((sphere1.Rigidbody.velocity == Vector2.zero) ? sphere2.Rigidbody.velocity : sphere1.Rigidbody.velocity) :
                        Vector2.Scale(sphere1.Rigidbody.velocity, sphere2.Rigidbody.velocity);
        var level = GetLevel(sphere1.Number);

        var newSphere = Instantiate(m_spherePrefab[level], centerPos, Quaternion.identity).GetComponent<Sphere>();
        newSphere.Rigidbody.velocity = velocity;
        newSphere.Ready();

        sphere1.IsMerged = true;
        sphere2.IsMerged = true;

        sphere1.gameObject.SetActive(false);
        sphere2.gameObject.SetActive(false);
        
        if (level > m_level)
        {
            m_level++;
        }

        Debug.LogWarning($"{sphere1.name}|{sphere2.name}|{GetLevel(sphere1.Number)}");
    }
}
