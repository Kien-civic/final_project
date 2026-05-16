using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    [Header("UI Settings")]
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI scoreText; // assign ScoreText in the inspector

    private int currentScore = 100; // starting score
    private bool isCountingPenalty = false; // prevent repeated penalty during a single collision
    public GameObject winPanel;
    public GameObject losePanel; // assign LosePanel in the inspector
    private bool isGameOver = false;
    private bool isFinished = false;

    void Start()
    {
        UpdateScoreDisplay();

        if (warningText != null)
            warningText.gameObject.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        Time.timeScale = 1f; // ensure time is running when scene starts/restarts
    }

    void Update()
    {
        if (isGameOver) return;

        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // Move relative to the car's local forward
        transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isGameOver) return;

        // Check collisions for traffic violations
        if (collision.gameObject.CompareTag("Curb") || collision.gameObject.CompareTag("Median_Strip") || collision.gameObject.CompareTag("House"))
        {
            Debug.Log("VI PHẠM: Xe đã va chạm với " + collision.gameObject.name);

            if (warningText != null)
            {
                StopAllCoroutines();
                StartCoroutine(ShowWarningAndDeductScore());
            }
        }
    }

void OnTriggerEnter(Collider other)
    {
        // Finish trigger
        if (other.CompareTag("Finish") && !isFinished)
        {
            LevelManager lm = FindObjectOfType<LevelManager>();

            // Chỉ win khi hoàn thành cả 2 checkpoint
            if (lm != null && lm.AllCheckpointsCompleted())
            {
                isFinished = true;
                ShowWinScreen();
            }
            else
            {
                Debug.Log("Bạn chưa hoàn thành đủ checkpoint!");
            }
        }
    }


    void ShowWinScreen()
    {
        Debug.Log("LEVEL COMPLETE!");
        isGameOver = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // stop gameplay
        moveSpeed = 0f;
        turnSpeed = 0f;
        Time.timeScale = 0f;
    }

    IEnumerator ShowWarningAndDeductScore()
    {
        if (!isCountingPenalty)
        {
            isCountingPenalty = true;
            currentScore -= 10;
            if (currentScore <= 0)
            {
                currentScore = 0;
                UpdateScoreDisplay();
                GameOver();
                yield break;
            }
            UpdateScoreDisplay();
        }

        if (warningText != null && currentScore > 0)
        {
            warningText.text = "CẢNH BÁO: VA CHẠM VẬT CẢN (-10 ĐIỂM)";
            warningText.gameObject.SetActive(true);

            yield return new WaitForSeconds(2f);

            warningText.gameObject.SetActive(false);
        }

        isCountingPenalty = false;
    }

    void GameOver()
    {
        isGameOver = true;
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; // pause game
    }

    public void RestartGame() // assign this to a UI Restart button
    {
        Time.timeScale = 1f; // restore time before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "ĐIỂM: " + currentScore;
            scoreText.color = (currentScore <= 50) ? Color.red : Color.white;
        }
    }
}
