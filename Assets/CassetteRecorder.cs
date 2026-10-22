using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CassetteRecorder : MonoBehaviour
{
    [Header("References")]
    public EightDirectionLook cameraLook;
    public Image progressBarFill;
    public GameObject progressBarRoot;

    [Header("Settings")]
    public Key recordKey = Key.Space;
    public float recordingDuration = 8f;

    public float Progress => recordingProgress;   
    public bool IsFinished => isFinished;

    [Header("Audio")]
    public AudioSource recorderAudio;     
    public AudioClip recordingLoopClip;   

    private bool isRecording = false;
    private float recordingProgress = 0f;
    private bool isFinished = false;  

    void Start()
    {
        if (progressBarRoot != null)
            progressBarRoot.SetActive(false);
    }

    void Update()
    {
        if (cameraLook == null) return;

        // Hide bar if not looking down
        if (!cameraLook.IsLookingDown)
        {
            if (progressBarRoot != null && progressBarRoot.activeSelf)
                progressBarRoot.SetActive(false);

            StopRecordingAudio();
            isRecording = false;
            return;
        }

        // Show the bar when looking down
        if (progressBarRoot != null && !progressBarRoot.activeSelf)
            progressBarRoot.SetActive(true);

        if (isFinished)
        {
            progressBarFill.fillAmount = 1f;
            StopRecordingAudio();
            return;
        }

        // Toggle recording
        if (Keyboard.current[recordKey].wasPressedThisFrame)
        {
            isRecording = !isRecording;

            if (isRecording)
                StartRecordingAudio();
            else
                StopRecordingAudio();
        }

        // Fill progress
        if (isRecording)
        {
            recordingProgress += Time.deltaTime / recordingDuration;
            recordingProgress = Mathf.Clamp01(recordingProgress);

            if (recordingProgress >= 1f)
            {
                isRecording = false;
                isFinished = true;
                StopRecordingAudio();
                Debug.Log("Warning fully recorded.");
            }
        }

        if (progressBarFill != null)
            progressBarFill.fillAmount = recordingProgress;
    }

    void StartRecordingAudio()
    {
        if (recorderAudio == null || recordingLoopClip == null) return;

        recorderAudio.clip = recordingLoopClip;
        recorderAudio.loop = true;
        recorderAudio.Play();
    }

    void StopRecordingAudio()
    {
        if (recorderAudio != null && recorderAudio.isPlaying)
            recorderAudio.Stop();
    }
}