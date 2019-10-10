using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFOGAME;


public enum SSActionEventType:int { Started, Competeted }
 
public interface ISSActionCallback {
    void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null);
	
}
 
public interface IUserAction {
    void BeginGame ();
    void GameOver ();
    void Restart ();
    int GetScore ();
    int GetRound ();
    int GetBlood ();
    void hit (Vector3 pos);
}

public class SSAction : ScriptableObject {
 
    public bool enable = false;
    public bool destroy = false;
 
    public GameObject gameobject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callback { get; set; }
 
    protected SSAction() { }
 
    public virtual void Start () {
        throw new System.NotImplementedException();
	}
	
	public virtual void Update () {
        throw new System.NotImplementedException();
    }
 
    public void reset()
    {
        enable = false;
        destroy = false;
        gameobject = null;
        transform = null;
        callback = null;
    }
}

public class SSActionManager : MonoBehaviour {
 
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> waitingDelete = new List<int>();
 
    
    // Use this for initialization
    protected void Start()
    {
 
    }
 
    // Update is called once per frame
    protected void Update()
    {
        //把等待队列里所有的动作注册到动作管理器里
        foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
        waitingAdd.Clear();
 
        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
            }
        }
 
        //把删除队列里所有的动作删除
        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }
 
    //初始化一个动作
    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }
}

public class SSFlyAction : SSAction {
    float acceleration; 
    public float horizontalSpeed = 5F;
    Vector3 direction;
    float time;
 
	public override void Start () {
        enable = true;
    }
 
    // Update is called once per frame
    public override void Update () {
        if (gameobject.activeSelf) {
            transform.Translate(new Vector3(gameobject.GetComponent<DiskData>().direction.x*Time.deltaTime * horizontalSpeed, -Time.deltaTime*2F, 0));
            if (this.transform.position.y < -6) {
                if(this.transform.position.y > -9){
                    BloodRecorder bloodRecorder;
                    bloodRecorder = Singleton<BloodRecorder>.Instance;
                    bloodRecorder.Record(this.gameobject);
                }
                this.destroy = true;
                this.enable = false;
                this.callback.SSActionEvent(this);
            }
        }
	}
 
    public static SSFlyAction GetSSAction()
    {
        SSFlyAction action = ScriptableObject.CreateInstance<SSFlyAction>();
        return action;
    }
}

public class MyActionManager : SSActionManager, ISSActionCallback {
    public Controller sceneController;
    public List<SSFlyAction> flyActions = new List<SSFlyAction> ();

    private List<SSFlyAction> used = new List<SSFlyAction> ();
    private List<SSFlyAction> free = new List<SSFlyAction> ();


    protected new void Start () {
        sceneController = SSDirector.getInstance().CurrentSceneController as Controller;
        sceneController.actionManager = this;
        flyActions.Add(SSFlyAction.GetSSAction());
    }

    SSFlyAction GetSSFlyAction () {
        SSFlyAction action = null;
        if (free.Count > 0) {
            action = free[0];
            free.Remove(free[0]);
        }
        else {
            action = ScriptableObject.Instantiate<SSFlyAction>(flyActions[0]);
        }
        used.Add(action);
        return action;
    }

    public void FreeSSAction (SSAction action) {
        SSFlyAction tmp = null;
        foreach (SSFlyAction i in used) {
            if (action.GetInstanceID () == i.GetInstanceID ()) {
                tmp = i;
                break;
            }
        }
        if (tmp != null) {
            tmp.reset ();
            free.Add (tmp);
            used.Remove(tmp);
        }
    }

    public void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null) {
        if (source is SSFlyAction) {
            DiskFactory df = Singleton<DiskFactory>.Instance;
            df.FreeDisk (source.gameobject);
            FreeSSAction(source);
        }
    }

    public void throwDisk (GameObject disk) {
        SSFlyAction fly = GetSSFlyAction();
        if (sceneController.GetRound() == 1){
            fly.horizontalSpeed = Random.Range(5F, 7F);
        }
        else if (sceneController.GetRound() == 2){
            fly.horizontalSpeed = Random.Range(7F, 9F);
        }
        else {
            fly.horizontalSpeed = Random.Range(9F, 11F);
        }
        RunAction (disk, fly, this);
    }
}