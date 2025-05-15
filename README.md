## Documentación
Metal Gear Solid es un famoso juego de Play Station donde el principal motor del gameplay es el sigilo y la infiltración. En este proyecto final vamos a crear algo parecido a lo que se ve en el juego para la IA de los enemigos.

Tenemos los siguientes puntos a considerar:

1) Hay un mundo virtual, un nivel donde están situados el protagonista y el resto de enemigos y objetos.

2) El protagonista será controlado por el jugador, así que en este caso usaremos el input y no una IA para controlarlo. El jugador podrá tirar objetos por el escenario para distraer a los enemigos, y también podrá dejarlos inconscientes si estos no le ven.

3) Los enemigos estarán controlados por IA y tendrán diferentes comportamientos gestionados por una máquina de estados, y también están en todo momento atentos por escuchar las órdenes del resto de compañeros.

## Enemigos

El comportamiento básico de un enemigo es que se dedica a patrullar primero. Si encuentra un objeto raro tirado en el suelo se quedará investigandolo, pero si se encuentra a un compañero caído, llama a otros compañeros cerca suya para que vengan. Luego el enemigo que ha encontrado al compañero caído dice a los otros que se repartan por la zona a puntos cercanos a ver si encuentran algo. Si solo está él consciente y no viene nadie, o si a él le apetece también, se va él a investigar a un punto cercano. Después de esto el cuerpo del inconsciente desaparece.

Si el enemigo ve en cualquier momento, en cualquier estado al jugador, empieza a perseguirlo, llama a otros enemigos cercanos para que lo ayuden a atraparlo, y un contador de alerta se pone al máximo valor mientras ve al jugador (en el Metal Gear se ponía a 99% si veían al jugador), y si lo pierde de vista, poco a poco empieza a bajar el contador de alerta hasta 0, momento en el que el enemigo vuelve a su ruta original a seguir patruyando.

Si el jugador es perseguido y este se esconde, el enemigo cuando llega a la ultima posicion registrada del jugador, se va a otro punto cercano a investigar y ver si lo encuentra, o se queda en el mismo punto investigando, y en ambos casos dice a los otros enemigos que se vayan a otro punto a investigar.

El momento en que el enemigo alcanza al jugador, se acaba el juego y se puede volver a intentar.

Todo esto se podría considerar como una expansión de la práctica 3 principalmente, cuya principal NOVEDAD es la gestión del comportamiento de varios agentes, donde los enemigos pueden indicar a otros qué es lo que tienen que hacer.

Los ESTADOS BÁSICOS, los principales que tienen que estar son los siguientes:

- Patrullar: Se dedica a deambular por una ruta, comprobando si encuentra algo de interés.
- Investigar: Comprueba si hay algo sospechoso en un punto. Si ve un objeto, lo investiga, si ve un cuerpo, intenta llamar a otros enemigos cercanos e investiga ese punto o la zona, si pierde de vista al jugador aún estando alerta sepone a investigar ese punto o la zona junto a sus compañeros.
- Perseguir: Persigue al jugador hasta alcanzarlo o perderlo de vista.

Adjunto una imagen esquematizada sobre su comportamiento, donde se explica brevemente la transición entre estados de un enemigo (el estado de que le indican de ir a un punto si se lo ordenan no viene representado en el esquema): 
![alt text](<Captura de pantalla 2025-05-15 192659.png>)


## Pseudo código

Aquí una breve representación muy básica de cómo sería la IA enemiga (aunque descrito con código normál más que con pseudocódigo real, y los métodos y estructuras que aparecen pueden no ser parte de la versión final por que no se ha completado el código todavía):

class EnemyAI:
{
enum Estados {PATRULLAR, PERSEGUIR, INVESTIGAR_OBJETO, INVESTIGAR_ZONA, INVESTIGAR_CUERPO};
Estados estado;

void Start{
  estado = PATRULLAR;
}

void Update {
  switch(estado){
    case PATRULLAR: {
      if (No ve nada raro)
        Patrol(); //Patrulla
      else if (Ve un objeto)
        estado = INVESTIGAR_OBJETO;
      else if (Ve un cuerpo)
        estado = INVESTIGAR_CUERPO;
      else if (VE al jugador)
        estado = PERSEGUIR;
      
      break;
    }

    case PERSEGUIR: {
      if (No ha avisado ya al resto de enemigos)
        WarningPartners(); //Avisa una vez a los enemigos cercanos cuando entra en estado de perseguir
      
      if (Sigue alerta){
        if (VE al jugador)
          ResetAlertCooldown(); //Mantiene el contador de alerta al tope
          Chase(); //Persigue al jugador
        else{
          UpdateAlertCooldown(); //Actualiza el contador de alerta hasta que llegue a 0
          Chase(); //Persigue al jugador
          if (ha llegado a donde estaba el jugador por última vez (antes de esconderse), pero no lo ve)
            estado = INVESTIGAR_ZONA;

          if (El cooldown de alerta llega a 0)
            Ya no está alerta;
        }
      }
      else
        estado = PATRULLAR;

      break;
    }

    case INVESTIGAR_ZONA: {
      if (No ha avisado ya al resto de enemigos que se repartan por la zona a investigar)
        OrderPartnersToInvestigateZone(); //Avisa una vez a los enemigos, cuando entra en estado de investigar la zona, de que vayan a otras zonas cercanas a investigar
      
      if (Si decide quedarse){
        InvestigateThisPoint(); //Investiga el punto en el que está
      }
      else{
        InvestigateZone(); //Va a un punto a investigar si encuentra al jugador, y si lo encuentra entonces vuelve a perseguirlo
      }

      if (No ha encontrado nada)
        estado = PATRULLAR;

      break;

    }

    case INVESTIGAR_OBJETO: {
      InvestigateObject(); //Se pone a investigar el objeto y a interactuar con él, y si ve al jugador, cambia al estado de perseguir

      if (ha terminado de investigar)
        estado = PATRULLAR;
      
      break;
    }

    case INVESTIGAR_CUERPO: {
      if (No ha dicho que se acerquen el resto de enemigos a su posición)
        OrderPartnersToCome(); //Avisa una vez a los enemigos, cuando entra en estado de investigar el cuerpo, de que vengan a ver lo que ha pasado

      if (No ha avisado ya al resto de enemigos que se repartan por la zona a investigar)
        OrderPartnersToInvestigateZone(); //Avisa una vez a los enemigos de que vayan a otras zonas cercanas a investigar

      if (Si decide quedarse){
        InvestigateThisPoint(); //Investiga el punto en el que está
      }
      else{
        InvestigateZone(); //Va a un punto a investigar si encuentra al jugador, y si lo encuentra entonces vuelve a perseguirlo
      }

      if (No ha encontrado nada)
        estado = PATRULLAR;

      break;
    }
  }
}


}