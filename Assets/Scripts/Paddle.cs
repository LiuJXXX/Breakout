using UnityEngine;

public class Paddle : MonoBehaviour{

    public float xMin;
    public float xMax;
    public float speed;

    private void Update(){

        // 读取输入
        float x = Input.GetAxisRaw("Horizontal");
        if(x != 0){
            Vector2 pos = transform.position;
            pos.x += x * Time.deltaTime * speed;
            // 限制x坐标的大小
            pos.x = Mathf.Clamp(pos.x, xMin, xMax);
            transform.position = pos;
        }
    }
}
