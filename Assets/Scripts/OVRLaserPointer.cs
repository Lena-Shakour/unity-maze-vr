using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OVRLaserPointer : MonoBehaviour
{
    [Tooltip("Optional - if empty, script will GetComponent<LineRenderer>() or add one.")]
    public LineRenderer line;
    public float maxDistance = 20f;

    [HideInInspector] public bool hasHit;
    [HideInInspector] public Vector3 hitPoint;

    void Awake()
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
            if (line == null)
            {
                line = gameObject.AddComponent<LineRenderer>();
            }
        }

        // ensure two positions
        line.positionCount = 2;

        // reasonable defaults (can change in Inspector)
        line.widthMultiplier = 0.01f;
    }

    void Update()
    {
        Vector3 start = transform.position;
        Vector3 dir = transform.forward;

        RaycastHit hit;
        Vector3 end = start + dir * maxDistance;
        hasHit = false;

        if (Physics.Raycast(start, dir, out hit, maxDistance))
        {
            hasHit = true;
            hitPoint = hit.point;
            end = hit.point;
        }

        if (line != null)
        {
            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }
    }
}
