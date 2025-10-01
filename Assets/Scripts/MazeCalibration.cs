using System.Collections.Generic;
using UnityEngine;

public class MazeCalibration : MonoBehaviour
{
    [Header("References")]
    public Transform rightController;     // RightHandAnchor
    public GameObject mazePrefab;         // Prefab of your maze (Cube/OBJ)
    public LineRenderer laserLine;        // Laser pointer line
    public TMPro.TextMeshProUGUI debugText;
    public GameObject pointMarkerPrefab;  // Optional: prefab for small spheres at points

    [Header("Settings")]
    public float depth = 0.1f;            // Thickness of the maze/cube
    public float markerSize = 0.05f;      // Size of debug markers

    private List<Vector3> selectedPoints = new List<Vector3>();
    private GameObject currentMaze;       // Track current maze instance

    void Update()
    {
        // Trigger pressed → capture point
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (laserLine != null && laserLine.positionCount > 1)
            {
                Vector3 hitPoint = laserLine.GetPosition(1);
                selectedPoints.Add(hitPoint);
                Debug.Log("[MazeCalibration] Captured point: " + hitPoint);

                // Spawn small marker spheres (optional)
                if (pointMarkerPrefab != null)
                {
                    GameObject marker = Instantiate(pointMarkerPrefab, hitPoint, Quaternion.identity);
                    marker.transform.localScale = Vector3.one * markerSize;
                }

                // After 4 points → calibrate
                if (selectedPoints.Count == 4)
                {
                    CalibrateMaze();
                    selectedPoints.Clear();
                }
            }
        }
    }

    void CalibrateMaze()
    {
        Debug.Log("[MazeCalibration] === Starting CalibrateMaze() ===");

        // Require exactly 4 points in this order: TL, TR, BR, BL
        Vector3 TL = selectedPoints[0];
        Vector3 TR = selectedPoints[1];
        Vector3 BR = selectedPoints[2];
        Vector3 BL = selectedPoints[3];

        // Compute basis vectors
        Vector3 right = (TR - TL).normalized;          // X axis
        Vector3 up = (BL - TL).normalized;             // Y axis
        Vector3 forward = Vector3.Cross(right, up).normalized; // Z axis (plane normal)

        // Center of quad
        Vector3 center = (TL + TR + BR + BL) / 4f;

        // Destroy old maze if exists
        if (currentMaze != null)
        {
            Destroy(currentMaze);
            Debug.Log("[MazeCalibration] Previous maze destroyed");
        }

        // Create new maze
        currentMaze = Instantiate(mazePrefab);
        currentMaze.SetActive(true);

        // Rotation: align cube axes
        currentMaze.transform.rotation = Quaternion.LookRotation(forward, up);

        // Position at center
        currentMaze.transform.position = center;

        // Scale to match distances
        float width = (TR - TL).magnitude;   // distance top edge
        float height = (BL - TL).magnitude;  // distance left edge

        currentMaze.transform.localScale = new Vector3(width, height, depth);

        Debug.Log($"[MazeCalibration] Maze at {center}, size=({width:F2},{height:F2},{depth:F2})");
        Debug.Log("[MazeCalibration] === CalibrateMaze() finished ===");

        if (debugText != null)
            debugText.text = $"Maze built at {center:F2}\nSize=({width:F2},{height:F2},{depth:F2})";
    }
}