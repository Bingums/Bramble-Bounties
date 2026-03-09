using UnityEngine;

public class Draw2DBoxCollider : MonoBehaviour
{
    [SerializeField] private GameObject line;
    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = Instantiate(line).GetComponent<LineRenderer>();
        lineRenderer.transform.SetParent(transform);
        lineRenderer.transform.localPosition = Vector3.zero;
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        OutlineBox();
    }

    public void OutlineBox()
    {
        Vector3[] positions = new Vector3[4];
        Vector2 size = boxCollider2D.size;
        positions[0] = transform.TransformPoint(new Vector2(size.x / 2f, size.y / 2f));
        positions[1] = transform.TransformPoint(new Vector2(-size.x / 2f, size.y / 2f));
        positions[2] = transform.TransformPoint(new Vector2(-size.x / 2f, -size.y / 2f));
        positions[3] = transform.TransformPoint(new Vector2(size.x / 2f, -size.y / 2f));
        lineRenderer.SetPositions(positions);
    }
}
