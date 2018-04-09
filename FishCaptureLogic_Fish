using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**************************************************
* 
* 创建者：		yangjifei
* 创建时间：	2017/07/28 8:46
* 描述：		策划要的鱼的复杂的运动模式
* 
**************************************************/
public class FishCaptureLogic_Fish : MonoBehaviour
{
    [SerializeField]
    public FishMovingParams setMoveParams;
    [ContextMenu("初始设置")]
    public void Test()
    {//测试用
        setFishMovingParams(setMoveParams);
    }


    FishMovingState moveState = new FishMovingState();
    FishMovingParams moveParams;
    MoveActionSequence curSeq;
    MoveActionSequence correctSeq;
    public void setFishMovingParams(FishMovingParams setMoveParams)
    {
        moveParams = setMoveParams;
        if (FishingFTUEManager.IsGuiding)
        {//引导特殊逻辑
            moveState.position = 95f;
            curSeq = new MoveActionSequence(null, false, new FishMovingActionBase[][] { new FishMovingActionBase[] { moveParams.actionB } });
            return;
        }
        moveState.position = moveParams.InitPos.getRandom();
        //常用循环序列    //CDE->ABC->loop
        MoveActionSequence loopSeq = new MoveActionSequence(null,true,
            new FishMovingActionBase[][] {
                new FishMovingActionBase[] { moveParams.actionC, moveParams.actionD, moveParams.actionE },
                new FishMovingActionBase[] { moveParams.actionA, moveParams.actionB, moveParams.actionC } });
        //初始使用序列 //A|B|AB->loop
        FishMovingActionBase[] initAction;
        if (moveState.position == 0)
        {
            initAction = new FishMovingActionBase[] { moveParams.actionA };
        } else if (moveState.position == 100)
        {
            initAction = new FishMovingActionBase[] { moveParams.actionB };
        }
        else
        {
            initAction = new FishMovingActionBase[] { moveParams.actionA,moveParams.actionB };
        }
        MoveActionSequence initSeq = new MoveActionSequence(loopSeq,false,new FishMovingActionBase[][] {initAction});
        //两端长时间停留复位序列   //F-DE->(AB->loop)
        MoveActionSequence afterCorrectSeq = new MoveActionSequence(loopSeq, false, new FishMovingActionBase[][] { 
            new FishMovingActionBase[] { moveParams.actionA, moveParams.actionB } });
        correctSeq = new MoveActionSequence(afterCorrectSeq, true, new FishMovingActionBase[][] {
            new FishMovingActionBase[] {moveParams.actionF},
            new FishMovingActionBase[] {moveParams.actionE,moveParams.actionD}
        });
        curSeq = initSeq;
    }
    private bool guidFishAction = false;
    public void Update()
    {
        if (moveParams==null)
        {
            return;
        }if (curSeq!=null)
        {
            if (FishingFTUEManager.IsGuiding)
            {//引导特殊逻辑 进度大于0.6开始下移动作
                if (!guidFishAction&&FishingDialog.instance.fishCaptureLogic.CaptureProgrss.sliderValue > .6f)
                {
                    guidFishAction = true;
                    FishingFTUEManager.FTUE_ActionStage++;//切下个状态
                }
                if (guidFishAction)
                {
                    curSeq.UpdateSeq(moveState);
                }
            }
            else
            {//通用逻辑
                //主流程序列(包含复位)
                var newSeq1= curSeq.UpdateSeq(moveState);
                if (newSeq1!=null)
                {
                    curSeq = newSeq1;
                }
                //复位流程序列 单独跑
                if (curSeq != correctSeq)
                {
                    var newSeq = correctSeq.UpdateSeq(moveState);
                    if (newSeq != null)
                    {
                        curSeq = newSeq;
                    }
                }
            }
        }
        moveState.update();
    }
#if UNITY_EDITOR
    public void OnGUI()
    {
        if (curSeq != null && curSeq.runingAction!=null)
        {
            GUILayout.Label(curSeq.runingAction.ToString());
        }
    }
#endif
    public FishMovingState getMoveState()
    {
        return moveState;
    }

