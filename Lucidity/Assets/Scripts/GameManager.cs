using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameObject PlayerRef { get; private set; }

    [SerializeField] private LoopCounter loopCounterUI;
    private int currentLoop = 0;
    private bool newLoop = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
    }

    public void AddLoopToCount()
    {
        currentLoop++;
        loopCounterUI.SetLoopCounterText(currentLoop);
        newLoop = true;
    }

    public void ResetLoops()
    {
        currentLoop = 0;
        loopCounterUI.SetLoopCounterText(currentLoop);
        newLoop= false;
    }

    public bool GetNewLoop()
    {
        return newLoop;
    }

    public void SetNewLoop(bool loopCondition)
    {
        newLoop = loopCondition;
    }
}
