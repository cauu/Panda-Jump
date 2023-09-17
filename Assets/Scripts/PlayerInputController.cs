using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private PlatformerMotor2D _motor;

    private bool _restored = true;
    private bool _enableOneWayPlatforms;
    private bool _oneWayPlatformsAreWalls;

    // Start is called before the first frame update
    void Start()
    {
        _motor = GetComponent<PlatformerMotor2D>();
    }

        // before enter en freedom state for ladders
    void FreedomStateSave(PlatformerMotor2D motor)
    {
        if (!_restored) // do not enter twice
            return;

        _restored = false;
        _enableOneWayPlatforms = _motor.enableOneWayPlatforms;
        _oneWayPlatformsAreWalls = _motor.oneWayPlatformsAreWalls;
    }
    // after leave freedom state for ladders
    void FreedomStateRestore(PlatformerMotor2D motor)
    {
        if (_restored) // do not enter twice
            return;

        _restored = true;
        _motor.enableOneWayPlatforms = _enableOneWayPlatforms;
        _motor.oneWayPlatformsAreWalls = _oneWayPlatformsAreWalls;
    }

    // Update is called once per frame
    void Update()
    {
        if (_motor.motorState != PlatformerMotor2D.MotorState.FreedomState)
        {
            // try to restore, sometimes states are a bit messy because change too much in one frame
            FreedomStateRestore(_motor);
        }

        // XY freedom movement
        // if (_motor.motorState == PlatformerMotor2D.MotorState.FreedomState)
        // {
        //     _motor.normalizedXMovement = Input.GetAxis(PC2D.Input.HORIZONTAL);
        //     _motor.normalizedYMovement = Input.GetAxis(PC2D.Input.VERTICAL);

        //     return; // do nothing more
        // }
    }

    public void MoveInput(Vector2 move) {
        // XY freedom movement
        if (_motor.motorState == PlatformerMotor2D.MotorState.FreedomState)
        {
            _motor.normalizedXMovement = move.x;
            _motor.normalizedYMovement = move.y;

            return; // do nothing more
        }
        // X axis movement
        if (Mathf.Abs(move.x) > PC2D.Globals.INPUT_THRESHOLD)
        {
            _motor.normalizedXMovement = move.x;
        }
        else
        {
            _motor.normalizedXMovement = 0;
        }

        if (move.y != 0) {
            bool up_pressed = move.y > 0;
            if (_motor.IsOnLadder())
            {
                if (
                    (up_pressed && _motor.ladderZone == PlatformerMotor2D.LadderZone.Top)
                    ||
                    (!up_pressed && _motor.ladderZone == PlatformerMotor2D.LadderZone.Bottom)
                 )
                {
                    // do nothing!
                }
                // if player hit up, while on the top do not enter in freeMode or a nasty short jump occurs
                else
                {
                    // example ladder behaviour

                    _motor.FreedomStateEnter(); // enter freedomState to disable gravity
                    _motor.EnableRestrictedArea();  // movements is retricted to a specific sprite bounds

                    // now disable OWP completely in a "trasactional way"
                    FreedomStateSave(_motor);
                    _motor.enableOneWayPlatforms = false;
                    _motor.oneWayPlatformsAreWalls = false;

                    // start XY movement
                    _motor.normalizedXMovement = move.x;
                    _motor.normalizedYMovement = move.y;
                }
            }
        }
        else if (move.y < -PC2D.Globals.FAST_FALL_THRESHOLD)
        {
            _motor.fallFast = false;
        }
    }

    public void JumpInput(bool isJump) {
        if (isJump) {
            _motor.Jump();
            _motor.DisableRestrictedArea();
        }

         _motor.jumpingHeld  = isJump;
    }

    public void DashInput(bool isDash) {
        if (isDash) {
            _motor.Dash();
        }
    }
}
