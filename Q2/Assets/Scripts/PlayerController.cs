using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement & Physics")]
    public float baseSpeed = 10f;
    public float boostedSpeed = 20f;
    public float jumpForce = 5f;

    [Header("References")]
    public TextMeshProUGUI countText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI powerUpText;
    public GameObject powerUpCapsule;

    private Rigidbody rb;
    private Renderer rend;
    private float movementX;
    private float movementY;
    private float currentSpeed;
    private int count;
    private int totalPickups;

    private bool gameStarted = false;
    private bool hasPowerUp = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();

        currentSpeed = baseSpeed;
        count = 0;

        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;

        rb.useGravity = false;
        rb.isKinematic = true;

        SetCountText();
        if (winText != null) winText.gameObject.SetActive(false);
        if (powerUpText != null) powerUpText.gameObject.SetActive(false);
        if (powerUpCapsule != null) powerUpCapsule.SetActive(false);
    }

    public void StartGame()
    {
        gameStarted = true;
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    void OnMove(InputValue movementValue)
    {
        if (!gameStarted) return;

        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void Update()
    {
        if (!gameStarted) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (!gameStarted) return;

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * currentSpeed);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SpecialWall"))
        {
            if (hasPowerUp)
            {
                Destroy(collision.gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();

            if (count >= totalPickups && powerUpCapsule != null)
            {
                powerUpCapsule.SetActive(true);
            }
        }

        if (other.gameObject.CompareTag("PowerUp"))
        {
            other.gameObject.SetActive(false);
            hasPowerUp = true;
            currentSpeed = boostedSpeed;

            if (rend != null)
            {
                rend.material.color = Color.red;
            }

            if (powerUpText != null)
            {
                powerUpText.text = "Power-Up Active! (Speed Boost)";
                powerUpText.gameObject.SetActive(true);
            }
        }

        if (other.gameObject.CompareTag("Goal"))
        {
            if (winText != null)
            {
                winText.text = "You win!";
                winText.gameObject.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }

    void SetCountText()
    {
        if (countText != null)
        {
            countText.text = "Count: " + count.ToString();
        }
    }
}