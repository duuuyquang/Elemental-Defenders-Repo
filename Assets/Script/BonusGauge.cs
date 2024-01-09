using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class BonusGauge : MonoBehaviour
{
    const float DELTA_VALUE_SCALE_TO_POS = 0.8f;
    public float initialScaleX = 3f;

    public TextMeshPro bonusScoreText;
    public GameObject scoreGainPrefab;

    private int curBonus = 3;
    private float speed;
    private bool startColdown = false;

    private GameManager gameManager;
    public Material material;

    public int CurBonus {  get { return curBonus; } set { curBonus = value; } }

    public bool StartColdown { get { return startColdown;  } set { startColdown = value; } }

    private void Start()
    {
        transform.localScale = new Vector3(initialScaleX, 0.2f, 1);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if(gameManager.GameMode == GameManager.MODE_DEFENSE)
        {
            gameObject.SetActive(false);
            return;
        }

        float enemySpeed = gameManager.GetEnemySpeed();
        GameObject enemyWall = GameObject.Find("EnemyWallInfo");
        GameObject playerWall = GameObject.Find("PlayerWallInfo");
        float distance = (enemyWall.transform.position.z - playerWall.transform.position.z);

        speed = initialScaleX * enemySpeed / distance;
    }

    // Update is called once per frame
    void Update()
    {
        if(startColdown)
        {
            Coldown();
        }
    }

    void Coldown()
    {
        if (transform.localScale.x > 0)
        {
            transform.localScale -= new Vector3(speed * Time.deltaTime * DELTA_VALUE_SCALE_TO_POS, 0, 0);
        }
        else
        {
            startColdown = false;
        }
        UpdateBonusTextAndGaugeColor();

    }

    void UpdateCurrentBonusScore()
    {
        float currentRate = transform.localScale.x / initialScaleX;
        curBonus = 0;
        if (currentRate > 0.8f)
        {
            curBonus = 3;
        }
        else if (currentRate > 0.55f)
        {
            curBonus = 2;
        }
        else if (currentRate > 0.3f)
        {
            curBonus = 1;
        }
    }

    void UpdateBonusTextAndGaugeColor()
    {
        UpdateCurrentBonusScore();
        bonusScoreText.text = "Bonus: " + (curBonus > 0 ? "+" + curBonus : 0);
        bonusScoreText.color = GetColorByScore(curBonus);
        material.color = GetColorByScore(curBonus);
    }

    public static Color GetColorByScore(int score)
    {
        Color curColor;
        switch (score)
        {
            case 0:
                curColor = new Color(1f, 1f, 1f);
                break;
            case 1:
                curColor = new Color(0f, 1f, 1f);
                break;
            case 2:
                curColor = new Color(1f, 1f, 0f);
                break;
            case 3:
                curColor = new Color(1f, 0.5f, 0f);
                break;
            default:
                curColor = new Color(1f, 1f, 1f);
                break;
        }

        return curColor;
    }

    public void DisplayBonusScoreGain(int score)
    {
        TextMeshPro text = scoreGainPrefab.GetComponent<TextMeshPro>();
        text.text = "+" + score;
        text.color = GetColorByScore(score);
        text.fontSize = 6;
        Instantiate(scoreGainPrefab, transform.position, scoreGainPrefab.transform.rotation);
    }

    public void ResetGauge()
    {
        startColdown = true;
        transform.localScale = new Vector3(initialScaleX, 0.2f, 1);
    }

    public void RegenGaugeByPercentage(float percent)
    {
        float value = initialScaleX * percent / 100;
        transform.localScale += new Vector3(value, 0, 0);
    }
}
