using UnityEngine;
using LOK1game.Tools;
using UnityEditorInternal.VersionControl;

namespace LOK1game
{
    public enum EDamageType : ushort
    {
        Normal = 1,
        Lazer,
        Void,
        Hit,
        Drill,
    }

    /// <summary>
    /// Represents damage information in the game, including source, type, value, and impact details.
    /// </summary>
    public struct Damage
    {
        /// <summary>
        /// The actor that caused the damage.
        /// </summary>
        public Actor Sender { get; set; }

        /// <summary>
        /// The type of damage being dealt (e.g., Normal, Lazer, Void, etc.).
        /// </summary>
        public EDamageType DamageType { get; set; }

        /// <summary>
        /// The amount of damage to be dealt.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// The physical force applied to objects when they receive damage.
        /// This affects how physical objects react to the damage.
        /// </summary>
        public float PhysicalForce { get; set; }

        /// <summary>
        /// The world position where the damage was dealt (e.g., bullet impact point).
        /// </summary>
        public Vector3 HitPoint { get; set; }

        /// <summary>
        /// The surface normal at the point of impact.
        /// </summary>
        public Vector3 HitNormal { get; set; }

        public Damage(int value)
        {
            Sender = null;
            Value = Mathf.Clamp(value, 0, Constants.Gameplay.MAXIMUM_DAMAGE);
            DamageType = EDamageType.Void;
            PhysicalForce = value;

            HitPoint = Vector3.zero;
            HitNormal = Vector3.zero;
        }

        public Damage(int value, EDamageType type)
        {
            Sender = null;
            Value = Mathf.Clamp(value, 0, Constants.Gameplay.MAXIMUM_DAMAGE);
            DamageType = type;
            PhysicalForce = value;

            HitPoint = Vector3.zero;
            HitNormal = Vector3.zero;
        }

        public Damage(int value, EDamageType type, Actor sender)
        {
            Sender = sender;
            Value = Mathf.Clamp(value, 0, Constants.Gameplay.MAXIMUM_DAMAGE);
            DamageType = type;
            PhysicalForce = value;

            HitPoint = Vector3.zero;
            HitNormal = Vector3.zero;
        }

        public Damage(int value, Actor sender)
        {
            Sender = sender;
            Value = Mathf.Clamp(value, 0, Constants.Gameplay.MAXIMUM_DAMAGE);
            DamageType = EDamageType.Normal;
            PhysicalForce = value;

            HitPoint = Vector3.zero;
            HitNormal = Vector3.zero;
        }

        public Damage(int value, Actor sender, Vector3 hitPoint, Vector3 hitNormal, float physicalForce)
        {
            Sender = sender;
            Value = Mathf.Clamp(value, 0, Constants.Gameplay.MAXIMUM_DAMAGE);
            DamageType = EDamageType.Normal;
            PhysicalForce = physicalForce;

            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }

        public Damage(DamageData data, Actor sender)
        {
            Sender = sender;
            Value = data.Damage;
            DamageType = data.Type;
            PhysicalForce = data.PhysicalForce;

            HitPoint = Vector3.zero;
            HitNormal = Vector3.zero;
        }

        public Damage(DamageData data, Actor sender, Vector3 hitPoint, Vector3 hitNormal)
        {
            Sender = sender;
            Value = data.Damage;
            DamageType = data.Type;
            PhysicalForce = data.PhysicalForce;

            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }



        public Vector3 GetHitDirection(bool drawDebugLines = false)
        {
            if (HitPoint == Vector3.zero)
                return Vector3.zero;

            var originPosition = Sender.transform.position;

            if (drawDebugLines)
            {
                Debug.DrawRay(originPosition, Vector3.up, Color.green, 4f);
                Debug.DrawRay(HitPoint, Vector3.up, Color.red, 4f);
                Debug.DrawLine(originPosition, HitPoint, Color.yellow, 4f);
            }

            return HitPoint.GetDirectionTo(originPosition).normalized;
        }
    }
}