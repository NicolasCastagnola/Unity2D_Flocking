using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Utils : MonoBehaviour
{
    #region Draw Line
    public struct LineDrawer
    {
        private LineRenderer lineRenderer;
        private float lineSize;

        public LineDrawer(float lineSize = 0.2f)
        {
            GameObject lineGO = new GameObject("LineObj");
            lineRenderer = lineGO.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
            this.lineSize = lineSize;
        }

        private void Initialize(float lineSize = 0.2f)
        {
            if (lineRenderer == null)
            {
                var lineObj = new GameObject("LineObj");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
                this.lineSize = lineSize;
            }
        }
        public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
        {
            if (lineRenderer == null)
            {
                Initialize(0.2f);
            }

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        public void Destroy(GameObject objectToDestroy)
        {
            objectToDestroy.SetActive(false);
        }
    }
    #endregion
}
