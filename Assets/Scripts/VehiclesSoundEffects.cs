using UnityEngine;
using System.Collections;
public class VehicleSoundEffects : MonoBehaviour
{
    [Header("Movement settings")]
    public float accelerationSoundLimit = 1.5f;
    public float speedChangeTolerance = 0.5f;
    public float topSpeed = 5f;
    public float hardBrakeAcceleration = 8f;

    [Header("Turning settings")]
    public float turnStopThreshold = 1f;
    public float turnChangeTolerance = 0.1f;

    [Header("Audio sources")]
    public AudioSource firingAudioSource;
    public AudioSource reloadAudioSource;
    public AudioSource movementAudioSource;
    public AudioSource engineAudioSource;
    public AudioSource turningAudioSource;
    public AudioSource ModuleAudioSource;

    [Header("Clips")]
    public AudioClip firingSound;
    public AudioClip reloadSound;
    public AudioClip engineIdleSound;

    public AudioClip TankAccelerationSound;
    public AudioClip TankConstantSpeedSound;
    public AudioClip TankDeccelerationSound;

    public AudioClip TankTurningSound;
    public AudioClip TankAccelerationTurningSound;
    public AudioClip TankDeccelerationTurningSound;
    [Header("Modules Sounds")]
    public EngineModule engineModule;
    public AudioClip engineStartUpSound;
    public AudioClip engineDestroyedSound;
    private float acceleration;
    private float previousSpeed;

    private Rigidbody2D rb;
    private TankTurret firing;
    private EngineModule engine;

    private bool wasDecelerating;

    private enum TurnState
    {
        None,
        Accelerating,
        Constant,
        Decelerating
    }

    private TurnState turnState = TurnState.None;

    private Tank tankController;
    private float previousTurn;


    void Start()
    {
        //Modules
        engine = GetComponentInChildren<EngineModule>();
        //Modules
        rb = GetComponent<Rigidbody2D>();

        foreach (MonoBehaviour component in GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (component is TankTurret turret)
            {
                firing = turret;
                break;

            }
        }
        

        foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
        {
            if (component is Tank tank)
            {
                tankController = tank;
                break;
            }
        }

        if (tankController != null)
        {
            previousTurn = tankController.CurrentTurn;
        }

        if (firing != null)
        {
            firing.OnShoot += PlayFiringSound;
            firing.OnReload += PlayReloadSound;
        }
        if(engine != null)
        {
            engine.OnDestroyedEvent += EngineAction;
            engine.OnRepairedEvent += EngineAction;
        }
        if (engine != null && !engine.IsDestroyed && engineAudioSource != null)
        {
            engineAudioSource.clip = engineIdleSound;
            engineAudioSource.loop = true;
            engineAudioSource.Play();
        }

        if (rb != null)
        {
            previousSpeed = rb.linearVelocity.magnitude;
        }
    }

    void PlayFiringSound()
    {
        Debug.Log("PlayFiringSound() was called!");

        if (firingAudioSource != null && firingSound != null)
        {
            firingAudioSource.PlayOneShot(firingSound);
        }
    }
    
