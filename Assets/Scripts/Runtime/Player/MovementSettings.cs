using UnityEngine;

namespace MJ198.Player
{
    [CreateAssetMenu(fileName = "DefaultMovementSettings", menuName = "Player/MovementSettings")]
    public class MovementSettings : ScriptableObject
    {
        [Header("Look")]
        public float Sensitivity = 3f;
        public Vector2 YLookLimit = new Vector2(-80f, 80f);
        public bool InvertLookY = false;
        public bool InvertLookX = false;

        [Header("Movement")]
        public float Speed = 10f;
        public float GroundAcceleration = 12f;
        public float GroundDeceleration = 10f;
        public float RunningMul = 2f;
        public float JumpPower = 0.2f;
        public float AirControl = 2f;
        public float GravityMul = 1f;

        [Header("Sliding")]
        public float SlideSpeedMul = 3f;
        public float SlideFriction = 8f;
        public float SlideMinSpeed = 1.1f;
        public float SlideCooldownTime = 0.2f;
        public float SlideHeadDrop = -0.4f;      
        public float SlideHeadForward = 0.25f;   
        public float SlideHeadSpeed = 9f;        
        public float SlideHeadReturnSpeed = 7f;

        [Header("Wall Running")]
        public LayerMask WallLayerMask;
        public float WallRunDuration = 1.5f;
        public float WallRunMinSpeed = 5f;
        public float WallRunGravityMul = 0.3f;
        public float WallRunSpeedMul = 1.2f;
        public float WallRunningCooldownTime = 0.2f;
        public float WallDetectionDistance = 1f;
        public float WallNormalLerpSpeed = 12f;
        public float WallStickForce = 25f;

        [Header("Grapple")]
        public float GrappleRange = 40f;
        public LayerMask GrappleLayerMask;
        public float GrappleMinDetachDistance = 3f;
        public float GrappleExitBoost = 18f;
        public float GrapplePullAcceleration = 110f;
        public float GrappleMaxSpeed = 35f;
        public float GrappleSphereCastRadius = 0.5f;

        [Header("Grapple Indicator")]
        public GameObject GrappleIconPrefab;

        [Header("Wall Jump")]
        public float WallJumpForwardBoost = 3f;
        public float WallJumpUpForce = 8f;
        public float WallJumpSideForce = 6f;
        public float WallJumpLockTime = 0.2f;

        [Header("Head Bobbing")]
        public float HeadBobAmplitudeX = 0.05f;
        public float HeadBobAmplitudeY = 0.05f;
        public float HeadBobFrequency = 8f;
        public float HeadBobRunMultiplier = 0.5f;

        [Header("FOV Kick")]
        public float DefaultFOV = 60f;
        public float FOVKickAmount = 10f;
        public float FOVKickSpeed = 10f;

        [Header("Head Tilt")]
        public float WallRunTiltAngle = 10f;
        public float SlideTiltAngle = 15f;
        public float TiltSpeed = 8f;

        [Header("Gun")]
        public float ShootTime = 0.2f;
        public float BulletVelocity = 2.0f;
        public float BulletLifeSpan = 10.0f;
        public float BulletDamage = 10.0f;
    }
}
