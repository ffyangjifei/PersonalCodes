using UnityEngine;
using System.Collections;
using System;
/**************************************************
* 
* 创建者：		yangjifei
* 创建时间：	2017/07/20 11:36
* 描述：		决定钓鱼是否成功的小游戏
* 
**************************************************/
public class FishCaptureLogic : MonoBehaviour
{
    public Transform floatingArea;
    public floatingBlocks floatingFish;
    public FishCaptureLogic_Fish fishLogic;
    public floatingBlocks floatingHandle;
    public floatingBlocks floatingTreasure;
    public UISlider CaptureProgrss;
    public UISlider TreasureProgress;
    public GameObject PressInputButton;

    public event Action<bool,FishingManager.FishingData_CaptureItem,bool> CaptureResult;//结果回调

    public void Awake()
    {
        floatingDistance = floatingArea.localScale.y;
    }

    public void OnEnable()
    {
        PressInputButton.GetComponent<UIEventListener>().onPress += OnInputButtonPress;
    }
    public void OnDisable()
    { 
        PressInputButton.GetComponent<UIEventListener>().onPress -= OnInputButtonPress;
    }

    public void Update()
    {
        if (!FishingFTUEManager.blockCaptureAction)
        {
            UpdateFloatingFish();
            UpdateFloatingHanle();
            updateCaptureProgress();
            updateTreasureBoxProgress();
        }
    }

    #region 初始化设置
    [CNName("设置鱼的移动范围(0-100)")]
    public float setFishFloatingLimit = 100;
    [CNName("设置鱼的速度(距离/秒)")]
    public float setFishFloatingSpeed = 50;

    [CNName("设置滑块的尺寸(0-100)")]
    public float setHandleSize = 20;
    [CNName("设置滑块的模式 0匀速 1加速")]
    public float setHandleMode = 0;
    [CNName("设置滑块模式0的速度(距离/秒)")]
    public float setHandleFloatingSpeed = 50;
    [CNName("设置滑块模式1的加速度上(速度/秒)")]
    public float setHandleFloatingAccelerateSpeedUp = 50;
    [CNName("设置滑块模式1的加速度下(速度/秒)")]
    public float setHandleFloatingAccelerateSpeedDown = 50;
    [CNName("设置进度的初始值(0-100)")]
    public float setProgressDefaultVaule = 50;
    [CNName("设置进度的变化速度(值/秒)")]
    public float setProgressSpeed = 50;
    [ContextMenu("应用当前参数")]
    public void InitLogic()
    {
        handleMode = (int)setHandleMode;
        setFishParams(setFishFloatingLimit, setFishFloatingSpeed);
        setProgressParams(setProgressDefaultVaule,setProgressSpeed);
        setHandleParams_Normal(setHandleSize, setHandleFloatingSpeed);
        setHandleParams_Acc(setHandleSize, setHandleFloatingAccelerateSpeedUp, setHandleFloatingAccelerateSpeedDown);
        if (fishLogic!=null)
        {
            fishLogic.Test();
        }
    }
    public void InitLogic(FishingManager.FishingData_Rod rod, FishingManager.FishingData_CaptureFish fish,FishingManager.FishingData_Bait bait)
    {
        //InitLogic();
        //进度条
        var progressP = FishingManager.getDiffuculty_Progress(fish.getClassConfig().difficultyProgress);
        setProgressParams(progressP.ProgressDefaultValue, progressP.ProgressSpeed);
        //滑块
        var hanleP = FishingManager.getDiffuculty_Handle(rod.captureHandleDifficulty);
        handleMode = hanleP.HandleMode;
        if (hanleP.HandleMode == 0)
        {
            setHandleParams_Normal(hanleP.HandleSize,hanleP.HandleFloatingSpeed);
        }
        if (hanleP.HandleMode == 1)
        {
            setHandleParams_Acc(hanleP.HandleSize, hanleP.HandleFloatingAccelerateSpeedUp, hanleP.HandleFloatingAccelerateSpeedDown);
        }
        //鱼
        if (fishLogic != null)
        {
            fishLogic.setFishMovingParams(FishingManager.getDiffuculty_Fish(fish.fishClass));
        }
        //宝箱
        var treasureConfig = FishingManager.getTreasureConfig();
        int treasureAddition = 0;
        if (bait.additionType == (int)FishingManager.FishingBaitAdditionType.Package || bait.additionType == (int)FishingManager.FishingBaitAdditionType.AllType)
        {
            treasureAddition = bait.additionValue[(int)FishingManager.FishingBaitAdditionType.Package - 1];
        }
        setTreasureProgrssParams(treasureConfig.showRate + treasureAddition, treasureConfig.showDelay, treasureConfig.progressSpeed);

        targetFish = fish;
    }
    private FishingManager.FishingData_CaptureItem targetFish;
#endregion

