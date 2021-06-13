using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SmootFollowTarget camFollow;
    [SerializeField] Rigidbody head;
    [SerializeField] Rigidbody foot1, foot2, hand;
    [SerializeField] GameObject mainOst;
    [SerializeField] FollowSpline doggo;

    [Header("UI")]
    [SerializeField] Button play;
    [SerializeField] Button credits, quit;

    float storedSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        storedSpeed = doggo.Speed;
        doggo.Speed = 0;
        mainOst.SetActive(false);
        hand.isKinematic = false;
        camFollow.enabled = false;
        head.isKinematic = true;
        foot1.isKinematic = true;
        foot2.isKinematic = true;
        play.onClick.AddListener(Play);
        quit.onClick.AddListener(() => Application.Quit());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Play()
    {
        camFollow.enabled = true;
        hand.GetComponent<Leash>().enabled = true;
        hand.isKinematic = true;
        head.isKinematic = false;
        foot1.isKinematic = false;
        foot2.isKinematic = false;
        doggo.Speed = storedSpeed;
        mainOst.SetActive(true);
        gameObject.SetActive(false);
    }
}
