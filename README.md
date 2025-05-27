## Documentación
Metal Gear Solid es un famoso juego de Play Station donde el principal motor del gameplay es el sigilo y la infiltración. En este proyecto final vamos a crear algo parecido a lo que se ve en el juego para la IA de los enemigos.

Tenemos los siguientes puntos a considerar:

1) Hay un mundo virtual, un nivel donde están situados el protagonista y el resto de enemigos y objetos.

2) El protagonista será controlado por el jugador, así que en este caso usaremos el input y no una IA para controlarlo. El jugador podrá tirar objetos por el escenario para distraer a los enemigos, y también podrá dejarlos inconscientes si estos no le ven, además de esconderse cuando sea perseguido.

3) Los enemigos estarán controlados por IA y tendrán diferentes comportamientos gestionados por una "pseudomáquina de estados" (en realidad tan solo pasa de un script a otro si en un script cumple una función determinada para transicionar), y también están en todo momento atentos por escuchar las órdenes del resto de compañeros gracias a un radar.

## Controles

Movimiento: WASD/Flechas teclado
Atacar: Tecla Espacio
Soltar objeto: Tecla Q
Intentar esconderse en un armario: Tan solo hay que meterse dentro de ellos sin que te vean

## Enemigos

El comportamiento básico de un enemigo es que se dedica a patrullar primero. Cuando llega a un punto del patrullaje se queda investigando, y luego vuelve a la patrulla. Si encuentra un objeto raro tirado en el suelo se quedará investigandolo, y luego lo eliminará, pero si se encuentra a un compañero caído, llama a otros compañeros cerca suya para que se repartan por la zona a puntos cercanos a ver si encuentran algo. Reanima al compañero mientras se queda en el mismo punto investigando.

Si el enemigo ve en cualquier momento, en cualquier estado al jugador, empieza a perseguirlo, llama a otros enemigos cercanos para que lo ayuden a atraparlo. En plena persecución habrá un contador interno por cada enemigo que volverá al valor máximo en cuanto uno de los enemigos vea al jugador, es decir, si en la persecución algún de enemigo ve al jugador, avisa a los enemigos cercanos de que el jugador todavía está allí y el contador de todos los enemigos vuelve a su valor máximo y siguen persiguiéndolo. Si durante un tiempo todos los enemigos lo pierden de vista, todos vuelven a sus posiciones.

Pero si en plena persecución el jugador se esconde sin que le vean, el enemigo y sus compañeros se quedarán investigando varios puntos cercanos de la zona. Pero si los enemigos ven como el jugador se mete en un punto de escondite, el jugador no podrá esconderse y sequirá siendo perseguido y podrá ser atrapado.

El momento en que el enemigo alcanza al jugador, el jugador pierde y se puede volver a intentar.

Todo esto se podría considerar como una expansión de la práctica 3 principalmente, cuya principal NOVEDAD es la gestión del comportamiento de varios agentes, donde los enemigos pueden indicar a otros qué es lo que tienen que hacer. Cada enemigo lleva lo que se dice como "EnemyComunicator", donde pueden indicar a otros enemigos diferentes órdenes si estos se encuentran dentro de la frecuencia.

Los ESTADOS BÁSICOS, los principales que tienen que estar son los siguientes:

- Patrullar: Se dedica a deambular por una ruta, comprobando si encuentra algo de interés.
- Investigar: Comprueba si hay algo sospechoso en un punto. Si ve un objeto, lo investiga, si ve un cuerpo, intenta llamar a otros enemigos cercanos a investigar la zona mientras el enemigo reanima a su compañero e investiga el punto, si pierde de vista al jugador aún estando alerta sepone a investigar ese punto o la zona junto a sus compañeros, si ve al jugador, lo comienza a perseguir.
- Perseguir: Persigue al jugador hasta alcanzarlo o perderlo de vista. Si se esconde el jugador, investiga la zona junto a sus compañeros.

Adjunto una imagen esquematizada sobre su comportamiento, donde se explica brevemente la transición entre estados de un enemigo (el estado de que le indican de ir a un punto si se lo ordenan no viene representado en el esquema): 
![alt text](image.png)

## Video de presentación de la IA de los enemigos

Aviso de que la velocidad alta del jugador era para hacer las pruebas más fáciles (y que puede atravesar paredes por ello)

https://youtu.be/O6ZUCy1LdQ0

## Pseudo código

Aquí una breve representación por encima de las diferentes clases que englogan el comportamiento de la IA enemiga (aunque descrito con código normál más que con pseudocódigo real):

