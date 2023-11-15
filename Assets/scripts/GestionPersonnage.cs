using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
public class GestionPersonnage : MonoBehaviour
{
    public float vitesse;
    public float hauteurSaut;
    float forceSaut;
    float forceAvant;
    float forceCote;
    Rigidbody rigidPersonnage;

    float forceRotationX;
    float forceRotationY;
    public float vitesseRotaton;

    public GameObject camera1Personne;
    //gestion des animation
    Animator brasAnime;
    public GameObject bras;
    //gestion des arme
    public GameObject pistolet;
    public float puissanceArme;
    float forceTir;
    //gestion du compteur
    public TextMeshProUGUI compteur;
    int nbDeCafardTue = 0;
    //cr�ation d'un infocollision pour stoqu� les information des raycast
    RaycastHit infoCollision;
    //systemes de particules
    public ParticleSystem coupDeFeu;
    void Start()
    {
        //set le rigidbody et l'animator dans des variable
        rigidPersonnage = GetComponent<Rigidbody>();
        brasAnime = bras.GetComponent<Animator>();
        //set le compteur de cafard � 0
        compteur.text = "0";
        //enlever le curseur de l'�cran
        Screen.lockCursor = true;
        //cacher le pistolet
        pistolet.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        /*
         gestion des d�placement
        */
        //appliquer la force selement quand le joueur est au sol
        bool auSol = Physics.SphereCast(transform.position + new Vector3(0, 0.5f, 0), 0.2f, -Vector3.up, out infoCollision, 2.1f);//2.1 �gale � la hauteur du personnage 
        if (auSol)
        {
            forceAvant = Input.GetAxis("Vertical") * vitesse;
            forceCote = Input.GetAxis("Horizontal") * vitesse;
        }
        else
        {
            forceAvant = 0;
            forceCote = 0;
        }

        /*
        gestion de la cam�ra 
        */
        forceRotationX = Input.GetAxis("Mouse X") * vitesseRotaton;
        forceRotationY = Input.GetAxis("Mouse Y") * -vitesseRotaton;
        //si le mouvement est de gauche � droite, alors tout le corps bouge
        transform.Rotate(0f, forceRotationX, 0f);
        //si le mouvement est de haut en bas, juste la cam�ra bouge
        //print(camera1Personne.transform.rotation.x > 0.70f);
/*        if (camera1Personne.transform.rotation.x > 0.70f)
        {
            print("+80");
            if (forceRotationY < 0)
            {
                forceRotationY = 0;
                print("force 0");
            }
        }
        if (camera1Personne.transform.rotation.x < -0.70f)
        {
            print("2e pos");
            if (forceRotationY < 0)
            {
                forceRotationY = 0;
            }
        }*/
        camera1Personne.transform.Rotate(forceRotationY, 0f, 0f);

        /*
         utilisation de l'espace pour sauter
        */
        Debug.DrawRay(transform.position, -Vector3.up * 2.2f, Color.blue);

        if (Input.GetKeyDown(KeyCode.Space) && auSol)
        {
            forceSaut = hauteurSaut;
        }
        else if (auSol)
        {
            rigidPersonnage.drag = 10f;
        }
        //quand il n'est plus au sol
        else
        {
            
            rigidPersonnage.drag = 0.5f;
        }

        //gestion de la course
        if (Input.GetKey(KeyCode.LeftShift))
        {
            forceAvant *= 2;
        }

        /* 
         * gestion du raycast por d�tecter les objets int�racifs
        */
        Ray camSouris = Camera.main.ScreenPointToRay(Input.mousePosition);


        // ouvrir / fermer portes et contenant avec la d�tection du raycast

        if( Physics.Raycast(camSouris.origin, camSouris.direction, out infoCollision, 3f, LayerMask.GetMask("Ouvrable")))
        {
            //appuyer sur E et ne pas avoir le pistolet en main pour int�ragir
            if (Input.GetKeyDown(KeyCode.E))
            {
                //si la porte touch� est enfant de la porte qui poss�de l'animator
                //alors l'animator d�clenche une �rreur et on catch le parent de la porte
                try
                {
                    infoCollision.collider.gameObject.GetComponent<Animator>().SetTrigger("ouvrirPorte");
                }
                catch
                {
                    infoCollision.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("ouvrirPorte");
                    //print(infoCollision.transform.parent.name);
                }
                    brasAnime.SetTrigger("interagir");
            }
            Debug.DrawRay(camSouris.origin, camSouris.direction * 100, Color.yellow);
        }

        /**
         * gestion des cafard
         */
        //Gestion du tir de l'arme
        if (Input.GetMouseButtonDown(0) && brasAnime.GetComponent<Animator>().GetBool("pistolet"))
        {
            //raycast qui d�tecte les cafards, si le joueur � tirer
            if (Physics.Raycast(camSouris.origin, camSouris.direction, out infoCollision, 500f, LayerMask.GetMask("Cafards")))
            {
                //d�truire le cafard s'il est touch� par le tir
                infoCollision.collider.gameObject.SetActive(false);
                print("touch�");
                //incr�menter le nombre de cafard tu�
                nbDeCafardTue++;
                //�crire le nouveau score
                compteur.text=nbDeCafardTue.ToString();
            }
            //ajouter une force de recule � l'arme
            forceTir = puissanceArme;
            camera1Personne.transform.Rotate(-forceTir, 0f, 0f);
            //afficher les particules du coup de feu
            coupDeFeu.Play();

        }
            Debug.DrawRay(camSouris.origin, camSouris.direction * 100, Color.red);

        /*
        Gestion des animations 
        */
        //gestion des armes
        //si le joueur scroll, l'arme passe a la prochaine
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            brasAnime.SetBool("pistolet", true);
            Invoke("FaireApparaitrePistolet", 0.25f);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            brasAnime.SetBool("pistolet", false);
            Invoke("FaireApparaitrePistolet", 0.25f);
        }
        //set la vitesse de l'animator pour qu'elle soit �gal � la vitesse du personnage
        brasAnime.SetFloat("vitesse", forceAvant);
        
    }
    void FixedUpdate()
    {
        //apliquer les forces de d�placement
        rigidPersonnage.AddRelativeForce(forceCote, forceSaut, forceAvant , ForceMode.VelocityChange);
        //rigidPersonnage.velocity = (Vector3.forward*forceAvant)+ (Vector3.right * forceCote)+ (Vector3.up * forceSaut) + new Vector3(0,0,0);
        //r�nitialiser les force apres leur utilisation
        forceSaut = 0;
        forceTir = 0; ;
    }
    void FaireApparaitrePistolet()
    {
        if (brasAnime.GetBool("pistolet") ==true)
        {
        pistolet.SetActive(true);
        print("boucle Active");
        }
        else
        {
        pistolet.SetActive(false);
        print("boucle non-Active");
        }
    }
}
