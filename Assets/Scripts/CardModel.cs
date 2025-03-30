using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------
// COMPORTAMIENTO DE UNA *Card.prefab* EN EL TABLERO (ENTORNO GRAFICO)
//----------------------------------------------------------

public class CardModel : MonoBehaviour
{
    //Esta clase representa una carta (visual) individual del juego.
    //podrá tener comportamiento visual o interactivo dentro del juego
    //se comporta como objeto en la escena
    //MonoBehaviour da acceso a métodos especiales como Start(), Update(), Awake()

    SpriteRenderer spriteRenderer;
    //el componente visual que dibuja una imagen (Sprite) en la pantalla en 2D.

    public Sprite cardBack;
    public Sprite front;
    public int value;
    public bool isFaceUp = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //---------------función de Unity que busca un componente adjunto al mismo objeto.

    }
    
    public void ToggleFace(bool showFace)
    {
        isFaceUp = showFace;
        if (showFace)
        {
            spriteRenderer.sprite = front;            
        }
        else
        {
            spriteRenderer.sprite = cardBack;            
        }
    }

    //Cuando entres al editor de Unity y veas el prefab de una carta (Card), vas a notar que:
    //      - Tiene un componente *SpriteRenderer* → este muestra la imagen.
    //      - Tiene el script *CardModel* adjunto.
    //      - Desde este script, se pueden modificar (en el Inspector) los valores de cardBack(imagen trasera), y luego en tiempo de ejecución se le asignará front y value.

}
