using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickInputReceiver : MonoBehaviour
{
    public struct InputState
    {
        public Vector2 MovementVector;
        public int SprintModifierMode;
    }

    [Header("Movement 1")]
    [SerializeField] private KeyCode _upCode = KeyCode.W;
    [SerializeField] private KeyCode _downCode = KeyCode.S;
    [SerializeField] private KeyCode _leftCode = KeyCode.A;
    [SerializeField] private KeyCode _rightCode = KeyCode.D;

    [Header("Movement 2")]
    [SerializeField] private KeyCode _altUpCode = KeyCode.UpArrow;
    [SerializeField] private KeyCode _altDownCode = KeyCode.DownArrow;
    [SerializeField] private KeyCode _altLeftCode = KeyCode.LeftArrow;
    [SerializeField] private KeyCode _altRightCode = KeyCode.RightArrow;

    [Header("Modifiers 1")]
    [SerializeField] private KeyCode _sprintCode = KeyCode.Z;
    [SerializeField] private KeyCode _secondSprintCode = KeyCode.X;

    [Header("Modifiers 2")]
    [SerializeField] private KeyCode _altSprintCode = KeyCode.LeftShift;
    [SerializeField] private KeyCode _altSecondSprintCode = KeyCode.LeftControl;

    [Header("Others")]
    [SerializeField] private bool _showLog;

    private InputState _inputState;

    public InputState QueryInputState()
    {
        return _inputState;
    }

    private void Update()
    {
        var movement = new Vector2();
        if (Input.GetKey(_rightCode) || Input.GetKey(_altRightCode)) 
        {
            movement.x = 1f;
        }
        else if (Input.GetKey(_leftCode) || Input.GetKey(_altLeftCode))
        {
            movement.x = -1f;
        }

        if (Input.GetKey(_upCode) || Input.GetKey(_altUpCode))
        {
            movement.y = 1f;
        }
        else if (Input.GetKey(_downCode) || Input.GetKey(_altDownCode))
        {
            movement.y = -1f;
        }

        int modifier = 0;
        modifier += (Input.GetKey(_sprintCode) || Input.GetKey(_altSprintCode)) ? 1 : 0;
        modifier += (Input.GetKey(_secondSprintCode) || Input.GetKey(_altSecondSprintCode)) ? 1 : 0;

        _inputState.MovementVector = movement;
        _inputState.SprintModifierMode = modifier;

        if (_showLog)
        {
            Debug.Log($"{name}: InputState: Movement: {_inputState.MovementVector}; SpeedMode: {_inputState.SprintModifierMode}");
        }
    }
}
