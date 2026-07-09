using System;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    // [SerializeField] private HandView handView;
    // [SerializeField] private CardData cardData;


    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         Card card = new(cardData);

    //         //生成位置在TestSystem上；Quaternion.identity意为单位四元数，无旋转
    //         CardView cardView = CardViewCreator.Instance.CreateCardView(card, transform.position, Quaternion.identity);
    //         StartCoroutine(handView.AddCard(cardView));
    //     }
    // }

    [SerializeField] private List<CardData> deckData;
    private void Start()
    {
        CardSystem.Instance.Setup(deckData);
    }


    // // Update is called once per frame
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         Card card = new(cardData);

    //         //生成位置在TestSystem上；Quaternion.identity意为单位四元数，无旋转
    //         CardView cardView = CardViewCreator.Instance.CreateCardView(card, transform.position, Quaternion.identity);
    //         StartCoroutine(handView.AddCard(cardView));
    //     }
    // }

}
