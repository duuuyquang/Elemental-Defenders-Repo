using UnityEngine;

public class SoundController: MonoBehaviour
{
    public const int TYPE_STRONGER = 1;
    public const int TYPE_SAME = 2;
    public const int TYPE_WEAKER = 3;

    public const int TYPE_PLAYER_WALL = 1;
    public const int TYPE_ENEMY_WALL = 2;

    public AudioSource typeStronger;
    public AudioSource typeSame;
    public AudioSource typeWeaker;
    public AudioSource playerWallExplosion;
    public AudioSource bulletExplosion;
    public AudioSource bulletAura;
    public AudioSource shootBullet;
    public AudioSource spawnSound;
    public AudioSource themeMusic;

    private bool isThemeMusicPlay = true;


    public void PlayElementCollisionByType(int type)
    {
        switch (type)
        {
            case TYPE_STRONGER:
                typeStronger.Play();
                break;
            case TYPE_SAME:
                typeSame.Play();
                break;
            case TYPE_WEAKER:
                typeWeaker.Play();
                break;
        }
    }

    public void PlayShootBullet()
    {
        shootBullet.Play();
    }

    public void PlayPlayerWallExplosion()
    {
        playerWallExplosion.Play();
    }

    public void PlayBulletExplosion()
    {
        bulletExplosion.Play();
    }

    public void PlayPlayerSpawn()
    {
        spawnSound.Play();
    }

    public void ToogleThemeMusic()
    {
        isThemeMusicPlay = !isThemeMusicPlay;
        if (isThemeMusicPlay)
        {
            themeMusic.Play();
        } else
        {
            themeMusic.Stop();
        }

    }

    public void ToggleBulletAura(bool isPlay)
    {
        if(isPlay)
        {
            bulletAura.Play();
        } else
        {
            bulletAura.Stop();
        }
    }
}
