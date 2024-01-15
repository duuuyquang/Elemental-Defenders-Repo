using UnityEngine;
using UnityEngine.UI;

public class SoundController: MonoBehaviour
{
    public const int TYPE_STRONGER  = 1;
    public const int TYPE_SAME      = 2;
    public const int TYPE_WEAKER    = 3;

    public const int TYPE_PLAYER_WALL   = 1;
    public const int TYPE_ENEMY_WALL    = 2;

    public const int THEME_MUSIC_NONE       = 0;
    public const int THEME_MUSIC_UNIVERSE   = 1;
    public const int THEME_MUSIC_BATTLE     = 2;

    public AudioSource typeStronger;
    public AudioSource typeSame;
    public AudioSource typeWeaker;
    public AudioSource playerWallExplosion;
    public AudioSource bulletExplosion;
    public AudioSource bulletAura;
    public AudioSource shootBullet;
    public AudioSource spawnSound;
    public AudioSource buttonSound;
    public AudioSource[] themeSounds;
    public AudioSource backgroundSound;
    public AudioSource startSound;
    public AudioSource countSound;
    public AudioSource healingSound;

    private bool isBackgroundSoundPlay = false;
    private int currentThemeSoundIndexedByMode = -1;

    public void Start()
    {
        LoadDefaultData();

        HandleCurrentThemeSoundStatus();

        GameMenuManager gameMenuManager = GameObject.Find("GameMenuManager").GetComponent<GameMenuManager>();
        gameMenuManager.SetCurrentThemeDropdownVal(currentThemeSoundIndexedByMode);
        if (isBackgroundSoundPlay)
        {
            backgroundSound.Play();
        }
    }

    public void LoadDefaultData()
    {
        isBackgroundSoundPlay = PlayerSetting.toggleBackGroundSound ? PlayerSetting.toggleBackGroundSound : false;
        currentThemeSoundIndexedByMode = PlayerSetting.currentThemeSoundIndex == -1 ? THEME_MUSIC_UNIVERSE : PlayerSetting.currentThemeSoundIndex;
    }

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

    public void SetThemeSoundDropdown(int mode)
    {
        currentThemeSoundIndexedByMode = mode;
        PlayerSetting.currentThemeSoundIndex = currentThemeSoundIndexedByMode;
        HandleCurrentThemeSoundStatus();
    }

    public void HandleCurrentThemeSoundStatus()
    {
        StopAllThemeSounds();
        if (currentThemeSoundIndexedByMode != THEME_MUSIC_NONE)
        {
            themeSounds[currentThemeSoundIndexedByMode].Play();
        }
    }

    private void StopAllThemeSounds()
    {
        foreach ( AudioSource themeSound in themeSounds)
        {
            if(themeSound != null)
            {
                themeSound.Stop();
            }
        }
    }

    public void ToggleBackgroundSound()
    {
        isBackgroundSoundPlay = !isBackgroundSoundPlay;
        if (isBackgroundSoundPlay)
        {
            backgroundSound.Play();
        }
        else
        {
            backgroundSound.Stop();
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

    public void PlayButtonClick()
    {
        buttonSound.Play();
    }

    public void PlayStartSound()
    {
        startSound.Play();
    }

    public void PlayCountSound()
    {
        countSound.Play();
    }

    public void PlayHealingSound()
    {
        healingSound.Play();
    }
}
