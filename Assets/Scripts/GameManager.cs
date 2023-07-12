using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{

    // 单例模式
    private static GameManager instance;
    public static GameManager Instance {get {return instance;}}

    // 初始生命值和当前生命值
    private const int InitLives = 3;
    private int lives = InitLives;
    // 关卡数
    public static int level = 1;
    // 当前砖块数
    public int brickNum;
    // 记录游戏状态
    public bool isPlaying = false;
    public bool isPassed = false;
    public bool isLosed = false;
    
    public Sprite[] brickSprites;
    public Text lifeText;
    public Text levelText;
    public GameObject startText;
    public GameObject winText;
    public GameObject loseText;
    
    private void Awake() {
        if(instance != null){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    private void Start(){

        brickNum = FindObjectsOfType<Brick>().Length;

        // 初始化
        Init();
    }

    private void Update() {
        // 通关或者失败重新加载场景
        if(isPassed || isLosed){
            if(Input.GetKeyDown(KeyCode.N)){
                ReloadScene();
            }
        }
    }

    private void OnDestroy() {
        if(instance == this){
            instance = null;
        }
    }

    private void Init(){
        // 初始化游戏状态
        isPlaying = false;
        isPassed = false;
        isLosed = false;

        // 初始化血量并设置血量文本
        lives = InitLives;
        SetLifeText();

        // 设置关卡文本
        SetLevelText();
    }

    // 设置关卡文本
    private void SetLevelText(){
        levelText.text = "关卡 " + level;
    }

    // 设置血量文本
    private void SetLifeText(){
        lifeText.text = "生命值：" + lives;
    }

    // 改变生命值
    private void ChangeLives(int delta){
        lives += delta;
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
        if(lives == 0){
            // 失败提示
            loseText.SetActive(true);

            // 游戏失败，关卡置1
            isLosed = true;
            level = 1;
        }
    }

    // 检查是否通关，查看当前砖块数量是否为0
    public void CheckPassed(){
        if(brickNum == 0){
            // 通关提示
            winText.SetActive(true);
            // 慢动作
            Time.timeScale = 0.2f;
            Invoke(nameof(WinStep), 0.2f);
        }
    }

    // 恢复时间，通关延时调用的函数
    private void WinStep(){
        Time.timeScale = 1f;
        isPlaying = false;
        isPassed = true;
        level++;
    }

    // 重新加载游戏界面
    private void ReloadScene(){
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
}
