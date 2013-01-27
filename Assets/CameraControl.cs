using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public float cameraSize;
    public float minCameraSize = 8;
    public float maxCameraSize = 20;

    public int horizontalScrollingMax;
    public int upwardScrollingMax; 
    public int downwardScrollingMax;

    public float scrollingSpeed;

    public Vector2 scrolling;
    public Vector2 desiredScrolling;

    public float smooth;

    public bool dragging;

    public Vector2 scrollingMouseOrigin;
    public Vector2 scrollingMouseCurrent;
    public Vector2 scrollingDir;

    void Start()
    {
        Camera.mainCamera.orthographicSize = cameraSize;
        scrolling = Camera.mainCamera.transform.position;
        desiredScrolling = scrolling;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragging = true;
            Screen.lockCursor = true;

            scrollingMouseOrigin = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            dragging = false;
            Screen.lockCursor = false;
        }

        if (dragging)
        {
            Vector2 newMousePosition = Input.mousePosition;
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); 
            delta.Normalize();

            scrollingDir = -delta;
            scrollingMouseCurrent = newMousePosition;

            desiredScrolling += scrollingDir * scrollingSpeed * cameraSize * Time.deltaTime;
            desiredScrolling.x = Mathf.Clamp(desiredScrolling.x, -horizontalScrollingMax, horizontalScrollingMax);
            desiredScrolling.y = Mathf.Clamp(desiredScrolling.y, -downwardScrollingMax, upwardScrollingMax);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            cameraSize = Mathf.Min(cameraSize + 1, maxCameraSize);
            Camera.mainCamera.orthographicSize = cameraSize;
        } 
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            cameraSize = Mathf.Max(cameraSize - 1, minCameraSize);
            Camera.mainCamera.orthographicSize = cameraSize;
        }

        scrolling = scrolling * smooth + (1 - smooth) * desiredScrolling;
		Vector3 pos = Camera.main.transform.position;
		if (horizontalScrollingMax > 0)
			pos.x = scrolling.x;
		pos.y = scrolling.y;
        Camera.main.transform.position = pos;

    }
}
