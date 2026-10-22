using UnityEngine;

public interface Tank 
{
   float CurrentSpeed { get; }
   float CurrentTurn { get; }
   bool isAccelerating { get; }
}
