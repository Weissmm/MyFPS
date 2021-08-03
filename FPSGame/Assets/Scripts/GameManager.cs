using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gamePanel;
    public RectTransform anchoredBossHealthBar;

    public List<Enemy> basicRoomEnemies;
    public List<Enemy> bossRoomEnemies;

    private void Awake()
    {
        Instance = this;
        gamePanel.SetActive(true);
        anchoredBossHealthBar.position = Vector3.up * 30;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void TriggerBasicEnemies()
    {
        foreach(Enemy basicEnemy in basicRoomEnemies)
        {
            basicEnemy.ChaseStart();
        }
    }

    public void TriggerBoss()
    {
        anchoredBossHealthBar.anchoredPosition = Vector3.down * 10f;
        foreach (var enemy in bossRoomEnemies)
        {
            Boss boss = enemy as Boss;
            if (boss)
            {
                boss.StartAttack();
            }
        }
    }
}
