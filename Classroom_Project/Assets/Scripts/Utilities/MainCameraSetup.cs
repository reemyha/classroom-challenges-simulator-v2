using UnityEngine;

/// <summary>
/// Ensures the Main Camera is properly set up when the scene starts.
/// Disables all other cameras and sets the main camera to the correct initial view.
/// </summary>
public class MainCameraSetup : MonoBehaviour
{
    [Header("Initial Camera Settings")]
    [Tooltip("Initial position of the camera when scene starts")]
    public Vector3 startPosition = new Vector3(0, 15, -10);

    [Tooltip("Initial rotation of the camera when scene starts (Euler angles)")]
    public Vector3 startRotation = new Vector3(30, 0, 0);

    [Header("Camera Configuration")]
    [Tooltip("Ensure this is the only active camera")]
    public bool disableOtherCameras = true;

    [Tooltip("Make sure this camera has the MainCamera tag")]
    public bool ensureMainCameraTag = true;

    [Tooltip("Ensure AudioListener is on this camera only")]
    public bool ensureSingleAudioListener = true;

    void Awake()
    {
        // This runs before Start(), ensuring camera is set up first
        SetupMainCamera();
    }

    void Start()
    {
        // Set initial position and rotation
        ResetToStartPosition();
    }

    /// <summary>
    /// Setup the main camera properly
    /// </summary>
    void SetupMainCamera()
    {
        Camera thisCamera = GetComponent<Camera>();

        if (thisCamera == null)
        {
            Debug.LogError("[MainCameraSetup] No Camera component found on this GameObject!");
            return;
        }

        // Ensure MainCamera tag
        if (ensureMainCameraTag && !gameObject.CompareTag("MainCamera"))
        {
            gameObject.tag = "MainCamera";
            Debug.Log($"[MainCameraSetup] Set camera tag to MainCamera");
        }

        // Ensure this camera is enabled
        thisCamera.enabled = true;

        // Set highest depth/priority
        thisCamera.depth = 0;

        // Disable all other cameras
        if (disableOtherCameras)
        {
            Camera[] allCameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in allCameras)
            {
                if (cam != thisCamera && cam.enabled)
                {
                    cam.enabled = false;
                    Debug.Log($"[MainCameraSetup] Disabled camera: {cam.gameObject.name}");
                }
            }
        }

        // Ensure only one AudioListener
        if (ensureSingleAudioListener)
        {
            AudioListener thisListener = GetComponent<AudioListener>();

            // Add AudioListener if missing
            if (thisListener == null)
            {
                thisListener = gameObject.AddComponent<AudioListener>();
                Debug.Log("[MainCameraSetup] Added AudioListener to main camera");
            }

            // Disable all other AudioListeners
            AudioListener[] allListeners = FindObjectsOfType<AudioListener>();
            foreach (AudioListener listener in allListeners)
            {
                if (listener != thisListener)
                {
                    listener.enabled = false;
                    Debug.Log($"[MainCameraSetup] Disabled AudioListener on: {listener.gameObject.name}");
                }
            }
        }

        Debug.Log("[MainCameraSetup] Main camera setup complete!");
    }

    /// <summary>
    /// Reset camera to starting position and rotation
    /// </summary>
    public void ResetToStartPosition()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);

        Debug.Log($"[MainCameraSetup] Camera reset to start position: {startPosition}, rotation: {startRotation}");
    }

    /// <summary>
    /// Focus camera on classroom center
    /// </summary>
    public void FocusOnClassroom()
    {
        // Default classroom center view
        transform.position = new Vector3(0, 15, -10);
        transform.rotation = Quaternion.Euler(30, 0, 0);
    }
}
