using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------
// COMPORTAMIENTO DE UNA MANO EN EL TABLERO (ENTORNO GRAFICO)
//***   como conjunto de cartas
//
//// 2. ACLARACION CHATGPT 4.0
//la conexión entre CardHand y CardModel no se hace directamente en el código
//sino que se establece a través del Editor de Unity, usando el sistema de prefabs 
//
//CardHand crea cartas (GameObjects) a partir de un prefab
//y cada carta ya lleva dentro el script CardModel
//----------------------------------------------------------

public class CardHand : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public GameObject card;
    public bool isDealer = false;
    public int points;
    private int coordY;    
     
    private void Awake()
    {
        points = 0;
        //Definimos dónde posicionamos las cartas de cada uno
        if (isDealer)
            coordY = 3;
        else
            coordY = -1;
    }

    public void Clear()
    {
        points = 0;
        if (isDealer)
            coordY = 3;
        else
            coordY = -1;
        foreach (GameObject g in cards)
        {
            Destroy(g);
        }
        cards.Clear();
        // El método Destroy(g) elimina cada carta de la escena.
        // cards.Clear() borra los registros de esas cartas en el script
    }

    public void InitialToggle()
    {
        //Destapa la primera carta del dealer. Esto se usará cuando el turno del dealer comience
        cards[0].GetComponent<CardModel>().ToggleFace(true);              
    }

    public void Push(Sprite front, int value)
    {
        //Creamos una carta y la añadimos a nuestra mano
        GameObject cardCopy = (GameObject)Instantiate(card);
        cards.Add(cardCopy); // 1. ACLARACION CHATGPT 4.0
        //Cuando haces cards.Add(cardCopy), estás añadiendo un puntero al objeto real.
        //Si luego lo modificas, esos cambios afectan al mismo objeto en memoria.

        //La posicionamos en el tablero 
        float coordX = (float)1.4 * (float)(cards.Count - 4);
        Vector3 pos = new Vector3(coordX, coordY);               
        cardCopy.transform.position = pos;

        //Le ponemos la imagen y el valor asignado
        cardCopy.GetComponent<CardModel>().front = front;
        cardCopy.GetComponent<CardModel>().value = value;
        
        //La cubrimos si es la primera del dealer
        if (isDealer && cards.Count <= 1)
            cardCopy.GetComponent<CardModel>().ToggleFace(false);
        else
            cardCopy.GetComponent<CardModel>().ToggleFace(true);

        //Calculamos la puntuación de nuestra mano
        int val = 0;
        int aces = 0;
        foreach (GameObject f in cards)
        {            

            if (f.GetComponent<CardModel>().value != 11)
                val += f.GetComponent<CardModel>().value;
            else
                aces++;
        }

        for (int i = 0; i < aces; i++)
        {
            if (val + 11 <= 21)
            {
                val = val + 11;
            }
            else
            {
                val = val + 1;
            }
        }

        points = val;
       
    }

    public int VisiblePoints()
    {
        int puntosVisibles = points;
        bool esCartaVisible = cards[0].GetComponent<CardModel>().isFaceUp;

        if (isDealer && !esCartaVisible)
        {
            int valorOculto = cards[0].GetComponent<CardModel>().value;
            puntosVisibles -= valorOculto;
        }
        return puntosVisibles;
    }

}
