using UnityEngine;

   public interface ITankMovement
    {
        float CurrentSpeed { get; }

        void DisableMovement();
        void EnableMovement();

        void DestroyLeftTrack();
        void DestroyRightTrack();
        void EngineDestroyed();
        
    }
