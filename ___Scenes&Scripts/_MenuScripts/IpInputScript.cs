using Mirror;
using TMPro;
using UnityEngine;

public class IpInputScript : MonoBehaviour
{
    [SerializeField] private NetworkManager MenuNetworkManager;

    [SerializeField] private TMP_InputField IpUnput;

    private void Start()
    {
        IpUnput.text = MenuNetworkManager.networkAddress;
    }

    private void Update()
    {
        MenuNetworkManager.networkAddress = IpUnput.text;
    }
}
