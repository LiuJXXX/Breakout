using System;
using UnityEngine;

public class Brick : MonoBehaviour{

    private const float AddHealthProb = 0.1f;
    private int health = 1;
    private SpriteRenderer spriteRenderer;
    public GameObject breakingAnimation;
    public AudioSource hitAudio;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        Init(GameManager.level);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // 播放音效
        hitAudio.Play();

        // 被击中时，砖块血量减1
        health--;
        // 砖块剩余血量大于0，变更砖块颜色
        if(health > 0){
            spriteRenderer.sprite = GameManager.Instance.brickSprites[health - 1];
            return;
        }

        // 砖块血量为0时消失
        GameManager.Instance.brickNum--;
        GameManager.Instance.CheckPassed();
        Destroy(gameObject);

        // 播放爆炸动画特效
        Instantiate(breakingAnimation, transform.position, Quaternion.identity);
    }

    // 根据level初始化砖块的血量和图案
    public void Init(int level){
        int brickSpritesLength = GameManager.Instance.brickSprites.Length;

        for(int i = 1; i < level; ++i){
            if(UnityEngine.Random.Range(0f, 1f) <= AddHealthProb){
                health = Math.Min(health + 1, brickSpritesLength);
            }
        }

        spriteRenderer.sprite = GameManager.Instance.brickSprites[health - 1];
    }
}
