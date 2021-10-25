using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceshipShooterGame : NetworkBehaviour
{
    [SerializeField]
    private string m_EndSceneName = "EndScene";

    [SerializeField]
    private NetworkVariable<float> m_GameTimer = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 25f);

    private UIDocument m_GameScreenUIdocument;

    private Label m_GameScreenLabel;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.TryGetComponent(out m_GameScreenUIdocument))
        {
            m_GameScreenLabel = m_GameScreenUIdocument.rootVisualElement.Q<Label>("GameTimerLabel");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;

        var currentTime = m_GameTimer.Value;
        currentTime -= Time.deltaTime;
        m_GameTimer.Value = currentTime;

        UpdateTimerLabelTextClientRPC(Mathf.RoundToInt(currentTime));

        // game is done!
        if (currentTime <= 0f)
            ChangeToEndSceneClientRpc();
    }

    [ClientRpc]
    void ChangeToEndSceneClientRpc()
    {
        //Game over!
        SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_EndSceneName);
    }

    [ClientRpc]
    void UpdateTimerLabelTextClientRPC(int currentTime)
    {
        m_GameScreenLabel.text = $"Game Time: {currentTime} s";
    }
}