    public abstract class FishMovingActionBase
    {
        protected bool isInit = false;
        public bool getIsInit() { return isInit; }
        protected abstract void InitAction(FishMovingState state);
        public abstract bool isActionDone(FishMovingState state);
        public void Init(FishMovingState state)
        {
            resetStateTime();
            isInit = true;
            InitAction(state);
        }
        public void Reset(FishMovingState state)
        {
            isInit = false;
        }
        public virtual bool isTrigger(FishMovingState state)
        {
            return true;
        }

        protected float stateTime;
        private void resetStateTime()
        {
            stateTime = 0;
        }
        public void updateStateTime()
        {
            stateTime += Time.deltaTime;
        }
    }
    [System.Serializable]
    public class FishMovingActionA : FishMovingActionBase
    {//加速度>0
        public FloatRange accValue;
        public FloatRange duration;

        private float t;
        protected override void InitAction(FishMovingState state)
        {
            state.accelerate = accValue.getRandom();
            t = duration.getRandom();
        }
        public override bool isActionDone(FishMovingState state)
        {
            updateStateTime();
            return stateTime >= t;
        }
    }
    [System.Serializable]
    public class FishMovingActionB : FishMovingActionBase
    {//加速度<0
        public FloatRange accValue;
        public FloatRange duration;

        private float t;
        protected override void InitAction(FishMovingState state)
        {
            state.accelerate = -accValue.getRandom();
            t = duration.getRandom();
        }
        public override bool isActionDone(FishMovingState state)
        {
            updateStateTime();
            return stateTime >= t;
        }
    }
    [System.Serializable]
    public class FishMovingActionC : FishMovingActionBase
    {//加速度==0
        public bool isEnabled;
        public FloatRange duration;

        private float t;
        protected override void InitAction(FishMovingState state)
        {
            if (isEnabled)
            {
                state.accelerate = 0;
                t = duration.getRandom();
            }
        }
        public override bool isActionDone(FishMovingState state)
        {
            if (!isEnabled)
            {
                return true;
            }
            updateStateTime();
            return stateTime >= t;
        }
    }
    [System.Serializable]
    public class FishMovingActionD : FishMovingActionBase
    {//速度==0
        public bool isEnabled;
        public FloatRange duration;

        private float t;
        protected override void InitAction(FishMovingState state)
        {
            if (isEnabled)
            {
                state.accelerate = 0;
                state.speed = 0;
                t = duration.getRandom();
            }
        }
        public override bool isActionDone(FishMovingState state)
        {
            if (!isEnabled)
            {
                return true;
            }
            updateStateTime();
            return stateTime >= t;
        }
    }
    [System.Serializable]
    public class FishMovingActionE : FishMovingActionBase
    {//速度降到0
        public bool isEnabled;
        public FloatRange accValue;

        float preSpeed;
        protected override void InitAction(FishMovingState state)
        {
            if (isEnabled && state.speed!=0)
            {
                state.accelerate = state.speed > 0 ? -accValue.getRandom() : accValue.getRandom();
                preSpeed = state.speed;
            }
        }
        public override bool isActionDone(FishMovingState state)
        {
            if (!isEnabled)
            {
                return true;
            }
            if (preSpeed*state.speed<=0)
            {
                return true;
            }
            return false;
        }
    }
    [System.Serializable]
    public class FishMovingActionF : FishMovingActionBase
    {//触顶复位
        public FloatRange targetPos;
        public float resetDelay;
        public float speed;

