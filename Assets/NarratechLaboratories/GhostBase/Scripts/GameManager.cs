/*    
   Copyright (C) 2025 Narratech Laboratories
   https://www.narratech.com
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com

   Modificación del GameEnding original para convertirlo en algo más parecido a un GameManager
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // Vamos a tenerlo como Ejemplar Único
    public static GameManager Instance { get; private set; }

    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public float elapsedTime = 0f; // Tiempo transcurrido
    public int gotchas = 0; // Pilladas  
    public int wins = 0; // Ganadas
    
    private GameObject player;
    private CanvasGroup exitBackgroundImageCanvasGroup;
    private CanvasGroup caughtBackgroundImageCanvasGroup;

    private bool m_IsPlayerAtExit;
    private bool m_IsPlayerCaught;
    private float m_Timer = 0.0f;
    private bool m_HasAudioPlayed;
    private GameObject start; // Referencia al waypoint de inicio
    private Label timeLabel; // Referencia a la etiqueta de UI Toolkit
    private Label gotchasLabel; // Referencia a la etiqueta de UI Toolkit
    private Label winsLabel; // Referencia a la etiqueta de UI Toolkit

    private AudioSource ambientAudio;
    private AudioSource alertAudio;
    private AudioSource exitAudio;
    private AudioSource caughtAudio;


    public List<GameObject> enemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Evita duplicados
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Mantiene este mismo objeto entre escenas

        SceneManager.sceneLoaded += OnSceneLoaded; // Se ejecuta cada vez que cambia la escena (incluida la primera vez)
    }

    // Se ejecuta entre el Awake y el Start
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Obtener las etiquetas desde el UI Toolkit
        var root = FindObjectOfType<UIDocument>().rootVisualElement;
        timeLabel = root.Q<Label>("TimeValue");
        gotchasLabel = root.Q<Label>("GotchasValue");
        winsLabel = root.Q<Label>("WinsValue");

        // Al cargar la escena, ya sea por primera vez o por pasarme el juego, recalculo el Start

        // Obtener todos los objetos con el tag "Start"
        GameObject[] startObjects = GameObject.FindGameObjectsWithTag("Start");

        // Verificar si hay al menos un objeto con ese tag
        if (startObjects.Length > 0)
        {
            // Seleccionar un objeto aleatorio de la lista
            start = startObjects[Random.Range(0, startObjects.Length)];
        }
        else
        {
            Debug.LogWarning("No se encontraron objetos con el tag 'Start'.");
        }

        // Obtener el avatar del jugador
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'Player'.");
        }
        else
        {
            player.transform.position = start.transform.position;
            player.transform.rotation = start.transform.rotation;
        }

        GameObject ambientObject = GameObject.FindWithTag("AMBIENT_MUSIC");
        if (ambientObject != null)
        {
            ambientAudio = ambientObject.GetComponent<AudioSource>();
            if (ambientAudio == null)
            {
                Debug.LogWarning("No se encontró un componente 'AudioSource' en el objeto 'AMBIENT_MUSIC' correspondiente.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'AMBIENT_MUSIC'.");
        }

        GameObject alertObject = GameObject.FindWithTag("ALERT_MUSIC");
        if (alertObject != null)
        {
            alertAudio = alertObject.GetComponent<AudioSource>();
            if (alertAudio == null)
            {
                Debug.LogWarning("No se encontró un componente 'AudioSource' en el objeto 'ALERT_MUSIC' correspondiente.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'ALERT_MUSIC'.");
        }

        GameObject escapeObject = GameObject.FindWithTag("Escape");
        if (escapeObject != null)
        {
            exitAudio = escapeObject.GetComponent<AudioSource>();
            if (exitAudio == null)
            {
                Debug.LogWarning("No se encontró un componente 'AudioSource' en el objeto 'Escape' correspondiente.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'Escape'.");
        }

        GameObject caughtObject = GameObject.FindWithTag("Caught");
        if (caughtObject != null)
        {
            caughtAudio = caughtObject.GetComponent<AudioSource>();
            if (caughtAudio == null)
            {
                Debug.LogWarning("No se encontró un componente 'AudioSource' en el objeto 'Caught' correspondiente.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'Caught'.");
        }

        GameObject exitImageObject = GameObject.FindWithTag("ExitImage");
        if (exitImageObject != null)
        {
            exitBackgroundImageCanvasGroup = exitImageObject.GetComponent<CanvasGroup>();
            if (exitBackgroundImageCanvasGroup == null)
            {
                Debug.LogWarning("No se encontró un componente 'CanvasGroup' en el objeto 'ExitImage' correspondiente.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'ExitImage'.");
        }

        GameObject caughtImageObject = GameObject.FindWithTag("CaughtImage");
        if (caughtImageObject != null)
        {
            caughtBackgroundImageCanvasGroup = caughtImageObject.GetComponent<CanvasGroup>();
            if (caughtBackgroundImageCanvasGroup == null)
            {
                Debug.LogWarning("No se encontró un componente 'CanvasGroup' en el objeto 'CaughtImage' correspondiente.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'CaughtImage'.");
        }

        ambientAudio.Play();

        // Inicializar el texto del cronómetro y los otros textos a cero
        UpdateTimerUI();
        UpdateGotchasUI();
        UpdateWinsUI();
    }

    void Start()
    {

    }

    public void AlertPlayerDetected()
    {
        ambientAudio.Stop();
        alertAudio.Play();
    }

    public void PlayerIsMissing()
    {
        ambientAudio.Play();
        alertAudio.Stop();
    }

    private void UpdateTimerUI()
    {
        // Convertir tiempo a minutos, segundos y décimas
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int hundredths = Mathf.FloorToInt((elapsedTime * 100) % 100); // Centésimas de segundo

        // Actualizar el texto del Label con el formato MM:SS:FF
        timeLabel.text = $"{minutes:00}:{seconds:00}:{hundredths:00}";
    }

    private void UpdateGotchasUI()
    {
        gotchasLabel.text =  gotchas.ToString();

    }

    private void UpdateWinsUI()
    {
        winsLabel.text = wins.ToString();

    }

    // El GameManager es a la vez la salida
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            m_IsPlayerAtExit = true;
            wins++;
            UpdateWinsUI();
        }
    }

    // Se llama aquí cuando un punto de vista (Observer) detecta al jugador
    public void CaughtPlayer()
    {
        m_IsPlayerCaught = true;
        gotchas++;
        UpdateGotchasUI();
    }

    void Update()
    {
        // Incrementar el tiempo
        elapsedTime += Time.deltaTime;

        // Actualizar el tiempo en la UI
        UpdateTimerUI();

        if (m_IsPlayerAtExit)
        {
            EndLevel(exitBackgroundImageCanvasGroup, false, exitAudio);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel(caughtBackgroundImageCanvasGroup, true, caughtAudio);
        }
    }

    void EndLevel(CanvasGroup imageCanvasGroup, bool doRestart, AudioSource audioSource)
    {
        if (!m_HasAudioPlayed)
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }

        m_Timer += Time.deltaTime;
        imageCanvasGroup.alpha = m_Timer / fadeDuration;

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            if (doRestart)
            {
                // Reiniciar significa simplemente sumar 1 a las 'pilladas' y volver a llevar a nuestro avatar al punto de inicio que hubiera en esta partida
                m_IsPlayerCaught = false; // Para que no se repita

                player.transform.position = start.transform.position;
                player.transform.rotation = start.transform.rotation;
                // Cambio todo lo necesario para volver a la normalidad pero sin reiniciar el nivel
                imageCanvasGroup.alpha = 0.0f;
                m_Timer = 0.0f;
                m_HasAudioPlayed = false; 
            }
            else
            {
                //foreach (GameObject obj in enemies)
                //{
                //    if (obj.GetComponent<EnemyHealth>() != null)
                //    {
                //        if (!obj.GetComponent<EnemyHealth>().IsAlive())
                //        {
                //            obj.GetComponent<EnemyHealth>().Revive();
                //        }
                //        else
                //        {
                //            obj.GetComponentInChildren<ChasePlayer>().enabled = false;
                //            obj.GetComponentInChildren<Investigate>().enabled = false;
                //            obj.GetComponentInChildren<WaypointPatrol>().enabled = true;
                //        }
                //    }
                //}

                // No reiniciar significa sumar 1 a las 'ganadas' y volver a cargar la escena, con lo que esta podría ser diferente (por ahora sólo cambia el punto de inicio)
                m_IsPlayerAtExit = false; // Para que no se repita
                m_Timer = 0.0f;
                m_HasAudioPlayed = false;
                SceneManager.LoadScene(0); // O podía usar "MainScene" que es el nombre de la escena  
                // En ningún caso haremos Application.Quit ();
                // ...si acaso podemos añadir un botón (la R) para hacer auténtico Reset, de tiempo y todo. y Quit para salir del juego
            }
        }
    }
}