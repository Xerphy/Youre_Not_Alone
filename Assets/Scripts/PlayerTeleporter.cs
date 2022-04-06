using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTeleporter : MonoBehaviour
{
    private GameObject currentTeleporter;

    CharacterController2D controller;

    private void Awake()
    {
        controller = this.gameObject.GetComponent<CharacterController2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject tp = collision.gameObject;

        if (collision.CompareTag("Teleporter"))
        {
            if (tp.name == "Teleporter1")
            {
                controller.doubleJumpEnabled = true;
            }

            var position = collision.gameObject.GetComponent<Teleporter>().GetDestination().position;
            float x = position.x;
            float y = position.y;

            transform.position = new Vector2(x, y);
        }
    }
}
