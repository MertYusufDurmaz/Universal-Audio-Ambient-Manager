using System.Collections;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance { get; private set; }

    [Header("Audio Sources (Otomatik Oluşturulur)")]
    private AudioSource sfxSource;        // Anlık kısa SFX'ler (Kapı, adım, buton)
    private AudioSource ambientSource;    // Arka plan müziği
    private AudioSource randomSfxSource;  // Rastgele korku sesleri
    private AudioSource earthquakeSource; // Deprem
    private AudioSource flashlightSource; // Glitch gibi uzun sesler

    [Header("Ambient / Background Music")]
    public AudioClip ambientClip;
    [Range(0f, 1f)] public float ambientVolume = 0.007f;

    [Header("Random Horror Events")]
    [Tooltip("Buraya fısıltı, gıcırtı, ani sesler ekle")]
    public AudioClip[] randomHorrorClips;
    public float minRandomTime = 15f;
    public float maxRandomTime = 45f;
    [Range(0f, 1f)] public float randomHorrorVolume = 0.8f;

    [Header("Earthquake Sound")]
    public AudioClip earthquakeSound;

    [Header("Footstep Settings")]
    public AudioClip[] footstepSounds;
    public float stepInterval = 0.6f;
    private float stepTimer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Tüm AudioSource'ları aynı objeye yığıp karıştırmak yerine, 
        // Manager'ın altına temiz Child (Çocuk) objeler olarak oluşturuyoruz.
        sfxSource = CreateAudioSource("SFX_Source", false, 1f);
        ambientSource = CreateAudioSource("Ambient_Source", true, 0f);
        randomSfxSource = CreateAudioSource("RandomHorror_Source", false, 0f);
        earthquakeSource = CreateAudioSource("Earthquake_Source", true, 0f);
        flashlightSource = CreateAudioSource("Flashlight_Source", false, 1f);
        
        ambientSource.volume = ambientVolume;
        randomSfxSource.volume = randomHorrorVolume;
    }

    private AudioSource CreateAudioSource(string name, bool isLooping, float spatialBlend)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(transform);
        AudioSource source = child.AddComponent<AudioSource>();
        source.loop = isLooping;
        source.playOnAwake = false;
        source.spatialBlend = spatialBlend;
        return source;
    }

    private void Start()
    {
        PlayAmbientMusic();
        
        if (randomHorrorClips != null && randomHorrorClips.Length > 0)
        {
            StartCoroutine(RandomHorrorRoutine());
        }
    }

    // =========================================================
    // EVRENSEL SFX METOTLARI (Eski spesifik metotların yerini aldı)
    // =========================================================
    
    /// <summary>
    /// UnityEvent'ler üzerinden (Kasa, Kapı, Fener) çağrılacak evrensel metot.
    /// Ses klibini doğrudan etkileşimli objenin Inspector'ından yollayın.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // =========================================================
    // RANDOM HORROR MANTIĞI (Update yerine Coroutine kullanıldı)
    // =========================================================
    private IEnumerator RandomHorrorRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minRandomTime, maxRandomTime);
            yield return new WaitForSeconds(waitTime);

            if (randomHorrorClips.Length > 0 && !randomSfxSource.isPlaying)
            {
                int index = Random.Range(0, randomHorrorClips.Length);
                randomSfxSource.PlayOneShot(randomHorrorClips[index], randomHorrorVolume);
            }
        }
    }

    // =========================================================
    // MÜZİK VE AMBİYANS
    // =========================================================
    public void PlayAmbientMusic()
    {
        if (ambientClip != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.Play();
        }
    }

    public void StopAmbientMusic() { if (ambientSource.isPlaying) ambientSource.Stop(); }
    public void SetAmbientVolume(float vol) { ambientVolume = vol; ambientSource.volume = ambientVolume; }

    // =========================================================
    // DEPREM
    // =========================================================
    public void PlayEarthquake(float volume = 1f)
    {
        if (earthquakeSound == null) return;
        earthquakeSource.clip = earthquakeSound;
        earthquakeSource.volume = volume;
        if (!earthquakeSource.isPlaying) earthquakeSource.Play();
    }

    public void StopEarthquake() { if (earthquakeSource.isPlaying) earthquakeSource.Stop(); }

    // =========================================================
    // ADIM SESLERİ (Player Movement üzerinden çağrılır)
    // =========================================================
    public void HandleFootsteps(bool isMoving, bool isGrounded, bool isSneaking)
    {
        if (isSneaking || !isGrounded || !isMoving) { stepTimer = 0f; return; }
        
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f) 
        { 
            if (footstepSounds != null && footstepSounds.Length > 0)
            {
                sfxSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
            }
            stepTimer = stepInterval; 
        }
    }

    // =========================================================
    // FLASHLIGHT GLITCH (Uzun sürdüğü için özel kaynağı var)
    // =========================================================
    public void PlayFlashlightGlitch(AudioClip glitchClip)
    {
        if (glitchClip != null && !flashlightSource.isPlaying)
        {
            flashlightSource.clip = glitchClip;
            flashlightSource.volume = 0.5f;
            flashlightSource.Play();
        }
    }

    public void StopFlashlightGlitch()
    {
        if (flashlightSource.isPlaying)
        {
            flashlightSource.Stop();
        }
    }
}
