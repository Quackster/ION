using System;

using Ion.Net.Messages;
using Ion.Net.Connections;

using Ion.HabboHotel.Client;
using Ion.HabboHotel.Habbos;

using Ion.Specialized.Encoding;

namespace Ion.HabboHotel.Client
{
    /// <summary>
    /// Represents a connected client. Holds information about the connection and the logged in user.
    /// </summary>
    public class GameClient
    {
        #region Fields
        private readonly uint mID;
        private ClientMessageHandler mMessageHandler;
        private Habbo mHabbo;
        private bool mPonged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ID of this client as a 32 bit unsigned integer.
        /// </summary>
        public uint ID
        {
            get { return mID; }
        }
        /// <summary>
        /// Gets the logged in status of this client as a boolean value.
        /// </summary>
        public bool isLoggedIn
        {
            get { return (mHabbo != null); }
        }
        public bool pingOK
        {
            get { return mPonged; }
            set { mPonged = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a GameClient instance for a given client ID.
        /// </summary>
        /// <param name="clientID">The ID of this client.</param>
        public GameClient(uint clientID)
        {
            mID = clientID;
            mMessageHandler = new ClientMessageHandler(this);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the IonTcpConnection instance of this client's connection.
        /// </summary>
        public IonTcpConnection GetConnection()
        {
            return IonEnvironment.GetTcpConnections().GetConnection(mID);
        }
        /// <summary>
        /// Returns the ClientMessageHandler instance of this client.
        /// </summary>
        public ClientMessageHandler GetMessageHandler()
        {
            return mMessageHandler;
        }
        /// <summary>
        /// Returns the Habbo instance holding the information of the Habbo this client is logged in to.
        /// </summary>
        public Habbo GetHabbo()
        {
            return mHabbo;
        }

        /// <summary>
        /// Starts the connection for this client, prepares the message handler and sends HELLO to the client.
        /// </summary>
        public void StartConnection()
        {
            IonTcpConnection pConnection = IonEnvironment.GetTcpConnections().GetConnection(mID);
            if (pConnection != null)
            {
                // Register request handlers
                mMessageHandler.RegisterGlobal();
                mMessageHandler.RegisterPreLogin();

                // Send HELLO to client
                ServerMessage HELLO = new ServerMessage(0); // "@@"
                pConnection.SendMessage(HELLO);

                // Create data router and start waiting for data
                IonTcpConnection.RouteReceivedDataCallback dataRouter = new IonTcpConnection.RouteReceivedDataCallback(HandleConnectionData);
                pConnection.Start(dataRouter);
            }
        }
        /// <summary>
        /// Stops the client, removes this user from room, updates last online values etc.
        /// </summary>
        public void Stop()
        {
            // Leave room
            // Update last online
            mHabbo = null;

            mMessageHandler.Destroy();
            mMessageHandler = null;
        }

        /// <summary>
        /// Handles a given amount of data in a given byte array, by attempting to parse messages from the received data and process them in the message handler.
        /// </summary>
        /// <param name="Data">The byte array with the data to process.</param>
        /// <param name="numBytesToProcess">The actual amount of bytes in the byte array to process.</param>
        public void HandleConnectionData(ref byte[] Data)
        {
            int pos = 0;
            while (pos < Data.Length)
            {
                try
                {
                    // Total length of message (without this): 3 Base64 bytes
                    int messageLength = Base64Encoding.DecodeInt32(new byte[] { Data[pos++], Data[pos++], Data[pos++] });

                    // ID of message: 2 Base64 bytes
                    uint messageID = Base64Encoding.DecodeUInt32(new byte[] { Data[pos++], Data[pos++] });

                    // Data of message: (messageLength - 2) bytes
                    byte[] Content = new byte[messageLength - 2];
                    for (int i = 0; i < Content.Length; i++)
                    {
                        Content[i] = Data[pos++];
                    }

                    // Create message object
                    ClientMessage pMessage = new ClientMessage(messageID, Content);

                    // Handle message object
                    mMessageHandler.HandleRequest(pMessage);
                }
                catch (IndexOutOfRangeException) // Bad formatting!
                {
                    IonEnvironment.GetHabboHotel().GetClients().StopClient(mID);
                }
                catch (Exception ex)
                {
                    IonEnvironment.Log.WriteUnhandledExceptionError("GameClient.HandleConnectionData", ex);
                }
            }
        }

        /// <summary>
        /// Tries to login this client on a Habbo account with a given username and password.
        /// </summary>
        /// <param name="sUsername">The username of the Habbo to attempt to login on.</param>
        /// <param name="sPassword">The login password of the Habbo username. Case sensitive.</param>
        public void Login(string sUsername, string sPassword)
        {
            try
            {
                // Try to login
                mHabbo = IonEnvironment.GetHabboHotel().GetAuthenticator().Login(sUsername, sPassword);
                
                // Authenticator has forced unique login now

                // Complete login
                ServerMessage pMessage = new ServerMessage(2); // "@B"
                pMessage.AppendString("fuse_login");
                GetConnection().SendMessage(pMessage);

                pMessage.Initialize(3); // "@C"
                GetConnection().SendMessage(pMessage);

                mMessageHandler.UnRegisterPreLogin();
                mMessageHandler.RegisterUser();
            }
            catch (IncorrectLoginException exLogin)
            {
                SendClientError(exLogin.Message);
            }
            catch (ModerationBanException exBan)
            {
                SendBanMessage(exBan.Message);
            }
        }
        
        /// <summary>
        /// Reports a given error string to the client. (message 33: @a)
        /// </summary>
        /// <param name="sError">The error to report.</param>
        public void SendClientError(string sError)
        {
            ServerMessage pMessage = new ServerMessage(33); // "@a"
            pMessage.Append(sError);

            GetConnection().SendMessage(pMessage);
        }
        /// <summary>
        /// Sends the 'You are banned' message to the client. The message holds a given ban reason.
        /// </summary>
        /// <param name="sText">The text to display in the ban message.</param>
        public void SendBanMessage(string sText)
        {
            ServerMessage pMessage = new ServerMessage(35); // "@c"
            pMessage.Append(sText);

            GetConnection().SendMessage(pMessage);
        }
        #endregion
    }
}
