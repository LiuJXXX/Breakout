using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{

    // 单例模式
    private static GameManager _instance;
    public static GameManager Instance {get {return _instance;}}

    // 初始生命值和当前生命值
    private const int InitLives = 3;
    private int _lives = InitLives;
    // 慢动作
    private const float SlowMotionRate = 0.2f;
    private const float SlowMotionTime = 0.2f;
    // 砖块的坐标数组
    private static float[,] _bricksPos =
    {
        {-2.5f, 3.5f}, {-1.25f, 3.5f}, {0f, 3.5f}, {1.25f, 3.5f}, {2.5f, 3.5f},
        {-2.5f, 3f}, {-1.25f, 3f}, {0f, 3f}, {1.25f, 3f}, {2.5f, 3f},
        {-2.5f, 2.5f}, {-1.25f, 2.5f}, {0f, 2.5f}, {1.25f, 2.5f}, {2.5f, 2.5f}
    };
    // 砖块数组
    private List<Brick> _bricks = new List<Brick>();
    
    // 道具有效时间
    private const float ItemValidTime = 2f;
    // 道具效果检测相关
    private int _currentBallItemCount;
    private int _currentPaddleItemCount;
    private GameObject _bricksParent;
    private GameObject _itemsParent;
    
    public Ball ball;
    public Paddle paddle;
    // 当前砖块数
    public int brickNum;
    // 关卡数
    public static int Level = 1;
    // 记录游戏状态
    public bool isPlaying;
    public bool isPassed;
    public bool isLost;
    
    public Sprite[] brickSprites;
    public Text lifeText;
    public Text levelText;
    public GameObject startText;
    public GameObject winText;
    public GameObject loseText;
    public GameObject brickPrefab;
    public AudioSource hitAudio;
    public GameObject itemPrefab;

    private void Awake() {
        if(_instance != null){
            Destroy(gameObject);
        }
        else{
            _instance = this;
        }
    }

    private void Start(){

        // 生成砖块的父节点
        _bricksParent = new GameObject("Bricks");
        _bricksParent.transform.position = Vector3.zero;

        // 生成道具的父节点
        _itemsParent = new GameObject("Items");
        _itemsParent.transform.position = Vector3.zero;
        
        // 初始化
        Init();
    }

    private void Update() {
        // 通关或者失败重新加载场景
        if(isPassed || isLost){
            if(Input.GetKeyDown(KeyCode.N)){
                // ReloadScene();
                Init();
            }
        }
    }

    private void OnDestroy() {
        if(_instance == this){
            _instance = null;
        }
    }

    private void Init(){
        // 初始化游戏状态
        isPlaying = false;
        isPassed = false;
        isLost = false;

        // 初始化血量并设置血量文本
        _lives = InitLives;
        SetLifeText();

        // 设置关卡文本
        SetLevelText();
        
        // 提示文本
        startText.SetActive(true);
        winText.SetActive(false);
        loseText.SetActive(false);
        
        // 初始化小球和平板
        ball.ResetPos();
        ball.SetSpeed();
        _currentBallItemCount = 0;
        paddle.ResetPos();
        paddle.SetLength();
        _currentPaddleItemCount = 0;

        // 关闭所有协程
        StopAllCoroutines();
        
        // 清除所有道具
        ClearItems();
        
        // 初始化砖块
        if (_bricks.Count == 0) {
            brickNum = _bricksPos.Length / 2;
            for (int i = 0; i < brickNum; ++i) {
                GameObject brick = Instantiate(brickPrefab, new Vector3(_bricksPos[i, 0], _bricksPos[i, 1]), Quaternion.identity);
                brick.transform.SetParent(_bricksParent.transform);
                _bricks.Add(brick.GetComponent<Brick>());
            }
        }
        else {
            brickNum = _bricks.Count;
            foreach (var brick in _bricks) {
                // 游戏失败时，如果不先关闭它们，有的砖块不会被重置
                brick.gameObject.SetActive(false);
                brick.gameObject.SetActive(true);
            }
        }
    }

    // 设置关卡文本
    private void SetLevelText(){
        levelText.text = "关卡 " + Level;
    }

    // 设置血量文本
    private void SetLifeText(){
        lifeText.text = "生命值：" + _lives;
    }

    // 改变生命值
    private void ChangeLives(int delta){
        _lives += delta;
        SetLifeText();
    }

    // 关闭开始游戏的文本
    public void CloseStartText(){
        startText.SetActive(false);
    }

    // 未接到小球，生命值减1，游戏结束
    public void GameOver(){
        isPlaying = false;

        // 生命值减1
        ChangeLives(-1);
        if(_lives == 0){
            // 失败提示
            loseText.SetActive(true);

            // 游戏失败，关卡置1
            isLost = true;
            Level = 1;
        }
    }

    // 检查是否通关，查看当前砖块数量是否为0
    public void CheckPassed(){
        if(brickNum == 0){
            // 通关提示
            winText.SetActive(true);
            // 慢动作
            Time.timeScale = SlowMotionRate;
            Invoke(nameof(WinStep), SlowMotionTime);
        }
    }

    // 恢复时间，通关延时调用的函数
    private void WinStep(){
        Time.timeScale = 1f;
        isPlaying = false;
        isPassed = true;
        Level++;
    }

    // 重新加载游戏界面
    private void ReloadScene(){
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
    
    // 播放音效
    public void PlayHitAudio() {
        hitAudio.Play();
    }
    
    // 改变球的速度
    public void ChangeBallSpeed(float speedRate = 1.0f) {
        _currentBallItemCount++;
        ball.SetSpeed(speedRate);
        
        // 恢复球的速度
        StartCoroutine(ChangeBallSpeedCoroutine());
    }
    
    // 改变平板的大小
    public void ChangePaddleLength(float lengthRate = 1.0f) {
        _currentPaddleItemCount++;
        paddle.SetLength(lengthRate);
        
        // 恢复平板大小
        StartCoroutine(ChangePaddleLengthCoroutine());
    }
    
    // 协程，改变球的速度
    private IEnumerator ChangeBallSpeedCoroutine() {
        yield return new WaitForSeconds(ItemValidTime);
        // 恢复球的速度
        if (_currentBallItemCount == 1) {
            print("效果恢复");
            ball.SetSpeed();
        }
        _currentBallItemCount--;
    }
    
    // 协程，改变平板的大小
    private IEnumerator ChangePaddleLengthCoroutine() {
        yield return new WaitForSeconds(ItemValidTime);
        // 恢复平板大小
        if (_currentPaddleItemCount == 1) {
            print("效果恢复");
            paddle.SetLength();
        }
        _currentPaddleItemCount--;
    }
    
    // 生成道具
    public void GenerateItem(ItemType itemType, Vector3 pos) {
        GameObject item = Instantiate(itemPrefab, pos, Quaternion.identity);
        item.GetComponent<Item>().SetItemTypeAndItemSprite(itemType);
        item.transform.SetParent(_itemsParent.transform);
    }
    
    // 清除所有道具
    private void ClearItems() {
        foreach (Transform child in _itemsParent.transform) {
            Destroy(child.gameObject);
        }
    }
}
