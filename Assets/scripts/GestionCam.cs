using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionCam : MonoBehaviour
{
    public GameObject joueur; 

    // Start is called before the first frame update
    void Start()
    {
        //mettre cette caméra active au début du jeu
        gameObject.SetActive(true);
        //s'assurer que le joueur est désactivé
        joueur.SetActive(false);
    }
    void changerDeCamera()
    {
        //faire apparaitre le personnage quand la caméra change de vue
        joueur.SetActive(true);
        //désactiver la cam pour laisser la vu en première personne
        gameObject.SetActive(false);
    }
}
