using UnityEngine;

public class MeldFactory : MonoBehaviour
{
    public static MeldFactory instance { get; private set; }
    [SerializeField] private GameObject sequenceMeldPrefab;
    [SerializeField] private GameObject tripleMeldPrefab;

    private void Awake()
    {
        if (instance != null && instance == this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }


    public GameObject createTripleMeld(
        Transform parent, string id, bool allowRemove, bool isLaid, string playerName = "")
    {
        GameObject newMeld = Instantiate(tripleMeldPrefab, parent);
        TripleMeld meldManager = newMeld.GetComponent<TripleMeld>();
        meldManager.Init(id, allowRemove, isLaid, playerName);
        return newMeld;
    }

    public GameObject createSequenceMeld(
        Transform parent, string id, bool allowRemove, bool isLaid, string playerName = "")
    {
        GameObject newMeld = Instantiate(sequenceMeldPrefab, parent);
        SequenceMeld meldManager = newMeld.GetComponent<SequenceMeld>();
        meldManager.Init(id, allowRemove, isLaid, playerName);
        return newMeld;
    }
}
