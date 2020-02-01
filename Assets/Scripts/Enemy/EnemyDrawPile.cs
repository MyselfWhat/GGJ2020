using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDrawPile : MonoBehaviour
{    
    [SerializeField]
    private List<EnemyCard> _startingCards;

    [SerializeField]
    private EnemyDiscardPile _discardPile;

    private EnemyHand _hand;

    public int CardCount { 
        get {
            return transform.childCount;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _discardPile = FindObjectOfType<EnemyDiscardPile>();
        _hand = FindObjectOfType<EnemyHand>();
    }

    public void Init(List<EnemyCard> cards)
    {
        _startingCards = cards;

        _startingCards = Shuffle(_startingCards);
        
        foreach (var card in _startingCards)
        {
            var cardObject = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
            cardObject.transform.SetParent(transform, false);
        }
    }

    public List<EnemyCard> Shuffle(List<EnemyCard> cards)
    {
        return cards.OrderBy(a => Random.Range(0, 1000)).ToList();
    }

    public List<Transform> Shuffle(List<Transform> cards)
    {
        return cards.OrderBy(a => Random.Range(0, 1000)).ToList();
    }

    public List<Transform> DrawCards(int drawAmount)
    {
        if (CardCount == 0)
        {
            GrabDiscardPile();
        }

        var drawnCards = new List<Transform>();

        var cardsInDrawPile = new List<Transform>();
        foreach (Transform card in transform)
        {
            cardsInDrawPile.Add(card);
        }

        for (var i = 0; i < drawAmount; i++)
        {
            if (!cardsInDrawPile.Any()) {
                GrabDiscardPile();

                cardsInDrawPile = new List<Transform>();
                foreach (Transform card in transform)
                {
                    cardsInDrawPile.Add(card);
                }

                if(!cardsInDrawPile.Any()) {
                    return drawnCards;
                }
            }

            var drawnCard = cardsInDrawPile.FirstOrDefault();

            drawnCards.Add(drawnCard);
            cardsInDrawPile.Remove(drawnCard);
        }

        return drawnCards;
    }

    private void GrabDiscardPile()
    {
        var cardsInDiscardPile = new List<Transform>();
        foreach (Transform card in _discardPile.transform)
        {
            cardsInDiscardPile.Add(card);
        }
        
        cardsInDiscardPile = Shuffle(cardsInDiscardPile);

        foreach (var card in cardsInDiscardPile)
        {
            card.SetParent(transform, false);
        }

        Debug.Log("Enemy Discard pile shuffled into draw pile.");
    }
}