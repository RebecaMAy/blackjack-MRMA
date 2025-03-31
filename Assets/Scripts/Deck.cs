using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;

    public GameObject dealer;
    public GameObject player;

    public Button hitButton;
    public Button stickButton; //stand
    public Button playAgainButton;
    public Button betButton;

    public Dropdown BetDropdown;

    public Text pointsDealer;
    public Text pointsPlayer;

    public Text bank;
    private int valueBank;

    public Text bet;
    public int currentBet = 0;

    public Text finalMessage;
    public Text probMessage1;
    public Text probMessage2;
    public Text probMessage3;

    public int[] values = new int[52];
    int cardIndex = 0;

    private void Awake()
    {    
        InitCardValues();        
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

            if (valor == 1)
                valor = 11; // As vale 11
            else if (valor >= 11)
                valor = 10; // J, Q, K valen 10

            values[i] = valor;
        }
    }

    private void Start()
    {
        ShuffleCards();
        //no puede repartir sin antes de apostar
        //StartGame();
        InitBank();
        BotonesInicio();
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

    private void InitBank()
    {
        valueBank = 1000;
        bank.text = valueBank.ToString();
    }

    public void BotonesInicio()
    {
        hitButton.interactable = false;
        stickButton.interactable = false;
        betButton.interactable = false;
        playAgainButton.interactable = false;
        BetDropdown.interactable = true;

        BetDropdown.ClearOptions();
        var opciones = new List<string>();

        for (int i = 10; i <= valueBank; i += 10)
        {
            opciones.Add(i.ToString());
        }

        BetDropdown.AddOptions(opciones);
    }

    public void ChangeCurrentBet()
    {
        reiniciarTablero();
        string seleccion = BetDropdown.options[BetDropdown.value].text;
        currentBet = int.Parse(seleccion);
        bet.text = seleccion;
        bank.text = (valueBank-currentBet).ToString();
        betButton.interactable = true;
    }

    public void Bet()
    {
        betButton.interactable = false;
        BetDropdown.interactable = false;
        valueBank = valueBank-currentBet;
        StartGame();
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer(); //le reparte la primera carta de la baraja, dandole a su CardHand las imgs y el valor de la carta
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             * ***Esto no tiene sentido si se supone que no sabes una de las cartas del dealer
             */
        }

        CalculateProbabilities();

        //muestra puntos de cada uno
        int playerPoints = player.GetComponent<CardHand>().points;
        int dealerVisiblePoints = dealer.GetComponent<CardHand>().VisiblePoints();

        pointsPlayer.text = playerPoints.ToString();
        pointsDealer.text = dealerVisiblePoints.ToString();

        //------------------------- ¿Hacer insurance?
        if (playerPoints == 21 /*|| dealerVisiblePoints == 11*/)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
            int dealerPoints = dealer.GetComponent<CardHand>().points;
            pointsDealer.text = dealerPoints.ToString();

            if (playerPoints == 21 && dealerPoints == 21) 
            {
                jugadorEmpata();
            }
            else
            {
                jugardorGana("¡Blackjack! Jugador gana");
            }
        }
        else
        {
            hitButton.interactable = true;
            stickButton.interactable = true;
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

        // 1. Cartas que quedan por repartir
        List<int> remainingCards = new List<int>();
        for (int i = cardIndex; i < values.Length; i++)
        {
            remainingCards.Add(values[i]);
        }

        int playerPoints = player.GetComponent<CardHand>().points;
        int dealerVisible = dealer.GetComponent<CardHand>().VisiblePoints();

        // ---- 1. Probabilidad de que el dealer tenga más puntos que el jugador ----
        int dealerWinsCount = 0;

        foreach (int hiddenCard in remainingCards)
        {
            int dealerTotal = dealerVisible + hiddenCard;
            // Considerar el valor del As (puede valer 1 si no cabe 11)
            if (hiddenCard == 11 && dealerVisible + 11 > 21)
            {
                dealerTotal = dealerVisible + 1;
            }
            if (dealerTotal > playerPoints && dealerTotal <= 21)
            {
                dealerWinsCount++;
            }
        }

        float probDealerHigher = (float)dealerWinsCount / remainingCards.Count;
        probMessage1.text = probDealerHigher.ToString();

        // ---- 2. Probabilidad de que tú saques carta y quedes entre 17 y 21 ----
        int goodRangeCount = 0;

        foreach (int card in remainingCards)
        {
            int total = playerPoints + card;

            if (card == 11 && playerPoints + 11 > 21)
                total = playerPoints + 1;

            if (total >= 17 && total <= 21)
            {
                goodRangeCount++;
            }
        }

        float probBetween17and21 = (float)goodRangeCount / remainingCards.Count;
        probMessage2.text = probBetween17and21.ToString();

        // ---- 3. Probabilidad de pasarse (>21) ----
        int over21Count = 0;

        foreach (int card in remainingCards)
        {
            int total = playerPoints + card;

            if (card == 11 && playerPoints + 11 > 21)
                total = playerPoints + 1;

            if (total > 21)
            {
                over21Count++;
            }
        }

        float probOver21 = (float)over21Count / remainingCards.Count;
        probMessage3.text = probOver21.ToString();

    }

    public void Hit()
    {
        //Repartimos carta al jugador
        PushPlayer();

        int puntosJugador = player.GetComponent<CardHand>().points;
        pointsPlayer.text = puntosJugador.ToString();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (puntosJugador > 21)
        {
            jugadorPierde("Te has pasado de 21. ¡Pierdes!");
        }
        else
        {
            CalculateProbabilities();
        }
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.// REALMENTE SIEMPRE DESPUES DE STAND DESTAPAS SU CARTA
         */
        int playerPoints = player.GetComponent<CardHand>().points;

        dealer.GetComponent<CardHand>().InitialToggle();
        int dealerPoints = dealer.GetComponent<CardHand>().points;
        pointsDealer.text = dealerPoints.ToString();

        if (dealerPoints == 21)
        {
            jugadorPierde("¡Blackjack! Dealer gana");
        }
        else
        {
            /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
            while (dealerPoints <= 16)
            {
                PushDealer();
                CalculateProbabilities();
                dealerPoints = dealer.GetComponent<CardHand>().points;
                pointsDealer.text = dealerPoints.ToString();
            }

            CalculateProbabilities();
            dealerPoints = dealer.GetComponent<CardHand>().points;
            pointsDealer.text = dealerPoints.ToString();

            if (dealerPoints > 21)
            {
                jugardorGana("El dealer se pasó de 21. ¡Jugador gana!");
            }
            else if (dealerPoints > playerPoints)
            {
                jugadorPierde("El dealer tiene más puntos. ¡Pierdes!");
            }
            else if (dealerPoints < playerPoints)
            {
                jugardorGana("¡Jugador gana!");
            }
            else
            {
                jugadorEmpata();
            }

        }
    }

    public void jugardorGana(string message)
    {
        finalMessage.text = message;

        valueBank = valueBank + (currentBet*2);
        bank.text = valueBank.ToString();

        bet.text = "0";
        currentBet = 0;

        BotonesInicio();
    }

    public void jugadorEmpata()
    {
        finalMessage.text = "¡Empate!";

        valueBank = valueBank + currentBet;
        bank.text = valueBank.ToString();

        bet.text = "0";
        currentBet = 0;

        BotonesInicio();
    }

    public void jugadorPierde(string message)
    {
        currentBet = 0;
        bet.text = "0";

        if (valueBank == 0)
        {
            finalMessage.text = message + " Te has quedado sin dinero para apostar, has perdido la partida. ¿Quieres volver a jugar?";
            BotonesFinal();
        }
        else
        {
            finalMessage.text = message;
            BotonesInicio();
        }
    }

    public void BotonesFinal()
    {
        hitButton.interactable = false;
        stickButton.interactable = false;
        betButton.interactable = false;
        BetDropdown.interactable = false;
        playAgainButton.interactable = true;
    }

    public void reiniciarTablero()
    {
        finalMessage.text = "";
        probMessage1.text = "";
        probMessage2.text = "";
        probMessage3.text = "";

        pointsPlayer.text = "";
        pointsDealer.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();

        cardIndex = 0;
        ShuffleCards();
    }

    public void PlayAgain()
    {
        reiniciarTablero();
        InitBank();
        BotonesInicio();
    }
    
}
