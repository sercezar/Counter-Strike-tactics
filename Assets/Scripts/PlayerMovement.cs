using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Head + Camera")]
    public Transform head;          // pusty obiekt (głowa)
    public Transform playerCamera;  // kamera (dziecko head)

    [Header("Ruch")]
    public float normalSpeed = 5f;
    public float crouchSpeed = 2f;
    public float sneakSpeed = 1.5f;

    [Header("Wysokość")]
    public float normalHeight = 2f;
    public float crouchHeight = 1f;

    public float headNormalY = 1.8f;
    public float headCrouchY = 1.0f;
    public float crouchSmooth = 8f;

    [Header("Skok")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Dźwięk kroków")]
    public AudioSource audioSource;
    public float normalVolume = 1f;
    public float crouchVolume = 0.5f;

    private float currentSpeed;
    private Vector3 velocity;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (playerCamera != null)
            playerCamera.localPosition = Vector3.zero;

        // audio setup
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        bool isSneaking = Input.GetKey(KeyCode.LeftShift);

        // prędkość
        currentSpeed = normalSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        if (isSneaking) currentSpeed = sneakSpeed;

        // KUCANIE (collider)
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

        // SKOK
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -0.1f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        float targetY = isCrouching ? headCrouchY : headNormalY;

        Vector3 headPos = head.localPosition;
        headPos.y = Mathf.Lerp(headPos.y, targetY, crouchSmooth * Time.deltaTime);
        head.localPosition = headPos;

        HandleFootsteps(x, z, isCrouching, isSneaking);
        print(velocity.y);
    }

    void HandleFootsteps(float x, float z, bool isCrouching, bool isSneaking)
    {
        if (controller.isGrounded && (x != 0 || z != 0) && !isSneaking)
        {
            audioSource.volume = isCrouching ? crouchVolume : normalVolume;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}