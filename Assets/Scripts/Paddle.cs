using UnityEngine;

public class Paddle : MonoBehaviour{

    private Vector3 _initPos;
    private Collider2D _collider;
    
    public float xMin;
    public float xMax;
    public float speed;

    private void Awake() {
        _initPos = transform.position;
        _collider = GetComponent<Collider2D>();
    }
    
    // 使用FixedUpdate更新坐标
    private void FixedUpdate(){
        // 读取输入
        float x = Input.GetAxisRaw("Horizontal");
        if(x != 0){
            Vector3 pos = transform.position;
            pos.x += x * Time.fixedDeltaTime * speed;
            // 限制x坐标的大小
            var colliderBoundsSizeX = _collider.bounds.size.x;
            pos.x = Mathf.Clamp(pos.x, xMin + colliderBoundsSizeX / 2, xMax - colliderBoundsSizeX / 2);
            transform.position = pos;
        }
    }
    
    // 重置平板的位置
    public void ResetPos() {
        transform.position = _initPos;
    }
    
    // 根据给定倍率设置平板的长度
    public void SetLength(float lengthRate = 1.0f) {
        transform.localScale = new Vector3(lengthRate, 1.0f, 1.0f);
    }

    // 初始化平板的位置和长度
    public void ResetPaddle(){
        ResetPos();
        SetLength();
    }
}
