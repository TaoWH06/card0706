using DG.Tweening;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardPrefab;
    // public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    // {
    //     CardView cardView = Instantiate(cardPrefab, position, rotation);
    //     //利用DOTween设置放大动画
    //     //初始缩放为0
    //     cardView.transform.localScale = Vector3.zero;
    //     //缩放动画，0.15s内平滑放大至正常大小
    //     cardView.transform.DOScale(Vector3.one, 0.15f);
    //     cardView.Setup(card);
    //     return cardView;
    // }

    public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        CardView cardView = Instantiate(cardPrefab, position, rotation);
        //利用DOTween设置放大动画
        //初始缩放为0
        cardView.transform.localScale = Vector3.zero;
        //缩放动画，0.15s内平滑放大至正常大小
        cardView.transform.DOScale(Vector3.one, 0.15f);
        cardView.Setup(card);
        return cardView;
    }

}
