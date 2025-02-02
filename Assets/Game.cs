using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    public GameObject ringPrefab; // Reference to the ring prefab
    public TMP_Text gameStatusText; // TextMeshPro for game status
    public TMP_Text levelStatusText; // TextMeshPro for level display
    public GameObject nextLevelPrompt; // UI for "Press Space to Continue"
    public AudioSource winAudioSource; // Reference to the AudioSource for the win sound
    public AudioSource movementAudioSource; // Reference to the AudioSource for movement sound

    private int currentLevel = 1; // Current level of the game
    private int requiredRings = 1; // Number of rings needed to win the current level
    private int ringsTouching = 0; // Rings currently touching the post
    private float timeTouching = 0f; // Time rings have been touching the post
    private bool levelComplete = false; // Whether the current level is complete
    private bool gameComplete = false; // Flag to check if the game is completed

    // Predefined spawn positions for the rings
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-0.022f, 0.154f, -0.263f), // First ring
        new Vector3(-0.022f, 0.154f, 0.257f),  // Second ring
        new Vector3(-0.34f, 0.154f, -0.003f)   // Third ring
    };

    void Start()
    {
        UpdateLevelText();
        DisplayMessage("Get the ring on the post!");
        SpawnRings(currentLevel); // Spawn the first ring
    }

    void Update()
    {
        // Check for arrow keys or spacebar input
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.RightArrow)) && movementAudioSource != null)
        {
            if (!movementAudioSource.isPlaying)  // Play the sound only if it's not already playing
            {
                movementAudioSource.Play(); // Play the movement sound
            }
        }

        if (levelComplete && Input.GetKeyDown(KeyCode.Space))
        {
            ProceedToNextLevel();
        }

        if (gameComplete && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ring"))
        {
            ringsTouching++;
            if (ringsTouching >= requiredRings)
            {
                StartCoroutine(CheckWinCondition());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ring"))
        {
            ringsTouching--;
            timeTouching = 0f; // Reset the timer since the ring is no longer touching
        }
    }

    private IEnumerator CheckWinCondition()
    {
        // Ensure this doesn't execute if the level is already complete
        if (levelComplete)
            yield break;

        timeTouching = 0f;

        while (ringsTouching >= requiredRings)
        {
            timeTouching += Time.deltaTime;
            if (timeTouching >= 2f)
            {
                WinLevel(); // This will now only be called once
                yield break;
            }
            yield return null;
        }

        timeTouching = 0f; // Reset if condition breaks
    }
    private void WinLevel()
    {
        if (!levelComplete)
        {
            levelComplete = true;
            DisplayMessage($"Level {currentLevel} Complete! Press Space to Continue.");
            nextLevelPrompt.SetActive(true);

            // Play the win sound
            if (winAudioSource != null)
            {
                winAudioSource.Play(); // Play the sound
            }
            else
            {
                Debug.LogError("Win AudioSource is not assigned!");
            }
        }
    }

    private void ProceedToNextLevel()
    {
        currentLevel++;
        requiredRings = currentLevel; // Increase the number of rings needed
        ringsTouching = 0;
        timeTouching = 0f;
        levelComplete = false;

        UpdateLevelText();
        DisplayMessage($"Level {currentLevel} Started! Touch the post with {requiredRings} ring(s).");
        nextLevelPrompt.SetActive(false);

        if (currentLevel > 3)
        {
            gameComplete = true;
            DisplayMessage("You're the pizza master! Press Space to Restart.");
            nextLevelPrompt.SetActive(true);
        }
        else
        {
            SpawnRings(currentLevel); // Spawn rings for the next level
        }
    }

    private void RestartGame()
    {
        // Reset game state and go back to level 1
        currentLevel = 1;
        requiredRings = 1;
        ringsTouching = 0;
        timeTouching = 0f;
        levelComplete = false;
        gameComplete = false;

        UpdateLevelText();
        DisplayMessage("Get the ring on the post!");
        SpawnRings(currentLevel); // Spawn the first ring
    }

    private void SpawnRings(int level)
    {
        // Clear any existing rings if necessary
        foreach (GameObject ring in GameObject.FindGameObjectsWithTag("Ring"))
        {
            Destroy(ring);
        }

        // Spawn rings for the current level
        for (int i = 0; i < level && i < spawnPositions.Length; i++)
        {
            Instantiate(ringPrefab, spawnPositions[i], Quaternion.identity);
        }
    }

    private void UpdateLevelText()
    {
        if (levelStatusText != null)
        {
            levelStatusText.text = $"Level {currentLevel}";
        }
    }

    private void DisplayMessage(string message)
    {
        if (gameStatusText != null)
        {
            gameStatusText.text = message;
            Debug.Log("Message displayed: " + message); // Check if this is printed
        }
        else
        {
            Debug.LogError("gameStatusText is null!");
        }
    }
}
