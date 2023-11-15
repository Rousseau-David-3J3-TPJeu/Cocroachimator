using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionCam : MonoBehaviour
{
    public GameObject joueur; 

    // Start is called before the first frame update
    void Start()
    {
        //mettre cette cam�ra active au d�but du jeu
        gameObject.SetActive(true);
        //s'assurer que le joueur est d�sactiv�
        joueur.SetActive(false);
    }
    void changerDeCamera()
    {
        //faire apparaitre le personnage quand la cam�ra change de vue
        joueur.SetActive(true);
        //d�sactiver la cam pour laisser la vu en premi�re personne
        gameObject.SetActive(false);
    }
}
