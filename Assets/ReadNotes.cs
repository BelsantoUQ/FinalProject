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
    [SerializeField] private GameObject hudCard;
    [SerializeField] private GameObject interact;
    [SerializeField] private MenuController menuController;
    [SerializeField] private bool isWin;
    [SerializeField] private bool isNote;

    [SerializeField] private GameObject pickUpSound;
    [SerializeField] private GameObject dropSound;

    private Renderer render;

    private float maxRange = 1f;
    private bool inReach;
    private bool reading;
    private bool isInteracting;
    private PlayerMovement playerMovement;

    void Start()
    {
        if (isWin == false)
        {
            noteUI.SetActive(false);
        }
        hud.SetActive(true);
        interact.SetActive(false);
        reading = false;
        inReach = false;
        isInteracting = false;
        playerMovement = FindObjectOfType<PlayerMovement>(); // referencia al script ImprovedPlayerMovement
        menuController = FindObjectOfType<MenuController>(); // referencia al script menuController



    }

    private void OnMouseEnter()
    {
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    Transform activeInteractable = child;
                    render = activeInteractable.GetComponent<Renderer>();
                }
            }
        }
    }


    private void OnMouseOver()
    {

        if (!menuController.isPaused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRange) && !isInteracting && !reading)
            {
                inReach = true;
                interact.SetActive(true);
                if (gameObject.CompareTag("Cards") || gameObject.CompareTag("Book"))
                {
                    //render.materials[1].SetFloat("_Show_Outline", 1.0f);
                    render.materials[1].SetInt("_Show_Outline", 1);
                }
            }


        }
    }

    void OnMouseExit()
    {
        if (reading == false)
        {
            inReach = false;
            interact.SetActive(false);
        }
        if (gameObject.CompareTag("Cards") || gameObject.CompareTag("Book"))
        {
            //render.materials[1].SetFloat("_Show_Outline", 0.0f);
            render.materials[1].SetInt("_Show_Outline", 0);

        }


    }

    void Update()
    {


        if (Input.GetKeyDown(KeyCode.E) && inReach && reading == false)
        {
            isInteracting = true;

            interact.SetActive(false);
            if (isWin == false)
            {
                playerMovement.SetIsAbleToLook(false);
                noteUI.SetActive(true);
            }
            else
            {
                noteUI.GetComponent<MenuController>().SetWinScreen();
            }
            if (isNote)
            {
                dropSound.SetActive(false);
                pickUpSound.SetActive(true);
            }
            hud.SetActive(false);
            
            
            StartCoroutine(CloseOpenNote(0.1f));
            if (!(selectedItem == ItemOptions.Note))
            {
                GameManager gameManager = GameManager.Instance;
                gameManager.AcquireItem(selectedItem.ToString());
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && Time.timeScale == 0 && reading && isWin == false)
        {
            isInteracting = false;


            reading = false;
            playerMovement.SetIsAbleToLook(true);
            noteUI.SetActive(false);
            
            if (isNote)
            {
                pickUpSound.SetActive(false);
                dropSound.SetActive(true);
            }
            hud.SetActive(true);
            if (isNote == false)
            {
                hudCard.SetActive(true);
            }
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