    #region 宝箱
    private float treasureProgressSpeed =0.3f;
    private int treasureShowRate = 0;
    private FloatRange treasureShowDelay;

    private float treasureProgressValue = 0;
    private float treasureShowWaitTime = 0;
    private bool isShowTreasureBox;
    private bool isTreasureActived;
    private bool isGetTreasure;
    private void setTreasureProgrssParams(int showRate,FloatRange showDelay,int ProgressSpeed)
    {
        treasureShowRate = showRate;
        if (FishingFTUEManager.IsGuiding)
        {//引导不出宝箱
            treasureShowRate = 0;
        }
        treasureShowDelay = showDelay;
        treasureProgressSpeed = ProgressSpeed*0.01f;

        treasureProgressValue = 0;
        treasureShowWaitTime = 0;
        isShowTreasureBox = (treasureShowRate > UnityEngine.Random.Range(0, 100));
        floatingTreasure.target.gameObject.SetActive(false);
        isTreasureActived = false;
        isGetTreasure = false;
        treasureShowWaitTime = treasureShowDelay.getRandom();
    }
    private void updateTreasureBoxProgress(){
        if (!isGetTreasure&&isShowTreasureBox)
        {
            if (!isTreasureActived)
            {
                treasureShowWaitTime -= Time.deltaTime;
                if (treasureShowWaitTime<0)
                {
                    floatingTreasure.target.gameObject.SetActive(true);
                    //在滑块范围外随机一个位置...=_=
                    float randomPos = UnityEngine.Random.Range(0, floatingDistance-floatingHandle.sizeDelta-2*floatingTreasure.sizeDelta);//(总长度-宝箱)-(滑块+宝箱)=可随机长度
                    if (randomPos > (floatingHandle.buttom - floatingTreasure.sizeDelta) && randomPos < (floatingHandle.top))//重叠判定
                    {
                        randomPos += floatingHandle.sizeDelta;
                    }
                    floatingTreasure.UpdatePos(randomPos/floatingDistance);
                    isTreasureActived = true;
                }
            }
            if (isTreasureActived)
            {
                int checkResult = checkContainsBlock(floatingTreasure, floatingHandle);
                treasureProgressValue = Mathf.Clamp(treasureProgressValue + treasureProgressSpeed * Time.deltaTime * (checkResult == 1 ? 1 : -1), 0, 1);
                TreasureProgress.sliderValue = treasureProgressValue;
                if (treasureProgressValue >= 1)
                {//成功
                    floatingTreasure.target.gameObject.SetActive(false);
                    isGetTreasure = true;
                }
            }
        }
    }
    #endregion
    #region 进度
    private float progressSpeed = 0;
    private float progressDefaultValue = 0;

    private float progressValue = 0;
    private void setProgressParams(float setProgressDefaultVaule,float setProgressSpeed)
    {
        progressDefaultValue = setProgressDefaultVaule * 0.01f;
        progressValue = progressDefaultValue;
        progressSpeed = setProgressSpeed * 0.01f;
    }
    private void updateCaptureProgress()
    {
        int checkResult = checkContainsBlock(floatingFish,floatingHandle);
        progressValue = Mathf.Clamp(progressValue+progressSpeed*Time.deltaTime*(checkResult==1?1:(FishingFTUEManager.IsGuiding?0:-1)),0,1);
        CaptureProgrss.sliderValue = progressValue;
        if (progressValue <= 0)
        {//失败
            onCaptureResult(false);
        }else if (progressValue >=1)
        {//成功
            onCaptureResult(true);
        }

    }
    private void onCaptureResult(bool result)
    {
        sendCaptureResult(result);
    }
    private void sendCaptureResult(bool result)
    {
        if (CaptureResult != null)
        {
            CaptureResult.Invoke(result,targetFish,isGetTreasure);
        }
    }
    #endregion
    #region 小鱼儿
    private float fishFloatingLimit = 0;
    private float fishFloatingSpeed = 0;

    private float fishFloatingTarget = 0;
    private bool fishFloatingDirection = true;
    private void setFishParams(float setFishFloatingLimit, float setFishFloatingSpeed)
    {
        fishFloatingLimit = setFishFloatingLimit * 0.01f;
        fishFloatingSpeed = setFishFloatingSpeed * 0.01f;

        floatingFish.UpdatePos(0);
    }
    private void UpdateFloatingFish()
    {
        if (fishLogic!=null)
        {
            floatingFish.UpdatePos(fishLogic.getMoveState().NormalizedPos);
            return;
        }

        float result = floatingFish.Move(fishFloatingSpeed * (fishFloatingDirection ? 1 : -1 ));
        if ((fishFloatingDirection && result >= fishFloatingTarget) || (!fishFloatingDirection && result <= fishFloatingTarget))
        {
            fishFloatingTarget = UnityEngine.Random.Range(0, fishFloatingLimit);
            fishFloatingDirection = fishFloatingTarget > result;
        }
    }
    public float getFishFloatingSpeed()
    {
        if (fishLogic != null)
        {
            return fishLogic.getMoveState().NormalizedSpeed;
        }
        return fishFloatingSpeed;
    }