    void PlayReloadSound()
    {
        if (reloadAudioSource != null && reloadSound != null)
        {
            if (firing != null && firing.IsReloading)
            {
                reloadAudioSource.PlayOneShot(reloadSound);
            }
        }
    }
    void EngineAction()
    {
        if (engine.IsDestroyed)
        {
            ModuleAudioSource.PlayOneShot(engineDestroyedSound);
            if (engineAudioSource != null)
            {
                engineAudioSource.Stop();
                engineAudioSource.clip = null;
            }
        }
        else
        {
            StartCoroutine(EngineStartUp());
        }
    }
    IEnumerator EngineStartUp()
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script is Tank)
            {
                script.enabled = false;
            }
        }

        if (engineAudioSource != null && engineStartUpSound != null)
        {
            engineAudioSource.clip = engineStartUpSound;
            engineAudioSource.loop = false;
            engineAudioSource.Play();

            yield return new WaitWhile(() => engineAudioSource.isPlaying);
        }

        foreach (MonoBehaviour script in scripts)
        {
            if (script is Tank)
            {
                script.enabled = true;
            }
        }

        if (engineAudioSource != null && engineIdleSound != null)
        {
            engineAudioSource.clip = engineIdleSound;
            engineAudioSource.loop = true;
            engineAudioSource.Play();
        }
    }
    void Update()
    {
        if (rb == null)
        {
            return;
        }

        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        float speed = rb.linearVelocity.magnitude;
        float raw = (speed - previousSpeed) / dt;
        acceleration = Mathf.Lerp(acceleration,raw,10f * Time.deltaTime);
        previousSpeed = speed;

        UpdateMovementSound(speed);
    }
    void FixedUpdate()
    {
        UpdateTurningSound();
    }

    void UpdateMovementSound(float speed)
    {
        if (movementAudioSource == null)
        {
            return;
        }

        if (speed < 0.1f)
        {
            movementAudioSource.Stop();
            movementAudioSource.clip = null;
            wasDecelerating = false;
            return;
        }

        bool isDecelerating = acceleration < -speedChangeTolerance;

        if (isDecelerating)
        {
            float hardness = Mathf.InverseLerp(speedChangeTolerance,hardBrakeAcceleration,-acceleration);

            movementAudioSource.volume = Mathf.Lerp(0.6f,1f,hardness);
            movementAudioSource.pitch = Mathf.Lerp(1f,1.2f,hardness);

            if (!wasDecelerating)
            {
                float startTime = 0f;

                if (TankDeccelerationSound != null && topSpeed > 0f)
                {
                    startTime = (1f - Mathf.Clamp01(speed / topSpeed)) * TankDeccelerationSound.length;
                }

                PlayMovementClip(TankDeccelerationSound,false,startTime);
            }
        }

        else if (speed < accelerationSoundLimit)
        {
            float hardness = Mathf.InverseLerp(0f,hardBrakeAcceleration,Mathf.Max(acceleration, 0f));

            movementAudioSource.volume = Mathf.Lerp(0.7f,1f,hardness );
            movementAudioSource.pitch = Mathf.Lerp(1f,1.15f,hardness);

            float startTime = TankAccelerationSound != null && accelerationSoundLimit > 0f ? Mathf.Clamp01(speed / accelerationSoundLimit) * TankAccelerationSound.length: 0f;
            PlayMovementClip(TankAccelerationSound,false,startTime);
        }

        else
        {
            movementAudioSource.volume = 1f;
            movementAudioSource.pitch = 1f;
            PlayMovementClip(TankConstantSpeedSound,true,0f);
        }

        wasDecelerating = isDecelerating;
    }

    
    void PlayMovementClip(AudioClip clip,bool loop,float startTime)
    {
        if (clip == null || movementAudioSource == null || movementAudioSource.clip == clip)
        {
            return;
        }

        movementAudioSource.clip = clip;
        movementAudioSource.loop = loop;
        movementAudioSource.time = Mathf.Clamp(startTime,0f,Mathf.Max(0f, clip.length - 0.01f));
        movementAudioSource.Play();
    }

    void UpdateTurningSound()
    {
        if (tankController == null || turningAudioSource == null)
        {
            return;
        }

        float currentTurn = tankController.CurrentTurn;
        float turnDifference = Mathf.Abs(currentTurn) - Mathf.Abs(previousTurn);

        if (Mathf.Abs(currentTurn) < turnStopThreshold)
        {
            if (turnState != TurnState.None)
            {
                turningAudioSource.Stop();
                turningAudioSource.clip = null;

                turnState = TurnState.None;
            }

            previousTurn = currentTurn;

            return;
        }

        TurnState newState;

        if (turnDifference > turnChangeTolerance)
        {
            newState = TurnState.Accelerating;
        }


        else if (turnDifference < -turnChangeTolerance)
        {
            newState = TurnState.Decelerating;
        }


        else
        {
            newState = TurnState.Constant;
        }


        if (newState != turnState)
        {
            turnState = newState;


            switch (turnState)
            {
                case TurnState.Accelerating:

                    PlayTurningClip(TankAccelerationTurningSound,false);
                    break;

                case TurnState.Constant:

                    PlayTurningClip(TankTurningSound,true);
                    break;

                case TurnState.Decelerating:
                    PlayTurningClip(TankDeccelerationTurningSound,false);
                    break;
            }
        }


        previousTurn = currentTurn;
    }


    void PlayTurningClip(AudioClip clip,bool loop)
    {
        if (clip == null || turningAudioSource == null)
        {
            return;
        }

        if (turningAudioSource.clip == clip && turningAudioSource.isPlaying)
        {
            return;
        }
        turningAudioSource.clip = clip;
        turningAudioSource.loop = loop;
        turningAudioSource.Play();
    }
}