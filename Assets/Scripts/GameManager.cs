using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    private void Awake() {
        if (instance != null) {
            Debug.Log("GameManager was already loaded");
        }
        else {
            instance = this;
        }
    }

    public void setSceneCameraState(bool isActivate) {
        if (sceneCamera == null)
            return;

        sceneCamera.SetActive(isActivate);
    }

    #region Player tracking
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    private const string PLAYER_ID_PREFIX = "PlayerNETID ";

    public static void RegisterPlayer(string netID, Player player) {
        string playerID = PLAYER_ID_PREFIX + netID;

        players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static void UnRegisterPlayer(string playerID) {
        players.Remove(playerID);
    }

    public static Player getPlayer(string playerID) {
        return players[playerID];
    }

    //public void OnGUI() {
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));

    //    GUILayout.BeginVertical();
    //    foreach (string p in players.Keys) {
    //        GUILayout.Label(p + " - " + players[p].transform.name);
    //    }
    //    GUILayout.EndVertical();

    //    GUILayout.EndArea();
    //}

    #endregion
}
