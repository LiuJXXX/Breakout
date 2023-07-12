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
    
    private void Update(){

        // 读取输入
        float x = Input.GetAxisRaw("Horizontal");
        if(x != 0){
            Vector3 pos = transform.position;
            pos.x += x * Time.deltaTime * speed;
            // 限制x坐标的大小
            pos.x = Mathf.Clamp(pos.x, xMin + _collider.bounds.size.x / 2, xMax - _collider.bounds.size.x / 2);
            transform.position = pos;
        }
    }
    
    // 重置平板的位置
    public void ResetPos() {
        transform.position = _initPos;
    }
    
    // 改变平板的大小
    public void SetLength(float lengthRate = 1.0f) {
        transform.localScale = new Vector3(lengthRate, 1.0f, 1.0f);
    }
}
