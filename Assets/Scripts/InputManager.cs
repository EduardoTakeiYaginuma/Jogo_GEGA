using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Componente que publica entradas do novo Input System
/// em propriedades estáticas. Deve ficar em um GameObject
/// que possua um PlayerInput apontando para o mesmo asset.
/// </summary>
public class InputManager : MonoBehaviour
{
    // ---------------- INPUTS PÚBLICOS -----------------
    public static Vector2 Movement  { get; private set; }
    public static bool    Running   { get; private set; }
    public static bool    Attacking { get; private set; }   // ← NOVO

    // ---------------- REFERÊNCIAS ----------------------
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _runAction;
    private InputAction _attackAction;                      // ← NOVO

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        // Se usar múltiplos Action Maps, pegue via map["Move"] etc.
        _moveAction   = _playerInput.actions["Move"];
        _runAction    = _playerInput.actions["Run"];
        _attackAction = _playerInput.actions["Attack"];     // ← NOVO
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _runAction.Enable();
        _attackAction.Enable();                             // ← NOVO
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _runAction.Disable();
        _attackAction.Disable();                            // ← NOVO
    }

    private void Update()
    {
        Movement  = _moveAction.ReadValue<Vector2>();
        Running   = _runAction.IsPressed();
        Attacking = _attackAction.WasPerformedThisFrame();  // ← NOVO
    }
}
