using System.Text;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkData;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient instance { get; private set; }

    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool m_Done;

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

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        m_Connection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.Parse("3.209.132.25", 12345);
        // endpoint.Port = 9000;
        m_Connection = m_Driver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    private void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!m_Done)
                Debug.Log("Something went wrong during connect");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) !=
               NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");

                var value = 1;
                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    m_Connection.Send(m_Driver, writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);

                byte[] message = stream.ReadBytesAsArray(ref readerCtx, stream.Length);
                string returnData = Encoding.ASCII.GetString(message);

                Debug.Log("Got this: " + returnData);

                m_Done = true;
                m_Connection.Disconnect(m_Driver);
                m_Connection = default(NetworkConnection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                m_Connection = default(NetworkConnection);
            }
        }
    }
    public void SendTransform(Transform t, string id, Vector3 translateInput, Vector3 rotateAxis)
    {

    }
}