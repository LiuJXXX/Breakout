using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour{
    
    private const float BounceAngle = 5;
    private const int InitAttack = 1;
    private Rigidbody2D _rb;
    private Transform _paddleTransform;
    private Vector3 _initPos;
    private Vector3 _nextPos;

    public float speed = 1;
    public float currentSpeed;
    public int attack;

    private void Awake(){
        _rb = GetComponent<Rigidbody2D>();
        _initPos = transform.position;
        _nextPos = _initPos;
        currentSpeed = speed;
        attack = InitAttack;
    }

    private void Start() {
        _paddleTransform = FindObjectOfType<Paddle>().transform;
    }

    private void FixedUpdate() {
        // 游戏未开始时，球要跟着板子移动
        if(!GameManager.Instance.isPlaying){
            _nextPos.x = _paddleTransform.position.x;
            _rb.MovePosition(_nextPos);
        }
    }

    private void Update(){
        // 通关或者失败不允许弹出小球
        if(GameManager.Instance.isPassed || GameManager.Instance.isLost){
            return;
        }

        // 按下空格键开始游戏
        if(Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.isPlaying){

            GameManager.Instance.isPlaying = true;

            // 生成随机一个方向
            Vector2 velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
            _rb.velocity = velocity * currentSpeed;

            // 关闭开始游戏的文本
            GameManager.Instance.CloseStartText();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!GameManager.Instance.isPlaying) {
            return;
        }

        // 碰到地板
        if (other.gameObject.CompareTag("Floor")) {
            GameManager.Instance.GameOver();
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        // 碰撞结束重置速度的大小
        Vector2 sp = _rb.velocity.normalized;

        // 对速度的方向进行约束，防止一直垂直或水平弹射
        // 垂直弹射
        if(sp.x == 0){
            // 向左偏移
            float tmpX = Mathf.Tan(BounceAngle * Mathf.Deg2Rad);
            Vector2 newVelocity = new Vector2(-tmpX, 1f).normalized;
            sp = newVelocity;
        }
        // 水平弹射
        else if(sp.y == 0){
            // 向下偏移
            float tmpY = Mathf.Tan(BounceAngle * Mathf.Deg2Rad);
            Vector2 newVelocity = new Vector2(1f, -tmpY).normalized;
            sp = newVelocity;
        }

        // 弹射夹角大于90-BounceAngle度或小于BounceAngle度，计算向量与水平方向以及垂直方向组成的直角三角形是否有锐角小于BounceAngle度
        if(Mathf.Asin(Mathf.Abs(sp.y) / 1) * Mathf.Rad2Deg < BounceAngle || Mathf.Asin(Mathf.Abs(sp.x) / 1) * Mathf.Rad2Deg < BounceAngle){
            // 将速度在水平方向和垂直方向上较小的分量增大至弹射夹角大于80度或小于10度
            float tmp = Mathf.Tan(BounceAngle * Mathf.Deg2Rad);
            float newX = Mathf.Sign(sp.x) * (Mathf.Abs(sp.x) > Mathf.Abs(sp.y) ? 1f : tmp);
            float newY = Mathf.Sign(sp.y) * (Mathf.Abs(sp.y) > Mathf.Abs(sp.x) ? 1f : tmp);
            Vector2 newVelocity = new Vector2(newX, newY).normalized;
            sp = newVelocity;
        }

        _rb.velocity = sp * currentSpeed;
    }

    // 重置球的位置
    private void ResetPos() {
        _rb.MovePosition(_initPos);
    }
    
    // 根据给定倍率设置球的速度
    public void SetSpeed(float speedRate = 1.0f) {
        currentSpeed = speed * speedRate;
        // 需要直接修改刚体的速度，否则只有在下次反弹才会改变速度
        Vector2 sp = _rb.velocity.normalized;
        _rb.velocity = sp * currentSpeed;
    }

    // 根据给定值增加球的攻击力
    public void SetAttack(int increasedValue){
        attack += increasedValue;
    }

    // 重置球的攻击力
    private void ResetAttack(){
        attack = InitAttack;
    }
    
    // 重置球的角速度
    public void ResetRotate() {
        _rb.angularVelocity = 0;
        transform.rotation = Quaternion.identity;
    }

    // 初始化小球的位置、速度和攻击力
    public void ResetBall(){
        ResetPos();
        SetSpeed();
        ResetAttack();
        ResetRotate();
    }
}
