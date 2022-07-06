using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    private int price;
    [SerializeField]
    private CharacterStatAbility ability;
    [SerializeField]
    private float buyableMouseDistance;
    private bool isBuyable = false;

    [SerializeField]
    private GameObject soldoutPrefab;

    private Shopkeeper shopkeeper;

    public void Init(Shopkeeper shopkeeper)
    {
        this.shopkeeper = shopkeeper;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스와 아이템 거리
            float dist = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // 클릭이 안되었으면 리턴
            if (dist > buyableMouseDistance) return;

            // 클릭은 되었는데, 플레이어가 앞에 없으면 메시지 출력 후, 리턴
            if (!isBuyable)
            {
                shopkeeper.Explain("구매하려는 물건 앞으로 가까이 와주세요.");
                return;
            }

            // 구매
            Buy();
        }
    }

    public void Buy()
    {
        // 현재 플레이어 코인이 가격보다 적으면
        if (Player.CurrentCoinCount < price)
        {
            shopkeeper.Explain("돈이 충분하지 않네요.\n다음에 와주세요!");
            return;
        }

        // 코인 차감
        Player.IncreaseCoinCount(-price);

        // 어빌리티 적용
        var players = Player.Players;
        foreach (var player in players)
        {
            ability.SetCharacter(player);
            ability.OnClickAbility();

            // 체력 관련은 한 번만 적용
            if (ability.TargetStat == CharacterStatType.Health || ability.TargetStat == CharacterStatType.CurrentHp)
                break;
        }

        // 매진 표시
        Instantiate(soldoutPrefab, transform.position, Quaternion.identity);

        // 감사 메시지 출력
        shopkeeper.Explain("구매해주셔서 감사합니다.");

        // 삭제
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player Foot")) return;

        Player player = other.GetComponentInParent<Player>();

        if (!player) return;
        if (player != Player.Main) return;

        // 설명 메시지 출력
        shopkeeper.Explain(ability.AbilityDescription);

        // 구매 가능
        isBuyable = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player Foot")) return;

        Player player = other.GetComponentInParent<Player>();

        if (!player) return;
        if (player != Player.Main) return;

        isBuyable = false;
    }
}
