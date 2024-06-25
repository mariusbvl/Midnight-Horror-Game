using EnemyScripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EnemyAI))] 
    public class FieldOfViewEditor : UnityEditor.Editor 
    {
        private void OnSceneGUI()
        {
            EnemyAI fov = (EnemyAI)target;
            Handles.color = Color.white;
            var position = fov.transform.position;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, fov.radius);

            var eulerAngles = fov.transform.eulerAngles;
            Vector3 viewAngle1 = DirectionFromAngle(eulerAngles.y, -fov.angle / 2);
            Vector3 viewAngle2 = DirectionFromAngle(eulerAngles.y, fov.angle / 2);
            Handles.color = Color.yellow;
            Handles.DrawLine(position, position + viewAngle1 * fov.radius);
            Handles.DrawLine(position, position + viewAngle2 * fov.radius);

            if (fov.canSeePlayer)
            {
                Handles.color = Color.green;
                Handles.DrawLine(fov.transform.position , fov.playerRef.transform.position);
            }
        }
        private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}

