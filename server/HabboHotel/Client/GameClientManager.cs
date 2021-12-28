using System;
using System.Threading;
using System.Collections.Generic;

using Ion.Net.Connections;
using Ion.Net.Messages;

using Ion.HabboHotel.Habbos;

namespace Ion.HabboHotel.Client
{
    /// <summary>
    /// Manages connected clients, checks their pings and manages the Habbo they are logged in on.
    /// </summary>
    public class GameClientManager
    {
        #region Fields
        private Thread mConnectionChecker;
        private Dictionary<uint, GameClient> mClients;
        #endregion

        #region Constructor
        public GameClientManager()
        {
            mClients = new Dictionary<uint, GameClient>();
        }
        #endregion

        #region Methods
        public void Clear()
        {
            mClients.Clear();
        }

        public void StartConnectionChecker()
        {
            if (mConnectionChecker == null)
            {
                mConnectionChecker = new Thread(TestClientConnections);
                mConnectionChecker.Priority = ThreadPriority.BelowNormal;

                mConnectionChecker.Start();
            }
        }
        public void StopConnectionChecker()
        {
            if (mConnectionChecker != null)
            {
                try { mConnectionChecker.Abort(); }
                catch { }

                mConnectionChecker = null;
            }
        }
        private void TestClientConnections()
        {
            while (true)
            {
                try
                {
                    /*
                    // Prepare "@r" message data
                    byte[] PINGDATA = new ServerMessage(50).GetBytes(); // "@r"
                    
                    // Gather timed out clients and reset ping status for in-time clients
                    List<uint> pTimedOutClients = new List<uint>();
                    lock (mClients)
                    {
                        foreach (GameClient pClient in mClients.Values)
                        {
                            if (pClient.pingOK)
                            {
                                pClient.pingOK = false;
                                pClient.GetConnection().SendData(PINGDATA);
                            }
                            else
                            {
                                pTimedOutClients.Add(pClient.ID);
                            }
                        }

                        // Stop the gathered timed out clients
                        foreach (uint timedOutClientID in pTimedOutClients)
                        {
                            StopClient(timedOutClientID);
                        }
                    }
                    */

                    lock (mClients) // Currently busy!
                    {
                        // Gather clients with dead connection
                        List<uint> pDeadClients = new List<uint>();
                        foreach (uint clientID in mClients.Keys)
                        {
                            if (IonEnvironment.GetTcpConnections().TestConnection(clientID) == false)
                                pDeadClients.Add(clientID);
                        }

                        // Stop the gathered dead clients
                        foreach (uint clientID in pDeadClients)
                        {
                            StopClient(clientID);
                        }
                    }


                    // Sleep for 30 seconds and repeat!
                    Thread.Sleep(30000);
                }
                catch (ThreadAbortException) { } // Nothing special!
            }
        }

        public GameClient GetClient(uint clientID)
        {
            if (mClients.ContainsKey(clientID))
                return mClients[clientID];

            return null;
        }
        public bool RemoveClient(uint clientID)
        {
            return mClients.Remove(clientID);
        }

        public void StartClient(uint clientID)
        {
            GameClient pClient = new GameClient(clientID);
            mClients.Add(clientID, pClient);

            pClient.StartConnection();
        }
        public void StopClient(uint clientID)
        {
            GameClient pClient = GetClient(clientID);
            if (pClient != null)
            {
                // Stop & drop connection
                IonEnvironment.GetTcpConnections().DropConnection(clientID);

                // Stop client
                pClient.Stop();

                // Drop client
                mClients.Remove(clientID);

                // Log event
                IonEnvironment.Log.WriteInformation("Stopped client " + clientID);
            }
        }

        public GameClient GetClientOfHabbo(uint accountID)
        {
            lock (mClients)
            {
                foreach (GameClient pClient in mClients.Values)
                {
                    if (pClient.GetHabbo() != null && pClient.GetHabbo().ID == accountID)
                        return pClient;
                }
            }

            return null;
        }
        public uint GetClientIdOfHabbo(uint accountID)
        {
            GameClient pClient = GetClientOfHabbo(accountID);
            
            return (pClient != null) ? pClient.ID : 0;
        }

        public void DropClientOfHabbo(uint accountID)
        {
            uint clientID = GetClientIdOfHabbo(accountID);
            if (clientID > 0)
                StopClient(clientID);
        }
        #endregion
    }
}