    #endregion
    #region 滑块
    #region 匀速模式
    private float handleFloatingSpeed = 0;
    private void setHandleParams_Normal(float setHandleSize, float setHandleFloatingSpeed)
    {
        setHandleSizeDelta(setHandleSize * 0.01f * floatingDistance);
        handleFloatingSpeed = setHandleFloatingSpeed * 0.01f;

        floatingHandle.UpdatePos(0);
        handleFloatingDirection = false;
    }
    private void updateFloatingHandle_Normal()
    {
        floatingHandle.Move(handleFloatingSpeed * (handleFloatingDirection ? 1 : -1));
    }
    #endregion
    #region 加速模式
    private float accHandleFloatingAccelerateSpUp;
    private float accHandleFloatingAccelerateSpDown;

    private float accHandleFloatingSpeed;
    private void setHandleParams_Acc(float setHandleSize, float setAccHandleFloatingAccelerateSpUp, float setAccHandleFloatingAccelerateSpDown)
    {
        setHandleSizeDelta(setHandleSize * 0.01f * floatingDistance);
        accHandleFloatingAccelerateSpUp = setAccHandleFloatingAccelerateSpUp * 0.01f;
        accHandleFloatingAccelerateSpDown = -Mathf.Abs(setAccHandleFloatingAccelerateSpDown * 0.01f);

        accHandleFloatingSpeed = 0;
        handleFloatingDirection = false;
        floatingHandle.UpdatePos(0);
    }
    public float reboundParams_IgnoreSpeed = -0.2f;
    public float reboundParams_SpeedLoss = 0.6f;
    private void updateFloatingHandle_Acc()
    {
        float accValue = (handleFloatingDirection ? accHandleFloatingAccelerateSpUp : accHandleFloatingAccelerateSpDown) * Time.deltaTime;//速度变化量
        accHandleFloatingSpeed += accValue;//速度变化

        float result = floatingHandle.Move(accHandleFloatingSpeed);
        if (result == 0 && accHandleFloatingSpeed < reboundParams_IgnoreSpeed)
        {//反弹
            accHandleFloatingSpeed = -(accHandleFloatingSpeed * reboundParams_SpeedLoss);
        }
        else if ((result == 1 && accHandleFloatingSpeed > 0) || (result == 0 && accHandleFloatingSpeed < 0 && accHandleFloatingSpeed >= reboundParams_IgnoreSpeed))
        {//停止
            accHandleFloatingSpeed = 0;
        }
    }
    public float getHandleFloatingSpeed()
    {
        return accHandleFloatingSpeed;
    }
    #endregion
    private void setHandleSizeDelta(float size)
    {
        floatingHandle.sizeDelta = size;
        Vector3 targetScale = floatingHandle.target.GetChild(0).localScale;
        targetScale.x = size;
        floatingHandle.target.GetChild(0).localScale = targetScale;
    }

    private void OnInputButtonPress(GameObject go, bool state)
    {
        onInputPress(go, state);
    }
    private bool handleFloatingDirection = false;
    private void onInputPress(GameObject go, bool state)
    {
        handleFloatingDirection = state;
    }
    public bool getHandleFolatingDirection()
    {
        return handleFloatingDirection;
    }
    private int handleMode = 0;
    private void UpdateFloatingHanle()
    {
        if (handleMode == 0)
        {
            updateFloatingHandle_Normal();
        }
        else if (handleMode == 1)
        {
            updateFloatingHandle_Acc();
        }
    }
    #endregion
    #region 基础功能
    private static float floatingDistance = 0;//滑动的最大距离
    public int checkContainsBlock(floatingBlocks item,floatingBlocks container)
    {//判定 如果item有部分在container区域 返回1
        if (item.top < container.buttom || container.top < item.buttom)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    [System.Serializable]
    public class floatingBlocks
    {
        public float sizeDelta;
        public Transform target;

        public void UpdatePos(float value)
        {
            _norPosValue = value;
            target.transform.localPosition = Vector3.up * Mathf.Lerp(0, floatingDistance - sizeDelta, value);
        }
        public float Move(float speed){
            _norPosValue = Mathf.Clamp(_norPosValue + speed * Time.deltaTime, 0, 1);
            UpdatePos(_norPosValue);
            return _norPosValue;
        }
        public float top
        {
            get { return target.localPosition.y + sizeDelta; }
        }
        public float buttom
        {//约定 下方对齐作为原点 _norPosValue=0时 buttom=0
            get { return target.localPosition.y; }
        }
        private float _norPosValue = 0;
        public float normalizedPos
        {//0~1
            get { return _norPosValue; }
        }
    }
    #endregion
}
