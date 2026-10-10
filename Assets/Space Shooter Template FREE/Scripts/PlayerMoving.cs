using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gestiona el movimiento del jugador mediante teclado (WASD / Flechas) 
/// y restringe la posición dentro de los límites visibles de la pantalla.
/// </summary>

[System.Serializable]
public class Borders
{
    [Tooltip("Margen desde los bordes de la pantalla para el movimiento de la nave")]
    public float minXOffset = 0.5f, maxXOffset = 0.5f, minYOffset = 0.5f, maxYOffset = 0.5f;
    [HideInInspector] public float minX, maxX, minY, maxY;
}

public class PlayerMoving : MonoBehaviour 
{
    [Tooltip("Límites de movimiento de la nave en la pantalla")]
    public Borders borders;

    [Tooltip("Velocidad de desplazamiento de la nave")]
    public float speed = 12f;

    Camera mainCamera;
    public static PlayerMoving instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        ResizeBorders();
    }

    private void Update()
    {
        // Leer el teclado con el Input System activo del proyecto.
        Vector2 keyboardInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            keyboardInput.x = (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f)
                - (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
            keyboardInput.y = (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1f : 0f)
                - (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? 1f : 0f);
        }

        Vector3 moveDirection = new Vector3(keyboardInput.x, keyboardInput.y, 0).normalized;
        transform.position += moveDirection * speed * Time.deltaTime;

        // Restringir la nave dentro de los bordes de la pantalla.
        if (mainCamera != null)
        {
            transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, borders.minX, borders.maxX),
                Mathf.Clamp(transform.position.y, borders.minY, borders.maxY),
                0
            );
        }
    }

    // Recalcula los límites de pantalla de forma segura según la cámara
    void ResizeBorders() 
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            borders.minX = mainCamera.ViewportToWorldPoint(Vector2.zero).x + borders.minXOffset;
            borders.minY = mainCamera.ViewportToWorldPoint(Vector2.zero).y + borders.minYOffset;
            borders.maxX = mainCamera.ViewportToWorldPoint(Vector2.right).x - borders.maxXOffset;
            borders.maxY = mainCamera.ViewportToWorldPoint(Vector2.up).y - borders.maxYOffset;
        }
    }
}