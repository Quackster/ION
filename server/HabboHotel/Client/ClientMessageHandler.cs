using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ion.Net.Messages;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        #region Fields
        private const int HIGHEST_MESSAGEID = 200; // "B]" : GETAVAILABLEBADGES
        private GameClient mSession;

        private ClientMessage Request;
        private ServerMessage Response;

        private delegate void RequestHandler();
        private RequestHandler[] mRequestHandlers;
        #endregion

        #region Constructor
        public ClientMessageHandler(GameClient pSession)
        {
            mSession = pSession;
            mRequestHandlers = new RequestHandler[HIGHEST_MESSAGEID + 1];
            
            Response = new ServerMessage(0);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Destroys all the resources in the ClientMessageHandler.
        /// </summary>
        public void Destroy()
        {
            mSession = null;
            mRequestHandlers = null;

            Request = null;
            Response = null;
        }
        /// <summary>
        /// Invokes the matching request handler for a given ClientMessage.
        /// </summary>
        /// <param name="pRequest">The ClientMessage object to process.</param>
        public void HandleRequest(ClientMessage pRequest)
        {
            IonEnvironment.Log.WriteLine("[" + mSession.ID + "] --> " + pRequest.Header + pRequest.GetContentString());

            if (pRequest.ID > HIGHEST_MESSAGEID)
                return; // Not in protocol
            if (mRequestHandlers[pRequest.ID] == null)
                return; // Handler not registered

            // Handle request
            Request = pRequest;
            mRequestHandlers[pRequest.ID].Invoke();
            Request = null;
        }
        /// <summary>
        /// Sends the current response ServerMessage on the stack.
        /// </summary>
        public void SendResponse()
        {
            if(Response.ID > 0)
                mSession.GetConnection().SendMessage(Response);
        }
        #endregion
    }
}
