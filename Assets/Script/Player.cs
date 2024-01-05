using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float MAX_GAUGE = 100f;

    public GameObject bullet;
    public GameObject maxGaugeEffect;
    public GameObject gaugeExplosion;

    private float curGauge;
    private float initialGaugeScale = 1f;

    private GameObject curGaugeBar;

    public float CurGauge {
        get { return curGauge; }
        set { curGauge = Mathf.Min(MAX_GAUGE, Mathf.Max(value, 0)); }
    }

    // Start is called before the first frame update
    void Start()
    {
        curGaugeBar = GameObject.Find("curGauge");
        curGauge = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAttackable() && Input.GetKeyDown(KeyCode.Space))
        {
            ProcessAttackEnemy();
            ReleaseGaugeBarPower();
        }
    }

    private void ProcessAttackEnemy()
    {
        Enemy.SetAllUnitSpeed(-1.0f);
        //StartCoroutine(SpamBullet());
        ShootBullet();
    }

    IEnumerator SpamBullet()
    {
        int count = 0;
        while(count < 5)
        {
            Instantiate(bullet, new Vector3(-5, 1, -20), bullet.transform.rotation);
            Instantiate(bullet, new Vector3(0, 1, -19), bullet.transform.rotation);
            Instantiate(bullet, new Vector3(5, 1, -20), bullet.transform.rotation);
            yield return new WaitForSeconds(0.2f);
            count++;
        }
    }

    private void ShootBullet()
    {
        Instantiate(bullet, new Vector3(-5, 1, -20), bullet.transform.rotation);
        Instantiate(bullet, new Vector3(0, 1, -19), bullet.transform.rotation);
        Instantiate(bullet, new Vector3(5, 1, -20), bullet.transform.rotation);
    }

    private bool IsAttackable()
    {
        return curGauge >= MAX_GAUGE;
    }

    public void UpdateGaugeBar(float value)
    {
        curGauge = Mathf.Max(0, value);
        float curPercentage = curGauge / MAX_GAUGE;
        float newY = (initialGaugeScale - curPercentage) * 0.5f;
        curGaugeBar.transform.localPosition = new Vector3(curGaugeBar.transform.localPosition.x, -newY, curGaugeBar.transform.localPosition.z);
        curGaugeBar.transform.localScale = new Vector3(curGaugeBar.transform.localScale.x, curPercentage, curGaugeBar.transform.localScale.z);
        if (IsAttackable()) 
        {
            maxGaugeEffect.SetActive(true);
        } else
        {
            maxGaugeEffect.SetActive(false);
        }
    }

    private void ReleaseGaugeBarPower()
    {
        StartCoroutine(GaugeBarExplosion());
    }

    IEnumerator GaugeBarExplosion()
    {
        Instantiate(gaugeExplosion, curGaugeBar.transform.position, gaugeExplosion.transform.rotation);
        float count = 99;
        float spf = 1.0f / 60.0f;
        float upf = 1f;
        while (count >= 0)
        {
            if(count % 50 == 0)
            {
                Instantiate(gaugeExplosion, curGaugeBar.transform.position, gaugeExplosion.transform.rotation);
            }
            UpdateGaugeBar(count);
            count -= upf;
            yield return new WaitForSeconds(spf);
        }
    }
}
