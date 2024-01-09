using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float MAX_GAUGE = 100f;

    public GameObject bullet;
    public GameObject maxGaugeEffect;
    public GameObject gaugeExplosion;

    private float initialGaugeScale = 1f;
    private float curGauge = 0f;
    private int perfectChain = 0;

    public GameObject curGaugeBar;

    public float CurGauge {
        get { return curGauge; }
        set { curGauge = Mathf.Min(MAX_GAUGE, Mathf.Max(value, 0)); }
    }

    public int PerfectChain {
        get { return perfectChain; }
        set { perfectChain = Mathf.Max(value, 0); }
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
        ShootBullet();
    }

    private void ShootBullet()
    {
        Instantiate(bullet, new Vector3(-5, 1, -21), bullet.transform.rotation);
        Instantiate(bullet, new Vector3(0, 1, -19), bullet.transform.rotation);
        Instantiate(bullet, new Vector3(5, 1, -21), bullet.transform.rotation);
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
            if(count % 30 == 0)
            {
                Instantiate(gaugeExplosion, curGaugeBar.transform.position, gaugeExplosion.transform.rotation);
            }
            UpdateGaugeBar(count);
            count -= upf;
            yield return new WaitForSeconds(spf);
        }
    }
}
