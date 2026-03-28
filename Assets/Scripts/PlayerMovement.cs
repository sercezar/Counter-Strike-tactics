using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerCamera;

    public float normalSpeed = 5f;
    public float crouchSpeed = 2f;
    public float sneakSpeed = 1.5f;

    public float normalHeight = 2f;
    public float crouchHeight = 1f;

    public float cameraNormalY = 1.8f;
    public float cameraCrouchY = 1.0f;
    public float crouchSmooth = 8f;

    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Dźwięk kroków")]
    public AudioSource audioSource; // ustawiony clip i loop = true
    public float crouchVolume = 0.5f;
    public float normalVolume = 1f;

    private float currentSpeed;
    private Vector3 velocity;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (audioSource != null)
        {
            audioSource.loop = true; // krok w pętli
            audioSource.playOnAwake = false;
            audioSource.volume = normalVolume;
        }
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        bool isSneaking = Input.GetKey(KeyCode.LeftShift);

        // Ustaw prędkość
        currentSpeed = normalSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        if (isSneaking) currentSpeed = sneakSpeed;

        // KUCANIE
        if (isCrouching)
        {
            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 2f, 0);
        }
        else
        {
            controller.height = normalHeight;
            controller.center = new Vector3(0, normalHeight / 2f, 0);
        }

        // RUCH
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // SKOK + GRAWITACJA
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // PŁYNNE KUCANIE KAMERY
        float targetY = isCrouching ? cameraCrouchY : cameraNormalY;
        Vector3 camPos = playerCamera.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetY, crouchSmooth * Time.deltaTime);
        playerCamera.localPosition = camPos;

        // 🔊 Dźwięk kroków
        HandleFootsteps(x, z, isCrouching, isSneaking);
    }

    void HandleFootsteps(float x, float z, bool isCrouching, bool isSneaking)
    {
        if (controller.isGrounded && (x != 0 || z != 0) && !isSneaking)
        {
            audioSource.volume = isCrouching ? crouchVolume : normalVolume;
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }
}