        private float toPos;
        protected override void InitAction(FishMovingState state)
        {
            LtLog.Info("FishMovingActionReset");
            delay = 0;
            state.accelerate = 0;
            toPos = targetPos.getRandom();
            state.speed = state.position > 50 ? -speed : speed;
        }
        public override bool isActionDone(FishMovingState state)
        {
            if (isInit)
            {
                return (state.position - toPos) * state.speed > 0;
            }
            return false;
        }
        private float delay=0;
        public override bool isTrigger(FishMovingState state)
        {
            if (state.position <= 0 || state.position >= 100)
            {
                delay += Time.deltaTime;
            }
            else
            {
                delay = 0;
            }
            return delay >= resetDelay;
        }
    }
    [System.Serializable]
    public class FishMovingState{
        public float position;//0-100
        public float speed;
        public float accelerate;

        public float NormalizedPos
        {
            get { return position * 0.01f; }
        }
        public float NormalizedSpeed
        {
            get { return speed * 0.01f; }
        }
        public void update()
        {
            speed += accelerate*Time.deltaTime;
            if ((speed > 0 && position >= 100) || (speed < 0 && position <= 0))
            {
                speed = 0;
            }
            position = Mathf.Clamp(position+=speed*Time.deltaTime,0,100);
        }
    }
    [System.Serializable]
    public class FishMovingParams
    {
        public FloatRange InitPos;
        public FishMovingActionA actionA = new FishMovingActionA();
        public FishMovingActionB actionB = new FishMovingActionB();
        public FishMovingActionC actionC = new FishMovingActionC();
        public FishMovingActionD actionD = new FishMovingActionD();
        public FishMovingActionE actionE = new FishMovingActionE();
        public FishMovingActionF actionF = new FishMovingActionF();
    }
    public class MoveActionSequence
    {
        private MoveActionSequence endSeq;
        private List<FishMovingActionBase[]> actions = new List<FishMovingActionBase[]>();
        private bool isloop = false;
        public MoveActionSequence(MoveActionSequence endSequence,bool loop,params FishMovingActionBase[][] actionPs)
        {
            isloop = loop;
            endSeq = endSequence;
            for (int i = 0; i < actionPs.Length; i++)
            {
                actions.Add(actionPs[i]);
            }
        }
        private int seqActionIndex = -1;
        public FishMovingActionBase runingAction;//TOOD 测试设置public
        public MoveActionSequence UpdateSeq(FishMovingState state)
        {//切换sequence的话返回对应sequence
            MoveActionSequence retSeq = null;
            if (runingAction==null)
            {
                nextAction(state,ref retSeq);
            }
            else
            {
                if (!runingAction.getIsInit())
	            {
                    if (runingAction.isTrigger(state))
                    {
                        runingAction.Init(state);
                        return this;
                    }
                }
                if (runingAction.getIsInit() && runingAction.isActionDone(state))
                {
                    runingAction.Reset(state);
                    nextAction(state,ref retSeq);
                }
            }

            return retSeq;
            //if (runingAction == null || runingAction.isActionDone(state))
            //{
            //    if (runingAction!=null)
            //    {
            //        runingAction.Reset(state);
            //    }
            //    seqActionIndex++;
            //    if (seqActionIndex >= actions.Count)
            //    {
            //        seqActionIndex = 0;
            //        if (endSeq!=null)
            //        {
            //            return endSeq;
            //        }
            //    }
            //    nextAction(state);
            //    if (runingAction.isTrigger(state))
            //    {
            //        runingAction.Init(state);
            //        return this;
            //    }
            //}
            //return null;
        }

        private void nextAction(FishMovingState state, ref MoveActionSequence changeSeq)
        {
            seqActionIndex++;
            if (seqActionIndex >= actions.Count)
            {
                seqActionIndex = 0;
                if (endSeq!=null)
                {
                    changeSeq = endSeq;
                }
                if (!isloop)
                {
                    return;
                }
            }
            FishMovingActionBase[] acs = actions[seqActionIndex];
            runingAction = acs[UnityEngine.Random.Range(0, acs.Length)];
        }
    }
}
