using UnityEngine;

namespace LOK1game.Tools
{
    public static class DebugUtility
    {
        public static void AssertNotNull(Object @object)
        {
            Debug.Assert(@object != null, "Shouldn't be null.");
        }

        /// <summary>
        /// Отрисовывает сферу в указанной позиции
        /// </summary>
        /// <param name="position">Позиция центра сферы</param>
        /// <param name="radius">Радиус сферы</param>
        /// <param name="color">Цвет сферы</param>
        /// <param name="duration">Время отображения в секундах</param>
        public static void DrawSphere(Vector3 position, float radius, Color color, float duration = 0)
        {
            // Отрисовка основных кругов сферы
            DrawCircle(position, Vector3.forward, radius, color, duration);
            DrawCircle(position, Vector3.up, radius, color, duration);
            DrawCircle(position, Vector3.right, radius, color, duration);
        }

        /// <summary>
        /// Отрисовывает куб в указанной позиции
        /// </summary>
        /// <param name="position">Позиция центра куба</param>
        /// <param name="size">Размер куба</param>
        /// <param name="color">Цвет куба</param>
        /// <param name="duration">Время отображения в секундах</param>
        public static void DrawCube(Vector3 position, Vector3 size, Color color, float duration = 0)
        {
            Vector3 halfSize = size * 0.5f;
            
            // Нижняя грань
            Debug.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), position + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), position + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), position + new Vector3(halfSize.x, -halfSize.y, halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), position + new Vector3(halfSize.x, -halfSize.y, halfSize.z), color, duration);

            // Верхняя грань
            Debug.DrawLine(position + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), position + new Vector3(halfSize.x, halfSize.y, -halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), position + new Vector3(-halfSize.x, halfSize.y, halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(halfSize.x, halfSize.y, -halfSize.z), position + new Vector3(halfSize.x, halfSize.y, halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(-halfSize.x, halfSize.y, halfSize.z), position + new Vector3(halfSize.x, halfSize.y, halfSize.z), color, duration);

            // Вертикальные линии
            Debug.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), position + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), position + new Vector3(halfSize.x, halfSize.y, -halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), position + new Vector3(-halfSize.x, halfSize.y, halfSize.z), color, duration);
            Debug.DrawLine(position + new Vector3(halfSize.x, -halfSize.y, halfSize.z), position + new Vector3(halfSize.x, halfSize.y, halfSize.z), color, duration);
        }

        private static void DrawCircle(Vector3 position, Vector3 normal, float radius, Color color, float duration)
        {
            const int segments = 20;
            Vector3 previousPoint = position + GetCirclePoint(normal, 0, radius);
            
            for (int i = 1; i <= segments; i++)
            {
                float angle = (float)i / segments * 360f * Mathf.Deg2Rad;
                Vector3 currentPoint = position + GetCirclePoint(normal, angle, radius);
                Debug.DrawLine(previousPoint, currentPoint, color, duration);
                previousPoint = currentPoint;
            }
        }

        private static Vector3 GetCirclePoint(Vector3 normal, float angle, float radius)
        {
            Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(right, normal).normalized;
            
            return right * (Mathf.Cos(angle) * radius) + up * (Mathf.Sin(angle) * radius);
        }
    }
}
