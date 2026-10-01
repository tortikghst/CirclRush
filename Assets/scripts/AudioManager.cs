using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Источники звука")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Музыка")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip bossMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    [Header("Игрок")]
    public AudioClip playerHurt;
    public AudioClip playerDeath;
    public AudioClip coinPickup;
    public AudioClip expPickup;
    public AudioClip heal;
    public AudioClip magnetPickup;
    public AudioClip levelUp;

    [Header("Навыки (12 шт)")]
    public AudioClip orbShoot;           // Орб
    public AudioClip blackHole;          // Чёрная дыра
    public AudioClip bouncingOrb;        // Скачущая сфера
    public AudioClip giftDrop;           // Небесный подарок
    public AudioClip spiralLaser;        // Спиральный лазер
    public AudioClip lungeSwing;         // Круговой выпад
    public AudioClip diskSpin;           // Разящие диски
    public AudioClip shockwave;          // Ударная волна
    public AudioClip ufoBeam;            // НЛО-луч
    public AudioClip waterSplash;        // Капли на воде
    public AudioClip meleeSwing;         // Круговая атака (меч)
    public AudioClip shieldActivate;     // Щит
    public AudioClip skillActivate;      // Общий звук активации

    [Header("Враги")]
    public AudioClip enemySpawn;
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioClip bossSpawn;
    public AudioClip bossAttack;
    public AudioClip bossDeath;

    [Header("UI")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    public AudioClip purchase;
    public AudioClip error;
    public AudioClip shopOpen;

    [Header("Окружение")]
    public AudioClip boxBreak;
    public AudioClip magnetActivate;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
            
        musicSource.loop = true;
        musicSource.volume = 0.5f;
        sfxSource.volume = 0.8f;
    }

    // ========== ОСНОВНЫЕ МЕТОДЫ ==========
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.isPlaying && musicSource.clip == clip) return;
        
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

    // ========== УДОБНЫЕ МЕТОДЫ ДЛЯ ИГРОКА ==========
    public void PlayCoinPickup() => PlaySFX(coinPickup, 0.6f);
    public void PlayExpPickup() => PlaySFX(expPickup, 0.7f);
    public void PlayHeal() => PlaySFX(heal, 0.8f);
    public void PlayMagnetPickup() => PlaySFX(magnetPickup, 0.7f);
    public void PlayLevelUp() => PlaySFX(levelUp, 1f);
    public void PlayPlayerHurt() => PlaySFX(playerHurt, 0.8f);
    public void PlayPlayerDeath() => PlaySFX(playerDeath, 1f);

    // ========== УДОБНЫЕ МЕТОДЫ ДЛЯ НАВЫКОВ (12 шт) ==========
    public void PlayOrbShoot() => PlaySFX(orbShoot, 0.7f);
    public void PlayBlackHole() => PlaySFX(blackHole, 0.9f);
    public void PlayBouncingOrb() => PlaySFX(bouncingOrb, 0.7f);
    public void PlayGiftDrop() => PlaySFX(giftDrop, 0.8f);
    public void PlaySpiralLaser() => PlaySFX(spiralLaser, 0.6f);
    public void PlayLungeSwing() => PlaySFX(lungeSwing, 0.8f);
    public void PlayDiskSpin() => PlaySFX(diskSpin, 0.5f);
    public void PlayShockwave() => PlaySFX(shockwave, 1f);
    public void PlayUfoBeam() => PlaySFX(ufoBeam, 0.7f);
    public void PlayWaterSplash() => PlaySFX(waterSplash, 0.6f);
    public void PlayMeleeSwing() => PlaySFX(meleeSwing, 0.8f);
    public void PlayShieldActivate() => PlaySFX(shieldActivate, 0.6f);
    public void PlaySkillActivate() => PlaySFX(skillActivate, 0.7f);

    // ========== УДОБНЫЕ МЕТОДЫ ДЛЯ ВРАГОВ ==========
    public void PlayEnemySpawn() => PlaySFX(enemySpawn, 0.6f);
    public void PlayEnemyHit() => PlaySFX(enemyHit, 0.7f);
    public void PlayEnemyDeath() => PlaySFX(enemyDeath, 0.8f);
    public void PlayBossSpawn() => PlaySFX(bossSpawn, 1f);
    public void PlayBossAttack() => PlaySFX(bossAttack, 0.9f);
    public void PlayBossDeath() => PlaySFX(bossDeath, 1f);

    // ========== УДОБНЫЕ МЕТОДЫ ДЛЯ UI ==========
    public void PlayButtonClick() => PlaySFX(buttonClick, 0.6f);
    public void PlayButtonHover() => PlaySFX(buttonHover, 0.4f);
    public void PlayPurchase() => PlaySFX(purchase, 0.7f);
    public void PlayError() => PlaySFX(error, 0.8f);
    public void PlayShopOpen() => PlaySFX(shopOpen, 0.7f);

    // ========== УДОБНЫЕ МЕТОДЫ ДЛЯ ОКРУЖЕНИЯ ==========
    public void PlayBoxBreak() => PlaySFX(boxBreak, 0.7f);
    public void PlayMagnetActivate() => PlaySFX(magnetActivate, 0.7f);
}