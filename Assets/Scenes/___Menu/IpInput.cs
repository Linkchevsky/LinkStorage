using Mirror;
using TMPro;
using UnityEngine;

public class IpInput : MonoBehaviour
{
    [SerializeField] private NetworkManager _menuNetworkManager;

    [SerializeField] private TMP_InputField _ipUnput;

    private void Start()
    {
        _ipUnput.text = _menuNetworkManager.networkAddress;
    }

    private void Update()
    {
        _menuNetworkManager.networkAddress = _ipUnput.text;
    }
}
