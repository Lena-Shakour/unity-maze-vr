using System.Collections.Generic;
using UnityEngine;

public class FreeSpaceMazeBuilder : MonoBehaviour
{
    public Transform rightController;   // ה־RightHandAnchor
    public GameObject mazePrefab;       // Prefab של המבוך

    private List<Vector3> selectedPoints = new List<Vector3>();

    void Update()
    {
        // לחיצה על טריגר ימין → שמירת מיקום היד
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            selectedPoints.Add(rightController.position);
            Debug.Log("Point captured: " + rightController.position);

            if (selectedPoints.Count == 4)
            {
                BuildMaze();
                selectedPoints.Clear();
            }
        }
    }

    void BuildMaze()
    {
        if (mazePrefab == null)
        {
            Debug.LogError("MazePrefab not assigned!");
            return;
        }

        // יוצרים מבוך חדש
        GameObject mazeInstance = Instantiate(mazePrefab);

        // מרכז = ממוצע ארבע נקודות
        Vector3 center = (selectedPoints[0] + selectedPoints[1] +
                          selectedPoints[2] + selectedPoints[3]) / 4f;

        mazeInstance.transform.position = center;

        // ציר X = קו בין נקודות ראשונות (0→1)
        Vector3 x = (selectedPoints[1] - selectedPoints[0]).normalized;

        // ציר Y = קו בין נקודות אנכיות (0→3)
        Vector3 yRaw = (selectedPoints[3] - selectedPoints[0]);
        Vector3 y = (yRaw - x * Vector3.Dot(yRaw, x)).normalized;

        // נורמל (קדימה–אחורה)
        Vector3 n = Vector3.Cross(x, y).normalized;

        mazeInstance.transform.rotation = Quaternion.LookRotation(n, y);

        // גודל
        float width = (selectedPoints[1] - selectedPoints[0]).magnitude;
        float height = (selectedPoints[3] - selectedPoints[0]).magnitude;
        mazeInstance.transform.localScale = new Vector3(width, height, mazeInstance.transform.localScale.z);

        Debug.Log("Maze built in free space!");
    }
}