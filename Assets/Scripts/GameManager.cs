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
    // 关卡数
    public static int Level = 1;
    // 当前砖块数
    public int brickNum;
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
    
    private void Awake() {
        if(_instance != null){
            Destroy(gameObject);
        }
        else{
            _instance = this;
        }
    }

    private void Start(){

        brickNum = FindObjectsOfType<Brick>().Length;

        // 初始化
        Init();
    }

    private void Update() {
        // 通关或者失败重新加载场景
        if(isPassed || isLost){
            if(Input.GetKeyDown(KeyCode.N)){
                ReloadScene();
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
}
