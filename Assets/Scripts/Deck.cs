using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton; //stand
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {    
        InitCardValues();        
    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        for (int i = 0; i < 52; i++) 
        {
            int valor = (i % 13) + 1;
            if (valor == 10) valor = 10;
            if (valor == 1) valor = 11;
            
            values[i] = valor;
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
        for (int i = 0; i < faces.Length; i++) 
        {
            // Algoritmo "Ficher-Yates"
            //la "carta" puesta en i en la iteracion queda inaccesible en la siguiente
            int rand = Random.Range(i, faces.Length); 

            Sprite tempSprite = faces[i];
            faces[i] = faces[rand];
            faces[rand] = tempSprite;

            int tempVal = values[i];
            values[i] = values[rand];   
            values[rand] = tempVal;
        }
    }

    void StartGame()
    {
        //tomar apuesta***********************************************************************************
        for (int i = 0; i < 2; i++)
        {
            PushPlayer(); //le reparte la primera carta de la baraja, dandole a su CardHand las imgs y el valor de la carta
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             * ***Esto no tiene sentido si se supone que no sabes una de las cartas del dealer
             */
        }

        int playerPoints = player.GetComponent<CardHand>().points;
        int dealerVisiblePoints = dealer.GetComponent<CardHand>().VisiblePoints();

        //------------------------- ¿Hacer insurance?******************************************************
        if (playerPoints == 21 || dealerVisiblePoints == 11)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
            int dealerPoints = dealer.GetComponent<CardHand>().points;
            hitButton.interactable = false;
            stickButton.interactable = false;
            if (playerPoints == 21 && dealerPoints == 21)
            {
                finalMessage.text = "Empate";
                //devolver apuesta*************************************************************************
            }
            else if (playerPoints == 21)
            {
                finalMessage.text = "¡Blackjack! Jugador gana";
                //gana apuesta*************************************************************************
            }
            else if (dealerPoints == 21)º
            {
                finalMessage.text = "¡Blackjack! Dealer gana";
                //pierde apuesta*************************************************************************
            }

        }

    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        
        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */      

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */                
         
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
}
