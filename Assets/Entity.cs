using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class DoorEntity : MonoBehaviour
{
    [Header("Setup")]
    public Transform[] doors;
    public Transform playerCamera;
    public AudioSource appearSound;
    public CassetteRecorder cassette;

    [Header("Timing")]
    public float baseAppearDuration = 5f;
    public float baseHideTimeMin = 4f;
    public float baseHideTimeMax = 8f;

    [Header("Flashlight")]
    public float flashlightAngle = 20f;
    public KeyCode flashKey = KeyCode.Mouse0;

    [Header("Audio Fade")]
    public float fadeInTime = 0.6f;
    public float fadeOutTime = 0.8f;

    [Header("Jumpscare")]
    public AudioSource jumpscareAudio;       
    public AudioClip jumpscareClip;
    public float lungeSpeed = 18f;           
    public float lungeDistance = 1.1f;       
    public GameObject gameOverScreen;        

    private int currentDoorIndex = -1;
    private bool isVisible = false;
    private bool isLunging = false;
    private Vector3 hiddenPosition = new Vector3(0, -25f, 0);
    private Coroutine fadeCoroutine;

    void Start()
    {
        transform.position = hiddenPosition;
        if (appearSound) appearSound.volume = 0f;
        if (gameOverScreen) gameOverScreen.SetActive(false);

        StartCoroutine(EntityLoop());
    }

    IEnumerator EntityLoop()
    {
        while (true)
        {
            if (isLunging) yield break; 

            float progress = cassette != null ? cassette.Progress : 0f;

            float hideMin = Mathf.Lerp(baseHideTimeMin, 1.2f, progress);
            float hideMax = Mathf.Lerp(baseHideTimeMax, 2.8f, progress);
            float appearTime = Mathf.Lerp(baseAppearDuration, 2.2f, progress);

            yield return new WaitForSeconds(Random.Range(hideMin, hideMax));

            currentDoorIndex = Random.Range(0, doors.Length);
            Transform door = doors[currentDoorIndex];

            transform.position = door.position;
            transform.rotation = door.rotation;
            isVisible = true;

            if (appearSound)
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeAudio(0f, 1f, fadeInTime));
                if (!appearSound.isPlaying) appearSound.Play();
            }

            float timer = 0f;
            while (timer < appearTime && isVisible)
            {
                timer += Time.deltaTime;

                if (IsBeingFlashed())
                {
                    Hide();
                    break;
                }
                yield return null;
            }

            if (isVisible)
            {
                StartCoroutine(Jumpscare());
                yield break;
            }
        }
    }

    IEnumerator Jumpscare()
    {
        isLunging = true;
        isVisible = false;

        if (appearSound) appearSound.Stop();

        if (jumpscareAudio && jumpscareClip)
        {
            jumpscareAudio.PlayOneShot(jumpscareClip);
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = playerCamera.position + playerCamera.forward * lungeDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * (lungeSpeed / 10f);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverScreen)
            gameOverScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    void Hide()
    {
        isVisible = false;

        if (appearSound)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutAndHide());
        }
        else
        {
            transform.position = hiddenPosition;
        }
    }

    IEnumerator FadeOutAndHide()
    {
        yield return StartCoroutine(FadeAudio(appearSound.volume, 0f, fadeOutTime));
        appearSound.Stop();
        transform.position = hiddenPosition;
    }

    IEnumerator FadeAudio(float from, float to, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            appearSound.volume = Mathf.Lerp(from, to, timer / duration);
            yield return null;
        }
        appearSound.volume = to;
    }

    bool IsBeingFlashed()
    {
        if (!Input.GetKey(flashKey)) return false;

        Vector3 dir = (transform.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, dir);
        return angle < flashlightAngle;
    }
}