using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceshipShooterGame : NetworkBehaviour
{
    [SerializeField]
    private string EndScene = "EndScene";

    [SerializeField]
    private NetworkVariable<float> gameTimer = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 25f);

    private UIDocument gameScreenUIdocument;

    private Label gameScreenLabel;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.TryGetComponent<UIDocument>(out gameScreenUIdocument))
        {
            gameScreenLabel = gameScreenUIdocument.rootVisualElement.Q<Label>("GameTimerLabel");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;

        var currentTime = gameTimer.Value;
        currentTime -= Time.deltaTime;
        gameTimer.Value = currentTime;

        UpdateTimerLabelTextClientRPC(Mathf.RoundToInt(currentTime));

        // game is done!
        if (currentTime <= 0f)
            ChangeToEndSceneClientRpc();
    }

    [ClientRpc]
    void ChangeToEndSceneClientRpc()
    {
        //Game over!
        SceneTransitionHandler.sceneTransitionHandler.SwitchScene(EndScene);
    }

    [ClientRpc]
    void UpdateTimerLabelTextClientRPC(int currentTime)
    {
        gameScreenLabel.text = $"Game Time: {currentTime} s";
    }
}
