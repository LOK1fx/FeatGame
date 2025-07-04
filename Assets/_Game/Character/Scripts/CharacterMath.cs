using UnityEngine;

namespace LOK1game
{
    public class CharacterMath
    {
        public struct MoveParams
        {
            public MoveParams(Vector3 accelDir, Vector3 prevVel)
            {
                AccelerationDirection = accelDir;
                PreviousVelocity = prevVel;
            }

            public Vector3 AccelerationDirection;
            public Vector3 PreviousVelocity;
        }

        public struct CharacterData
        {
            public float Height { get; set; }
            public float SkinWidth { get; set; }
            public float Radius { get; set; }
            public LayerMask WorldLayerMask { get; set; }
            public float MaxSlopeAngle { get; set; }
        }

        private const uint COLLIDE_AND_SLIDE_MAX_BOUNCES = 12;
        private const float COLLIDE_AND_SLIDE_SKIN_WIDTH = 0.015f;

        public static Vector3 ProjectOnPlane(Vector3 dir, Vector3 normal)
        {
            return dir - Vector3.Dot(dir, normal) * normal;
        }

        public static bool IsFloor(Vector3 normal)
        {
            return Vector3.Angle(Vector3.up, normal) < 45;
        }

        public static Vector3 Accelerate(MoveParams moveParams, float accelerate, float maxVelocity, float delta)
        {
            float projectVel = Vector3.Dot(moveParams.PreviousVelocity, moveParams.AccelerationDirection);
            float accelerationVelocity = accelerate * delta;

            if (projectVel + accelerationVelocity > maxVelocity)
                accelerationVelocity = maxVelocity - projectVel;

            return moveParams.PreviousVelocity + moveParams.AccelerationDirection * accelerationVelocity;
        }

        public static Vector3 TestAccelerate(MoveParams moveParams, float accelerate, float maxVelocity, float delta, CharacterData character, Vector3 position, bool gravityPass, bool isGrounded)
        {
            var projectVel = Vector3.Dot(moveParams.PreviousVelocity, moveParams.AccelerationDirection);
            var accelerationVelocity = accelerate * delta;

            if (projectVel + accelerationVelocity > maxVelocity)
                accelerationVelocity = maxVelocity - projectVel;

            var velocity = moveParams.PreviousVelocity + moveParams.AccelerationDirection * accelerationVelocity;

            return CollideAndSlide(velocity, position, character, 0, gravityPass, velocity, isGrounded);
        }

        public static Vector3 CollideAndSlide(Vector3 velocity, Vector3 position, CharacterData character, uint depth, bool gravityPass, Vector3 desiredVelocity, bool isGrounded)
        {
            if (depth >= COLLIDE_AND_SLIDE_MAX_BOUNCES)
                return Vector3.zero;

            var distance = velocity.magnitude + character.SkinWidth;
            var point2 = position + Vector3.up * character.Height;

            if (Physics.CapsuleCast(position, point2, character.Radius,
                velocity.normalized, out var hit, distance, character.WorldLayerMask, QueryTriggerInteraction.Ignore))
            {
                var snapToSurface = velocity.normalized * (hit.distance - character.SkinWidth);
                var leftover = velocity - snapToSurface;
                var angle = Vector3.Angle(Vector3.up, hit.normal);

                if (snapToSurface.magnitude <= character.SkinWidth)
                    snapToSurface = Vector3.zero;

                if (angle <= character.MaxSlopeAngle)
                {
                    if (gravityPass == true)
                        return snapToSurface;

                    leftover = ProjectAndScale(leftover, hit.normal);
                }
                else
                {
                    var scale = 1 - Vector3.Dot(
                            new Vector3(hit.normal.x, 0f, hit.normal.z).normalized,
                            -new Vector3(desiredVelocity.x, 0f, desiredVelocity.z).normalized
                        );

                    if (isGrounded && gravityPass == false)
                    {
                        leftover = ProjectAndScale(
                            new Vector3(leftover.x, 0f, leftover.z),
                            new Vector3(hit.normal.x, 0f, hit.normal.z)
                            ).normalized;
                        leftover *= scale;
                    }
                    else
                    {
                        leftover = ProjectAndScale(leftover, hit.normal) * scale;
                    }                 
                }
                

                return snapToSurface + CollideAndSlide(leftover, position + snapToSurface, character, depth+1, gravityPass, desiredVelocity, isGrounded);
            }

            return velocity;
        }

        private static Vector3 ProjectAndScale(Vector3 vector, Vector3 normal)
        {
            var magnitude = vector.magnitude;
            vector = Vector3.ProjectOnPlane(vector, normal).normalized;
            vector *= magnitude;

            return vector;
        }
    } 
}