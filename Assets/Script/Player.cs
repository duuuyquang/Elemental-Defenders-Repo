using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float MAX_GAUGE = 100f;
    const float PLAYER_SPAWN_POS_Z = 17;
    const int NUM_BULLET_PER_SHOOT = 3;
    const int SHIFT_BULLET_POS_X = 5;

    public GameObject[] playerPrefabs;
    public GameObject bullet;
    public GameObject maxGaugeEffect;
    public GameObject gaugeExplosion;

    private Vector3[] playerSpawnPos =
    {
        new Vector3(-6, 0, -PLAYER_SPAWN_POS_Z),
        new Vector3(0, 0, -PLAYER_SPAWN_POS_Z),
        new Vector3(6, 0, -PLAYER_SPAWN_POS_Z)
    };


    private bool    isAttacking = false;
    private float   initialGaugeScale = 1f;
    private float   curGauge = 0f;
    private int     perfectChain = 0;
    private int     curPlayerSpawnPosIndex = 0;
    private int     spawnableNum = 3;

    private SoundController soundController;
    private GameManager gameManager;
    private BonusGauge bonusGauge;
    private Wall playerWall;
    private Player player;

    public GameObject curGaugeBar;

    public Vector3[] PlayerSpawnPos {  get { return playerSpawnPos; } }

    public bool IsAttacking {  get { return isAttacking; } set { isAttacking = value; } }

    public int CurPlayerSpawnPosIndex { get { return curPlayerSpawnPosIndex; } set { curPlayerSpawnPosIndex = value; } }

    public float CurGauge {
        get { return curGauge; }
        set { curGauge = Mathf.Min(MAX_GAUGE, Mathf.Max(value, 0)); }
    }

    public int PerfectChain {
        get { return perfectChain; }
        set { perfectChain = Mathf.Max(value, 0); }
    }

    public int SpawnableNum
    {
        get { return spawnableNum; }
        set { spawnableNum = value; }
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        playerWall = GameObject.Find("PlayerWall").GetComponent<Wall>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (IsAttackable() && Input.GetKeyDown(KeyCode.Space))
        {
            IsAttacking = true;
            gameManager.BlockPlayerSpawn();
            ProcessAttackEnemy();
            ReleaseGaugeBarPower();
            float regenHP = (Bullet.DAMAGE + player.PerfectChain) * gameManager.GetPlayerCurrentRegenRate();
            playerWall.RegenHP(regenHP);
            curPlayerSpawnPosIndex = 0;
        }

        if (spawnableNum <= 0)
        {
            gameManager.BlockPlayerSpawn();
        }
    }

    private void ProcessAttackEnemy()
    {
        ShootBulletWave();
    }

    public void SpawnElement(int type)
    {
        if (gameManager.PlayerSpawnable)
        {
            int bonusScore = 0;
            if ( gameManager.HasScoreSystem() )
            {
                if (!bonusGauge)
                {
                    bonusGauge = GameObject.Find("BonusGauge").GetComponent<BonusGauge>();
                }
                bonusScore = bonusGauge.CurBonus;
            }
            playerPrefabs[type].GetComponent<OnTouchEnemy>().bonusScore = bonusScore;

            soundController.PlayPlayerSpawn();

            Instantiate(
                playerPrefabs[type],
                playerSpawnPos[curPlayerSpawnPosIndex] + new Vector3(0, playerPrefabs[type].transform.position.y, 0),
                playerPrefabs[type].transform.rotation);

            curPlayerSpawnPosIndex++;
            if(curPlayerSpawnPosIndex > GameManager.PLAYER_POS_INDEX_MAX)
            {
                curPlayerSpawnPosIndex = 0;
            }

            spawnableNum--;
        }
    }

    private void ShootBulletWave()
    {
        soundController.ToggleBulletAura(true);
        StartCoroutine(ShootBulletEach());
    }

    IEnumerator ShootBulletEach()
    {
        int count = NUM_BULLET_PER_SHOOT;
        int initalPosX = -SHIFT_BULLET_POS_X;
        while (count > 0)
        {
            ShootBullet(new Vector3(initalPosX, 1, -21));
            initalPosX += SHIFT_BULLET_POS_X;
            count--;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ShootBullet(Vector3 position)
    {
        soundController.PlayShootBullet();
        Instantiate(bullet, position, bullet.transform.rotation);
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
        }
        else
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
