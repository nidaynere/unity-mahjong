using UnityEngine;

public class Pool : MonoBehaviour
{
    public static Pool Instance;
    [SerializeField] private Transform holder;
    [SerializeField] private GameMapMember poolObject;
    [SerializeField] int poolSize;

    public void Awake ()
    {
        Instance = this;

        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);

        size = poolSize;
        poolObjects = new GameMapMember[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            poolObjects[i] = Instantiate(poolObject, holder);
        }
    }

    private GameMapMember[] poolObjects;
    private int currentStep = 0;
    private int size;

    public void Dispose()
    {
        for (int i = 0; i < size; i++)
        {
            Destroy(poolObjects[i].gameObject);
        }

        poolObjects = null;
    }

    public GameMapMember Get()
    {
        var target = poolObjects[currentStep];
        if (++currentStep > size)
            currentStep = 0;

        return target;
    }

    public void Reset()
    {
        for (int i = 0; i < size; i++)
        {
            poolObjects[i].SetHighlight(false);
            poolObjects[i].SetActiveness(false);
            poolObjects[i].gameObject.SetActive(false);
        }

        currentStep = 0;
    }
}
