using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuControl : MonoBehaviour
{
    [SerializeField]
    private string m_NextSceneToSwitchTo = "SampleScene";

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
        //After a domain reload, we need to re-cache our VisualElements and hook our callbacks
        InitializeVisualTree(GetComponent<UIDocument>());
    }

    private void InitializeVisualTree(UIDocument uiDocument)
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component missing!");
            return;
        }

        var root = uiDocument.rootVisualElement;

        var hostButton = root.Q<Button>("HostButton");
        hostButton.clicked += HostButton_clicked;

        var clientButton = root.Q<Button>("ClientButton");
        clientButton.clicked += ClientButton_clicked;

        var serverButton = root.Q<Button>("ServerButton");
        serverButton.clicked += ServerButton_clicked;
    }

    private void HostButton_clicked()
    {
        NetworkManager.Singleton.StartHost();
        SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_NextSceneToSwitchTo);
    }

    private void ClientButton_clicked()
    {
        NetworkManager.Singleton.StartClient();
        SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_NextSceneToSwitchTo);
    }

    private void ServerButton_clicked()
    {
        NetworkManager.Singleton.StartServer();
        SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_NextSceneToSwitchTo);
    }
}
