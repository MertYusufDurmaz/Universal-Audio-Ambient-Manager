using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance; // Singleton
    private AudioSource audioSource;     // Genel kısa SFX'ler

    // --- AMBIENT ---
    [Header("Ambient / Background Music")]
    public AudioClip ambientClip;
    [Range(0f, 1f)] public float ambientVolume = 0.007f;
    private AudioSource ambientSource;

    // --- RANDOM HORROR EVENTS (YENİ) ---
    [Header("Random Horror Events")]
    [Tooltip("Buraya fısıltı, gıcırtı, ani sesler ekle")]
    public AudioClip[] randomHorrorClips;
    [Tooltip("En az kaç saniye sonra ses çıksın?")]
    public float minRandomTime = 15f;
    [Tooltip("En fazla kaç saniye sonra ses çıksın?")]
    public float maxRandomTime = 45f;
    [Range(0f, 1f)] public float randomHorrorVolume = 0.8f;

    private AudioSource randomSfxSource; // Bu sesler için özel kaynak
    private float randomEventTimer;

    // --- EARTHQUAKE ---
    [Header("Earthquake Sound")]
    public AudioClip earthquakeSound;
    [HideInInspector] public AudioSource earthquakeSource;

    // --- FOOTSTEPS ---
    [Header("Footstep Settings")]
    public AudioClip[] footstepSounds;
    public float stepInterval = 0.6f;
    private float stepTimer;

    // --- DOOR / DRAWER / SAFE ---
    [Header("Door Sounds")]
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    [Header("Drawer Sounds")]
    public AudioClip drawerOpenSound;
    public AudioClip drawerCloseSound;
    public AudioClip bigDrawerOpenSound;
    public AudioClip bigDrawerCloseSound;

    [Header("Safe Sounds")]
    public AudioClip safeButtonPress;
    public AudioClip safeOpenning;
    public AudioClip safeError;

    // --- FLASHLIGHT SOUNDS ---
    [Header("Flashlight Sounds")]
    public AudioClip flashlightToggleSound;
    public AudioClip flashlightReloadSound;
    public AudioClip flashlightGlitchSound;

    // Glitch sesi uzun olduğu için onu kontrol edecek özel kaynak
    private AudioSource flashlightSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        earthquakeSource = gameObject.AddComponent<AudioSource>();
        earthquakeSource.loop = true;
        earthquakeSource.playOnAwake = false;
        earthquakeSource.spatialBlend = 0f;
        earthquakeSource.volume = 1f;

        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        ambientSource.playOnAwake = false;
        ambientSource.spatialBlend = 0f;
        ambientSource.volume = ambientVolume;

        // Flashlight Glitch Kaynağı
        flashlightSource = gameObject.AddComponent<AudioSource>();
        flashlightSource.loop = false;
        flashlightSource.playOnAwake = false;
        flashlightSource.spatialBlend = 0f;

        // YENİ: Random Horror Kaynağı
        randomSfxSource = gameObject.AddComponent<AudioSource>();
        randomSfxSource.loop = false;
        randomSfxSource.playOnAwake = false;
        randomSfxSource.spatialBlend = 0f; // 2D Ses (Kafanın içinde gibi)
        randomSfxSource.volume = randomHorrorVolume;
    }

    private void Start()
    {
        PlayAmbientMusic();
        ResetRandomTimer(); // Zamanlayıcıyı başlat
    }

    // YENİ: Süre sayımı için Update eklendi
    private void Update()
    {
        if (randomHorrorClips != null && randomHorrorClips.Length > 0)
        {
            randomEventTimer -= Time.deltaTime;

            if (randomEventTimer <= 0f)
            {
                PlayRandomHorrorSound();
                ResetRandomTimer();
            }
        }
    }

    // --- RANDOM HORROR MANTIĞI ---
    private void ResetRandomTimer()
    {
        // Belirlenen iki süre arasında rastgele bir zaman seç
        randomEventTimer = Random.Range(minRandomTime, maxRandomTime);
    }

    private void PlayRandomHorrorSound()
    {
        if (randomHorrorClips.Length == 0) return;

        // Rastgele bir ses seç
        int index = Random.Range(0, randomHorrorClips.Length);
        AudioClip clip = randomHorrorClips[index];

        // Sesi çal (volume ayarını dikkate alarak)
        if (clip != null && randomSfxSource != null)
        {
            // Eğer o an başka bir korku sesi çalmıyorsa çal
            if (!randomSfxSource.isPlaying)
            {
                randomSfxSource.PlayOneShot(clip, randomHorrorVolume);
            }
        }
    }

    // --- MÜZİK ---
    public void PlayAmbientMusic()
    {
        if (ambientClip != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.volume = ambientVolume;
            ambientSource.Play();
        }
    }

    public void StopAmbientMusic() { if (ambientSource.isPlaying) ambientSource.Stop(); }
    public void SetAmbientVolume(float vol) { ambientVolume = vol; ambientSource.volume = ambientVolume; }

    // --- DEPREM ---
    public void PlayEarthquake(float volume = 1f)
    {
        if (earthquakeSound == null) return;
        earthquakeSource.clip = earthquakeSound;
        earthquakeSource.volume = volume;
        if (!earthquakeSource.isPlaying) earthquakeSource.Play();
    }

    public void StopEarthquake() { if (earthquakeSource.isPlaying) earthquakeSource.Stop(); earthquakeSource.volume = 1f; }

    // --- ADIM ---
    public void HandleFootsteps(bool isMoving, bool isGrounded, bool isSneaking)
    {
        if (isSneaking || !isGrounded || !isMoving) { stepTimer = 0f; return; }
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f) { PlayFootstep(); stepTimer = stepInterval; }
    }

    private void PlayFootstep()
    {
        if (footstepSounds != null && footstepSounds.Length > 0)
            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
    }

    // --- KAPI / DOLAP / KASA ---
    public void PlayDoorOpen() { if (doorOpenSound != null) audioSource.PlayOneShot(doorOpenSound); }
    public void PlayDoorClose() { if (doorCloseSound != null) audioSource.PlayOneShot(doorCloseSound); }
    public void PlayDrawerOpen() { if (drawerOpenSound != null) audioSource.PlayOneShot(drawerOpenSound); }
    public void PlayDrawerClose() { if (drawerCloseSound != null) audioSource.PlayOneShot(drawerCloseSound); }
    public void PlayBigDrawerOpen() { if (bigDrawerOpenSound != null) audioSource.PlayOneShot(bigDrawerOpenSound); }
    public void PlayBigDrawerClose() { if (bigDrawerCloseSound != null) audioSource.PlayOneShot(bigDrawerCloseSound); }
    public void PlayButtonPressed() { if (safeButtonPress != null) audioSource.PlayOneShot(safeButtonPress); }
    public void SafeErrorSound() { if (safeError != null) audioSource.PlayOneShot(safeError); }
    public void safeOpenSound() { if (safeOpenning != null) audioSource.PlayOneShot(safeOpenning); }

    // --- FLASHLIGHT FONKSİYONLARI ---
    public void PlayFlashlightToggle()
    {
        if (flashlightToggleSound != null) audioSource.PlayOneShot(flashlightToggleSound);
    }

    public void PlayFlashlightReload()
    {
        if (flashlightReloadSound != null) audioSource.PlayOneShot(flashlightReloadSound);
    }

    // Glitch Sesi Mantığı
    public void PlayFlashlightGlitch()
    {
        if (flashlightGlitchSound != null)
        {
            if (!flashlightSource.isPlaying)
            {
                flashlightSource.clip = flashlightGlitchSound;
                flashlightSource.volume = 0.5f;
                flashlightSource.Play();
            }
        }
    }

    // Sesi anında kesmek için fonksiyon
    public void StopFlashlightGlitch()
    {
        if (flashlightSource.isPlaying)
        {
            flashlightSource.Stop();
        }
    }
}