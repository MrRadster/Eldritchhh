using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;
    public KeyCode flashKey = KeyCode.Mouse0;

    [Header("Audio")]
    public AudioSource flashlightAudio;   
    public AudioClip turnOnClip;
    public AudioClip turnOffClip;

    private bool wasOn = false;

    void Update()
    {
        bool isOn = Input.GetKey(flashKey);

        if (flashlight != null)
            flashlight.enabled = isOn;

        if (isOn && !wasOn)
        {
            if (flashlightAudio && turnOnClip)
                flashlightAudio.PlayOneShot(turnOnClip);
        }
        else if (!isOn && wasOn)
        {
            if (flashlightAudio && turnOffClip)
                flashlightAudio.PlayOneShot(turnOffClip);
        }

        wasOn = isOn;
    }
}