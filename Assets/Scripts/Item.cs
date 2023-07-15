using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemType _itemType = ItemType.None;
    private const float BallAccelerationRate = 1.3f;
    private const float BallDecelerationRate = 0.7f;
    private const float PaddleExtensionRate = 1.5f;
    private const float PaddleRetractionRate = 0.8f;
    private const float FallingSpeed = 3f;

    private void FixedUpdate() {
        Vector3 nextPos = transform.position;
        // FixedUpdate中要使用Time.fixedDeltaTime
        nextPos.y -= Time.fixedDeltaTime * FallingSpeed;
        transform.position = nextPos;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // 正在游戏，且平板接到道具
        if (GameManager.Instance.isPlaying && other.gameObject.CompareTag("Paddle")) {
            switch (_itemType)
            {
                case ItemType.BallAcceleration:
                    GameManager.Instance.ChangeBallSpeed(BallAccelerationRate);
                    print("小球加速");
                    break;
                case ItemType.BallDeceleration:
                    GameManager.Instance.ChangeBallSpeed(BallDecelerationRate);
                    print("小球减速");
                    break;
                case ItemType.PaddleExtension:
                    GameManager.Instance.ChangePaddleLength(PaddleExtensionRate);
                    print("平板拉长");
                    break;
                case ItemType.PaddleRetraction:
                    GameManager.Instance.ChangePaddleLength(PaddleRetractionRate);
                    print("平板缩短");
                    break;
            }
            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("Floor")){
            // 掉到地面则消失
            Destroy(gameObject);
        }
            
    }

    // 设置道具类型和图案
    public void SetItemTypeAndItemSprite(ItemType itemType) {
        _itemType = itemType;
        // 设置图案
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.itemSprites[(int)_itemType];
    }
}