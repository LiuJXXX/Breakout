using System;
using UnityEngine;

public class Brick : MonoBehaviour{

    private const float AddHealthProb = 0.1f;
    private const float ItemProb = 0.5f;
    private int _health = 1;
    private ItemType _itemType = ItemType.None;
    private SpriteRenderer _spriteRenderer;
    public GameObject breakingAnimation;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        Init(GameManager.Level);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // 播放音效
        GameManager.Instance.PlayHitAudio();

        // 被击中时，砖块血量减少小球攻击力的值
        _health -= GameManager.Instance.GetBallAttack();
        // 砖块剩余血量大于0，变更砖块颜色
        if(_health > 0){
            _spriteRenderer.sprite = GameManager.Instance.brickSprites[_health - 1];
            return;
        }

        // 砖块血量为0时消失，将其设为false
        GameManager.Instance.brickNum--;
        gameObject.SetActive(false);

        // 生成道具
        if (_itemType != 0) {
            GameManager.Instance.GenerateItem(_itemType, transform.position);
        }
        
        // 播放爆炸动画特效
        Instantiate(breakingAnimation, transform.position, Quaternion.identity);

        // 检查是否通关
        GameManager.Instance.CheckPassed();
    }

    // 根据level初始化砖块的血量和图案
    private void Init(int level) {
        // 初始化血量
        _health = 1;
        
        int brickSpritesLength = GameManager.Instance.brickSprites.Length;
        for(int i = 1; i < level; ++i){
            if(UnityEngine.Random.Range(0f, 1f) <= AddHealthProb){
                _health = Math.Min(_health + 1, brickSpritesLength);
            }
        }

        // 初始化图案
        _spriteRenderer.sprite = GameManager.Instance.brickSprites[_health - 1];
        
        // 初始化道具
        if(UnityEngine.Random.Range(0f, 1f) <= ItemProb)
        {
            int itemIdx = UnityEngine.Random.Range(1, Enum.GetNames(typeof(ItemType)).Length);
            _itemType = (ItemType)itemIdx;
        }
    }
}