Class BaseState:
{

    public Transform player; //Referencia al player
    public GameManager gameManager; //Referencia al gamemanager
    public EnemyComunicator enemyComunicator;
    public GameObject comunicator; //Comunicador del enemigo

    public List<Transform> lookablePoints = new List<Transform>(); //Lista de puntos cercanos donde mirar si pasa algo

    public float visionRange = 10f;         // Cuánto alcance tiene el cono
    public float visionAngle = 30f;         // Ángulo total del cono
    public int rayCount = 20;               // Cuántos rayos lanzar


    public virtual void DetectSmth()
    {
        //Lanza RayCasts en diferentes direcciones para ver si detecta algo, aunque el método cambia en clases hijas
    }

    public void ScanLookablePoints()
    {
        //Actualiza lookablePoints para ver si encuentra puntos cercanos donde investigar
    }

    public Transform RandomDestination()
    {
        //Devuelve un punto aleatorio de la lista de lookablePoints
    }
}

Class WaypointPatrol (hereda de BaseState):
{
  public NavMeshAgent navMeshAgent; //Referencia al nav mesh agent
  public Transform[] waypoints; //Lista de puntos de patruyaje

  int m_CurrentWaypointIndex; //Indice del punto de patrullaje a donde va

  private void OnEnable()
  {
      navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
  }

  void Start ()
  {
    //Busca el game manager y el player en la escena y los añade a las referencias
    navMeshAgent.SetDestination (waypoints[0].position);
    enemyComunicator = comunicator.GetComponent<EnemyComunicator>();
  }

  public override void DetectSmth()
  {
    //Se pone a lanzar diferentes raycasts para ver si detecta algo.
    //Si ve al jugador, activa la clase ChasePlayer, avisa a los compañeros por el comunicador, y desactiva WaypointPatrol.
    //Si ve a un aliado caído, avisa a sus compañeros cercanos de investigar la zona, activa la clase Investigate e investiga ese punto, y desactiva WaypointPatrol.
    //Si ve un objeto tirado en el suelo, activa Investigate y va hacia ese punto.
  }

  void Update ()
  {
    DetectSmth();
    transform.rotation = navMeshAgent.transform.rotation;
    if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
    {
        GetComponent<Investigate>().enabled = true;
        GetComponent<Investigate>().GoToPointToInvestigate(waypoints[m_CurrentWaypointIndex]);
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        this.enabled = false;
    }
  }
}

Class Investigate (hereda de BaseState):
{
  GameObject objectToDestroy; //Objeto a destruir si ha sido encontrado en el suelo
  GameObject partnerToRevive; //Compañero a revivir si ha quedado inconsciente

  public NavMeshAgent navMeshAgent; //Referencia al nav mesh agent
  Transform pointToInvestigate; //Punto a investigar, puede dejarse en null si no se va a usar
  
  //Parametros que indican cuanto dura el tiempo de investigación
  public float investigationTime;
  float stopInvestigationCoolDown;

  private void OnDisable()
  {
    if (objectToDestroy != null)
        Destroy(objectToDestroy);

    stopInvestigationCoolDown = investigationTime;
  }

  public void ResetTime()
  {
    stopInvestigationCoolDown = investigationTime;
  }

  public void GoToPointToInvestigate(Transform point) //Puede no usar se este método cuando está patrullando e investiga un punto de su ruta de patrullaje, pero no otra cosa
  {
    pointToInvestigate = point;
    navMeshAgent.SetDestination(pointToInvestigate.position);
  }

  public void ObjectToDestroy(GameObject obj)
  {
    objectToDestroy = obj;
  }

  public void PartnerToRevive(GameObject obj)
  {
    partnerToRevive = obj;
  }

  void Start()
  {
    //Busca el game manager y el player en la escena y los añade a las referencias
    stopInvestigationCoolDown = investigationTime;
    enemyComunicator = comunicator.GetComponent<EnemyComunicator>();
    this.enabled = false;
  }

  public override void DetectSmth()
  {
    //Se pone a lanzar diferentes raycasts para ver si detecta algo.
    //Si ve al jugador, activa la clase ChasePlayer, avisa a los compañeros por el comunicador, y desactiva Investigate.
    //Si ve a un aliado caído, avisa a sus compañeros cercanos de investigar la zona, e investiga ese punto.
    //Si ve un objeto tirado en el suelo, va hacia ese punto a investigar.
  }

  void Update()
  {
    DetectSmth();

    if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance * 3)
    {

      //Revive al compañero caído si tiene que hacerlo

      stopInvestigationCoolDown -= Time.deltaTime;

      if (stopInvestigationCoolDown <= investigationTime / 2)
      {
        transform.Rotate(new Vector3(0, 2, 0)); //Rota a ver si encuentra algo
      }

      if (stopInvestigationCoolDown <= 0.0f)
      {
        GetComponent<WaypointPatrol>().enabled = true;
        this.enabled = false;
      }
    }
  }
}


