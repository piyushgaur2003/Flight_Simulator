using UnityEditor;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { set; get; }
    public SaveState state;

    void Awake()
    {
        ResetSave();
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();
    }

    public void Save(){
         PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("save")){
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else{
            state = new SaveState();
            Save();
            Debug.Log("No saved files found. Creating a new one! ");
        }
    }

    public bool IsPlaneOwned(int index){
        return(state.planeOwned & (1 << index)) != 0;
    }

    public bool BuyPlane(int index, int cost){
        if (state.gold >= cost){
            state.gold -= cost;
            UnlockPlane(index);

            Save();

            return true;
        } else{
            return false;
        }
    }

    public void UnlockPlane(int index){
        state.planeOwned |= 1 << index;
    }

    public void ResetSave(){
        PlayerPrefs.DeleteKey("save");
    }
}
