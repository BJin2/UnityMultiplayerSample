using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkData;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient instance { get; private set; }

    public string IP;
    public ushort Port;
    public GameObject cube;

    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;

    public string myID { get; private set; }
    private Dictionary<string, GameObject> players;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        //var endpoint = NetworkEndPoint.Parse("3.209.132.25", 12345);
        var endpoint = NetworkEndPoint.Parse(IP, Port);
        m_Connection = m_Driver.Connect(endpoint);

        players = new Dictionary<string, GameObject>();
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    private void OnConnect()
    {
        Debug.Log("Connected to the server.");
    }
    private void OnData(DataStreamReader stream)
    {
        NativeArray<byte> message = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(message);
        string returnData = Encoding.ASCII.GetString(message.ToArray());

        NetworkHeader header = new NetworkHeader();
        try
        {
            header = JsonUtility.FromJson<NetworkHeader>(returnData);
        }
        catch (System.ArgumentException e)
        {
            Debug.LogError(e.ToString() + "\nHeader loading failed. Disconnect");
            Disconnect();
            return;
        }

        try
        {
            switch (header.cmd)
            {
                case Commands.NEW_CLIENT:
                {
                    Debug.Log("New client");
                    NewPlayer np = JsonUtility.FromJson<NewPlayer>(returnData);
                    SpawnPlayers(np.player);
                    break;
                }
                case Commands.UPDATE:
                {
                    UpdatedPlayer up = JsonUtility.FromJson<UpdatedPlayer>(returnData);
                    UpdatePlayers(up.update);
                    break;
                }
                case Commands.CLIENT_DROPPED:
                {
                    DisconnectedPlayer dp = JsonUtility.FromJson<DisconnectedPlayer>(returnData);
                    DestroyPlayers(dp.disconnect);
                    Debug.Log("Client dropped");
                    break;
                }
                case Commands.CLIENT_LIST:
                {
                    ConnectedPlayer cp = JsonUtility.FromJson<ConnectedPlayer>(returnData);
                    SpawnPlayers(cp.connect);
                    Debug.Log("Client list");
                    break;
                }
                case Commands.OWN_ID:
                {
                    Player p = JsonUtility.FromJson<Player>(returnData);
                    myID = p.id;
                    SpawnPlayers(p);
                    Debug.Log("Player's own id");
                    break;
                }
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString() + "\nMessage contents loading failed. Disconnect");
            Disconnect();
            return;
        }
    }
    private void Disconnect()
    {
        Debug.Log("Disconnecting");
        m_Connection.Disconnect(m_Driver);
    }
    private void OnDisconnect()
    {
        Debug.Log("Client got disconnected from server");
        m_Connection = default(NetworkConnection);
    }

    private void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            Debug.Log("Connection Error");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                OnConnect();
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                OnDisconnect();
            }
        }
    }

    private void SpawnPlayers(Player p)
    {
        GameObject temp = Instantiate(cube, p.position, p.rotation);
        temp.GetComponent<NetworkCharacter>().SetNetworkID(p.id);
        temp.GetComponent<NetworkCharacter>().SetControllable(p.id == myID);
        players.Add(p.id, temp);
    }
    private void SpawnPlayers(Player[] p)
    {
        foreach (Player player in p)
        {
            SpawnPlayers(player);
        }
    }
    private void UpdatePlayers(Player[] p)
    {
        foreach (Player player in p)
        {

        }
    }
    private void DestroyPlayers(Player p)
    {
        
    }

    private void SendData(object data)
    {
        var writer = m_Driver.BeginSend(m_Connection);
        NativeArray<byte> sendBytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(JsonUtility.ToJson(data)), Allocator.Temp);
        writer.WriteBytes(sendBytes);
        m_Driver.EndSend(writer);
    }
    public void SendInput(Vector3 input)
    {
        PlayerInput playerInput = new PlayerInput();
        playerInput.input = input;
        SendData(playerInput);
    }
}