Class ChasePlayer (hereda de BaseState):
{
  public NavMeshAgent navMeshAgent; //Referencia al nav mesh agent

  //Parametros que determinan la cantidad de tiempo que hay entre que ningun enemigo ve al jugador y paren de perseguirlo
  public float chaseTime;
  float stopChaseCoolDown;


  float originalStoppingDistance; //Distancia a la que paran, importante para que el enemigo no esté empujando al player cuando no lo ve

  bool playerVisible; //Determina si el jugador es visible o no, si otro compañero lo ve, entonces aquí también es true
  bool partnerAlert; //Determina si ha sido alertado por otro de que ha visto al player

  public void SeePlayer(bool see)
  {
    playerVisible = see;
    partnerAlert = see;
  }

  private void OnEnable()
  {
    originalStoppingDistance = navMeshAgent.stoppingDistance;
    playerVisible = true;
    partnerAlert = true;
    navMeshAgent.SetDestination(player.position);  
  }

  private void OnDisable()
  {
    navMeshAgent.stoppingDistance = originalStoppingDistance;
    stopChaseCoolDown = chaseTime;
    playerVisible = false;
    partnerAlert = false;
  }

  void Start()
  {  
    //Busca el game manager y el player en la escena y los añade a las referencias
    playerVisible = false;
    partnerAlert = false;
    enemyComunicator = comunicator.GetComponent<EnemyComunicator>();
    stopChaseCoolDown = chaseTime;
    this.enabled = false;
  }

  public override void DetectSmth()
  {
    //Se pone a lanzar diferentes raycasts para ver si detecta algo.
    //Si ve al jugador, alerta al resto de compañeros de que lo ha visto y hace que no se pueda esconder porque lo está viendo
  }

  void Update()
  {
    DetectSmth();

    transform.rotation = navMeshAgent.transform.rotation;

    if (player.gameObject.GetComponent<PlayerMovement>().IsInvisible())
    {
        navMeshAgent.stoppingDistance = originalStoppingDistance * 10;
    }
    else
    {
        navMeshAgent.stoppingDistance = originalStoppingDistance;
    }

    navMeshAgent.SetDestination(player.position);
    if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && player.gameObject.GetComponent<PlayerMovement>().IsInvisible())
    {
      //Ha llegado a donde está el jugador, pero como está escondido y no lo ve, avisa a sus compañeros de investigar junto a él la zona, y luego desactiva ChasePlayer
    }
    if (playerVisible)
    {
        stopChaseCoolDown = chaseTime;
    }
    else if (!playerVisible)
    {
        stopChaseCoolDown -= Time.deltaTime;
        if (stopChaseCoolDown <= 0.0f)
        {
            gameManager.PlayerIsMissing();
            enemyComunicator.StopChasingPlayer();
            GetComponent<WaypointPatrol>().enabled = true;
            this.enabled = false;
        }
    }
    partnerAlert = false;
  }
}


Aquí va una representación de la clase que determina el comportamiento del comunicador de los enemigos:

Class EnemyComunicator:
{
    List<GameObject> partners = new List<GameObject>(); //Lista de compañeros que están dentro de la cobertura para darles órdenes.

    public void PartnersSpreadAroundTheArea(List<Transform> lookablePoints)
    {
        //En la persecución, cuando no ven al jugador escondido, uno de los enemigos le dice al resto a través de este método que investiguen la zona en varios lookablePoints
    }

    public void PartnerIsUnconciousSpread(List<Transform> lookablePoints)
    {
      //Se encarga de avisar a los compañeros de que investiguen la zona cercana a donde está el compañero caído en varios lookablePoints
    }

    public void GoAndChasePlayer()
    {
      //Si un enemigo ve al jugador cuando patrulla o investiga, con este método avisa a sus compañeros cercanos de que se pongan a perseguir también al jugador
    }

    public void IseePlayer()
    {
      //Si un enemigo ve al jugador cuando lo persigue, con este método avisa a sus compañeros cercanos de que sigan persiguiendo al jugador
    }

    public void StopChasingPlayer()
    {
      //Si un enemigo pierde de vista al jugador en la persecución (no tiene por qué estar escondido), avisa al resto de que vuelvan a sus puestos
    }

    void OnTriggerEnter(Collider other)
    {
      //Añade compañeros a la lista de partners si han entrado en la zona de cobertura.
    }

    void OnTriggerExit(Collider other)
    {
      //Si un compañero sale de la zona de cobertura, se elimina de la lista de partners.
    }

}
