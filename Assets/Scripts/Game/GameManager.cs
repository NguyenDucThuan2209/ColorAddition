using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    Initializing,
    Playing,
    Pausing,
    End
}

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;

    [SerializeField] GameState m_state;
    [SerializeField] float m_sensitive;
    [SerializeField] Vector2 m_vertical;
    [SerializeField] Vector2 m_horizontal;
    [SerializeField] Transform m_playZone;
    [SerializeField] Transform m_dropPoint;
    [SerializeField] Transform m_sphereHolder;
    [SerializeField] GameObject m_mergeEffect;
    [SerializeField] GameObject[] m_spherePrefab;

    private bool m_isReady = false;
    private bool m_isAiming = false;

    private int m_score = 0;
    private int m_level = 1;
    private int m_highScore = 0;
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
        if (m_state != GameState.Playing) return;

        if (!m_isReady)
        {
            if (m_timeCount >= m_delayAiming)
            {
                int randomIndex = Random.Range(0, m_level);

                m_timeCount = 0;
                m_isReady = true;
                m_currentSphere = Instantiate(m_spherePrefab[randomIndex], m_dropPoint.position, Quaternion.identity, m_sphereHolder).GetComponent<Sphere>();
            }
            else
            {
                m_timeCount += Time.deltaTime;
                return;
            }
        }
        
        if (m_currentSphere != null)
        {
            if (Input.GetMouseButtonDown(0) && !m_isAiming)
            {
                m_isAiming = true;
                m_currentX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            }
            else if (Input.GetMouseButton(0) && m_isAiming)
            {
                var newX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                var radius = m_currentSphere.Collider.radius / 2 + 0.01f;
                var deltaX = (newX - m_currentX) * m_sensitive;
                bool isInBound = (m_horizontal.x + radius < m_currentSphere.transform.position.x + deltaX)
                                && (m_currentSphere.transform.position.x + deltaX < m_horizontal.y - radius);

                if (Mathf.Abs(deltaX) > 0.001f && isInBound)
                {
                    m_currentX = newX;
                    m_currentSphere.transform.position += new Vector3(deltaX, 0f, 0f);
                }
            }
            else if (Input.GetMouseButtonUp(0) && m_isAiming)
            {
                m_currentSphere.Rigidbody.gravityScale = 1;
                m_currentSphere.Aim.SetActive(false);
                m_currentSphere = null;
                m_isAiming = false;
                m_isReady = false;
            }
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
        m_score = 0;
        m_level = 3;
        m_timeCount = 0f;
        m_isReady = false;
        m_isAiming = false;
        m_currentSphere = null;
    }
    private void ResetGameProperties()
    {
        for (int i = 0; i < m_sphereHolder.childCount; i++)
        {
            Destroy(m_sphereHolder.GetChild(i).gameObject);
        }
    }
    private void RandomizeSpawn()
    {
        int amount = Random.Range(3, 7);
        for (int i = 0; i < amount; i++)
        {
            var index = Random.Range(0, m_level);
            var x = Random.Range(m_horizontal.x + 0.5f, m_horizontal.y - 0.5f);
            var y = Random.Range(m_vertical.x + 0.5f, m_vertical.y - 0.5f);
            var sphere = Instantiate(m_spherePrefab[index], new Vector2(x, y), Quaternion.identity, m_sphereHolder);
            
            sphere.GetComponent<Sphere>().Ready();
        }
    }

    public void StartGame()
    {
        Debug.LogWarning("Start Game");
        m_state = GameState.Playing;

        m_playZone.gameObject.SetActive(true);
        m_sphereHolder.gameObject.SetActive(true);
        ResetGameData();
        ResetGameProperties();
        
        RandomizeSpawn();
    }
    public void PauseGame()
    {
        Debug.LogWarning("Pause Game");
        m_isAiming = false;
        m_state = GameState.Pausing;

        m_playZone.gameObject.SetActive(false);
        m_sphereHolder.gameObject.SetActive(false);
    }
    public void ResumeGame()
    {
        Debug.LogWarning("Resume Game");
        m_state = GameState.Playing;

        m_playZone.gameObject.SetActive(true);
        m_sphereHolder.gameObject.SetActive(true);
    }
    public void EndGame()
    {
        Debug.LogWarning("End Game");
        m_state = GameState.End;

        MenuManager.Instance.EndGame();
        m_playZone.gameObject.SetActive(false);
        m_sphereHolder.gameObject.SetActive(false);

        if (m_score > m_highScore)
        {
            m_highScore = m_score;
        }
        MenuManager.Instance.SetScoreEndGame(m_score, m_highScore);
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

        var newSphere = Instantiate(m_spherePrefab[level], centerPos, Quaternion.identity, m_sphereHolder).GetComponent<Sphere>();
        newSphere.Rigidbody.velocity = velocity;
        newSphere.Ready();

        var effect = Instantiate(m_mergeEffect, centerPos, Quaternion.identity, m_sphereHolder);
        var effectData = effect.GetComponent<ParticleSystem>().main;
        effectData.startColor = newSphere.Color;
        effect.GetComponent<ParticleSystem>().Play();

        sphere1.IsMerged = true;
        sphere2.IsMerged = true;

        Destroy(sphere1.gameObject);
        Destroy(sphere2.gameObject);
        
        if (level > m_level)
        {
            m_level++;
        }
        m_score += sphere1.Number + sphere2.Number;
        MenuManager.Instance.SetScoreInGame(m_score);
        SoundManager.Instance.PlaySound("Merge");
    }
}
