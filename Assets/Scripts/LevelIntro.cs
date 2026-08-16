using UnityEngine;
using System.Collections;

public class LevelIntro : MonoBehaviour
{
    public Camera mainCamera;
    public CameraFollow2D cameraFollow;
    [Header("Enemy Camera Positions")]
    public Transform[] introEnemyPositions;

    [Header("Camera Movement")]
    public float moveToEnemyDuration = 2f;
    public float waitAtEnemyPosition = 3f;
    public float moveBackDuration = 2f;

    public static bool gameplayStarted = false;

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        gameplayStarted = false;
        cameraFollow.followEnabled = false;
        Vector3 playerPosition = mainCamera.transform.position;
        Quaternion playerRotation = mainCamera.transform.rotation;

        foreach (Transform position in introEnemyPositions)
        {
            yield return StartCoroutine(MoveCameraTo(position, moveToEnemyDuration));
            yield return new WaitForSeconds(waitAtEnemyPosition);
        }

        yield return StartCoroutine(MoveCameraBack(playerPosition,playerRotation,moveBackDuration));

        cameraFollow.followEnabled = true;
        gameplayStarted = true;
        Debug.Log("INTRO FINISHED");
    }

    IEnumerator MoveCameraTo(Transform target, float duration)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            mainCamera.transform.position = Vector3.Lerp(startPosition, target.position, t);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, target.rotation, t);

            yield return null;
        }

        mainCamera.transform.position = target.position;
        mainCamera.transform.rotation = target.rotation;
    }

    IEnumerator MoveCameraBack(Vector3 targetPosition,Quaternion targetRotation,float duration)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }
}