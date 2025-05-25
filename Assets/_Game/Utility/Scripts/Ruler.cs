using UnityEngine;
using UnityEditor;

public class Ruler : MonoBehaviour
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private Color _lineColor = Color.yellow;
    [SerializeField] private float _textOffset = 0.5f;
    [SerializeField] private int decimalPlaces = 2;

    private void OnDrawGizmos()
    {
        if (_pointA == null || _pointB == null) return;

        Gizmos.color = _lineColor;
        Gizmos.DrawLine(_pointA.position, _pointB.position);

        var distance = Vector3.Distance(_pointA.position, _pointB.position);
        var midPoint = (_pointA.position + _pointB.position) / 2f;
        var textPosition = midPoint + Vector3.up * _textOffset;

        Handles.Label(textPosition, distance.ToString($"F{decimalPlaces}") + "m");
    }

    [ContextMenu("Create Points")]
    private void CreatePoints()
    {
        if (_pointA == null)
        {
            GameObject pointAObj = new GameObject("PointA");
            pointAObj.transform.SetParent(transform);
            pointAObj.transform.localPosition = Vector3.zero;
            _pointA = pointAObj.transform;
        }

        if (_pointB == null)
        {
            GameObject pointBObj = new GameObject("PointB");
            pointBObj.transform.SetParent(transform);
            pointBObj.transform.localPosition = Vector3.right;
            _pointB = pointBObj.transform;
        }
    }
} 