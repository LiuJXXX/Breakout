using UnityEngine;

public class Ball : MonoBehaviour{
    
    private Rigidbody2D rb;
    private Transform paddleTransform;

    public float speed = 1;
    [HideInInspector]
    public float ballInitYPos;
    
    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        ballInitYPos = transform.position.y;
        paddleTransform = FindObjectOfType<Paddle>().transform;
    }

    private void Update(){
        // 游戏未开始时，球要跟着板子移动
        if(!GameManager.Instance.isPlaying){
            transform.position = new Vector3(paddleTransform.position.x, ballInitYPos);
        }

        // 通关或者失败不允许弹出小球
        if(GameManager.Instance.isPassed || GameManager.Instance.isLosed){
            return;
        }

        // 按下空格键开始游戏
        if(Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.isPlaying){

            GameManager.Instance.isPlaying = true;

            // 生成随机一个方向
            Vector2 velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
            rb.velocity = velocity * speed;

            // 关闭开始游戏的文本
            GameManager.Instance.CloseStartText();
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(GameManager.Instance.isPlaying){
            // 碰撞结束重置速度的大小
            Vector3 sp = rb.velocity.normalized;

            // 对速度的方向进行约束，防止一直垂直或水平弹射
            // 垂直弹射
            if(sp.x == 0){
                // 向左偏移
                float tmpX = Mathf.Tan(10 * Mathf.Deg2Rad);
                Vector3 newVelocity = new Vector3(-tmpX, 1f, 0).normalized;
                sp = newVelocity;
            }
            // 水平弹射
            else if(sp.y == 0){
                // 向下偏移
                float tmpY = Mathf.Tan(10 * Mathf.Deg2Rad);
                Vector3 newVelocity = new Vector3(1f, -tmpY, 0).normalized;
                sp = newVelocity;
            }

            // 弹射夹角大于80度或小于10度，计算向量与水平方向以及垂直方向组成的直角三角形是否有锐角小于10度
            if(Mathf.Asin(Mathf.Abs(sp.y) / 1) * Mathf.Rad2Deg < 10 || Mathf.Asin(Mathf.Abs(sp.x) / 1) * Mathf.Rad2Deg < 10){
                // 将速度在水平方向和垂直方向上较小的分量增大至弹射夹角大于80度或小于10度
                float tmp = Mathf.Tan(10 * Mathf.Deg2Rad);
                float newX = Mathf.Sign(sp.x) * (Mathf.Abs(sp.x) > Mathf.Abs(sp.y) ? 1f : tmp);
                float newY = Mathf.Sign(sp.y) * (Mathf.Abs(sp.y) > Mathf.Abs(sp.x) ? 1f : tmp);
                Vector3 newVelocity = new Vector3(newX, newY, 0).normalized;
                sp = newVelocity;
            }

            rb.velocity = sp * speed;
        }
    }
}
