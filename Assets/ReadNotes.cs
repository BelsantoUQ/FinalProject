using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using LeanTween;

public class ReadNotes : MonoBehaviour
{
    private enum ItemOptions { Red, Blue, Gold, Note }
    [SerializeField]
    private ItemOptions selectedItem = ItemOptions.Red; // Valor azul predeterminado

    [SerializeField] private GameObject noteUI;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject pause;

    [SerializeField] private GameObject interact;
    [SerializeField] private bool isWin;

    [SerializeField] private AudioSource pickUpSound;
    [SerializeField] private AudioSource dropSound;

    private bool inReach;
    private bool reading;
    private PlayerMovement playerMovement;

    void Start()
    {
        noteUI.SetActive(false);
        hud.SetActive(true);
        pause.SetActive(false);
        interact.SetActive(false);
        reading = false;
        inReach = false;
        playerMovement = FindObjectOfType<PlayerMovement>(); // referencia al script ImprovedPlayerMovement
    }

    void OnMouseEnter()
    {
        if (reading == false)
        {
            inReach = true;
            interact.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (reading == false)
        {
            inReach = false;
            interact.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inReach && reading == false)
        {
            interact.SetActive(false);
            playerMovement.SetIsAbleToLook(false);
            noteUI.SetActive(true);
            pickUpSound.Play();
            hud.SetActive(false);
            pause.SetActive(false);
            StartCoroutine(CloseOpenNote(0.1f));
            if (!(selectedItem == ItemOptions.Note))
            {
                GameManager gameManager = GameManager.Instance;
                gameManager.AcquireItem(selectedItem.ToString());
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && inReach && Time.timeScale == 0 && reading && isWin == false)
        {
            reading = false;
            playerMovement.SetIsAbleToLook(true);
            noteUI.SetActive(false);
            dropSound.Play();
            hud.SetActive(true);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor en el centro de la pantalla
            Cursor.visible = false; // Hacer el cursor pauseisible
            // Encoger y destruir
            /* LeanTween.scale(gameObject, Vector3.zero, 0.5f).setOnComplete(() => {
                Destroy(gameObject);
            }); */
            if (!(selectedItem == ItemOptions.Note))
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator CloseOpenNote(float time)
    {
        yield return new WaitForSeconds(time);
        reading = !reading;
        Time.timeScale = reading ? 0f : 1f;
        if (isWin)
        {
            // Make the cursor visible and unlock it
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
