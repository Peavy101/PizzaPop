using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 720f; // Degrees per second
    private Quaternion originalRotation; // Store the original rotation
    private Quaternion targetRotation; // The desired rotation
    private Vector3 originalPosition; // Store the original position for space key functionality

    private enum RotationState { Idle, Rotating, SnappingBack }
    private RotationState currentState = RotationState.Idle; // Track the current rotation state

    // Track key presses for combination
    private bool upPressed = false;
    private bool downPressed = false;
    private bool leftPressed = false;
    private bool rightPressed = false;

    void Start()
    {
        // Store the original rotation and position of the object at the start
        originalRotation = transform.rotation;
        targetRotation = originalRotation;
        originalPosition = transform.position;
    }

    void Update()
    {
        // Process input only when keys are pressed once
        if (Input.GetKeyDown(KeyCode.UpArrow) && !upPressed)
        {
            upPressed = true;
            HandleRotation();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && !downPressed)
        {
            downPressed = true;
            HandleRotation();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !leftPressed)
        {
            leftPressed = true;
            HandleRotation();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && !rightPressed)
        {
            rightPressed = true;
            HandleRotation();
        }

        switch (currentState)
        {
            case RotationState.Rotating:
                // Smoothly rotate toward the target rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Stop rotating when the target is reached
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.01f) // Lower threshold for smoother stop
                {
                    transform.rotation = targetRotation; // Snap to target
                    currentState = RotationState.SnappingBack; // Start the snap-back process
                }
                break;

            case RotationState.SnappingBack:
                // If snapping back to original position
                transform.rotation = Quaternion.RotateTowards(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);

                // Stop snapping back when the original position is reached
                if (Quaternion.Angle(transform.rotation, originalRotation) < 0.01f) // Lower threshold for smoother stop
                {
                    transform.rotation = originalRotation; // Snap to original
                    currentState = RotationState.Idle; // Go back to idle state
                }
                break;
        }

        // Reset key flags after rotation is triggered
        if (Input.GetKeyUp(KeyCode.UpArrow)) { upPressed = false; }
        if (Input.GetKeyUp(KeyCode.DownArrow)) { downPressed = false; }
        if (Input.GetKeyUp(KeyCode.LeftArrow)) { leftPressed = false; }
        if (Input.GetKeyUp(KeyCode.RightArrow)) { rightPressed = false; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Move up a little and return back (no rotation)
            StartCoroutine(MoveUpAndDown());
        }
    }

    void HandleRotation()
    {
        // Handle rotation based on key presses
        if (upPressed && rightPressed)
        {
            targetRotation = transform.rotation * Quaternion.Euler(45, 0, -45); // Up + Right
            currentState = RotationState.Rotating;
        }
        else if (upPressed && leftPressed)
        {
            targetRotation = transform.rotation * Quaternion.Euler(45, 0, 45); // Up + Left
            currentState = RotationState.Rotating;
        }
        else if (downPressed && rightPressed)
        {
            targetRotation = transform.rotation * Quaternion.Euler(-45, 0, -45); // Down + Right
            currentState = RotationState.Rotating;
        }
        else if (downPressed && leftPressed)
        {
            targetRotation = transform.rotation * Quaternion.Euler(-45, 0, 45); // Down + Left
            currentState = RotationState.Rotating;
        }
        else if (upPressed)
        {
            targetRotation = transform.rotation * Quaternion.Euler(45, 0, 0); // Rotate Up
            currentState = RotationState.Rotating;
        }
        else if (downPressed)
        {
            targetRotation = transform.rotation * Quaternion.Euler(-45, 0, 0); // Rotate Down
            currentState = RotationState.Rotating;
        }
        else if (rightPressed)
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, 0, -45); // Rotate Right
            currentState = RotationState.Rotating;
        }
        else if (leftPressed)
        {
            targetRotation = transform.rotation * Quaternion.Euler(0, 0, 45); // Rotate Left
            currentState = RotationState.Rotating;
        }
    }

    // Coroutine to move the object up a little and return back
    private IEnumerator MoveUpAndDown()
    {
        Vector3 targetPosition = originalPosition + Vector3.up * 0.2f; // Move up by 1 unit
        float timeElapsed = 0f;
        float moveDuration = 0.1f; // Duration for moving up and down

        // Move up
        while (timeElapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        // Move back down to the original position
        timeElapsed = 0f;
        while (timeElapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(targetPosition, originalPosition, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }
}
