using System;
using System.Collections.Generic;

using Ion.HabboHotel.Client.Utilities;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 7 - "@G"
        /// </summary>
        public void GET_INFO()
        {
            if (mSession.GetHabbo() != null)
            {
                Response.Initialize(5); // "@E"
                Response.Append(mSession.GetHabbo().ToProtocolString());

                SendResponse();
            }
        }
        /// <summary>
        /// 8 - "@H"
        /// </summary>
        public void GET_CREDITS()
        {
            if (mSession.GetHabbo() != null)
            {
                Response.Initialize(6); // "@F"
                Response.Append(mSession.GetHabbo().Coins);
                Response.Append(".0");

                SendResponse();
            }
        }
        /// <summary>
        /// 12 - "@L"
        /// </summary>
        private void MESSENGER_INIT()
        {
            bool enableMessenger = false;
            if (enableMessenger)
            {
                // TODO: initialize messenger, send @L

                RegisterMessenger();
            }
        }
        /// <summary>
        /// 26 - "@Z"
        /// </summary>
        public void SCR_GINFO()
        {
            // 7: [[#habbo_club_handler, #handle_scr_sinfo]], 
            // 23: [[#habbo_club_handler, #handle_scr_sok]],
            // 22: [[#habbo_club_handler, #handle_scr_nosub]]

            string sSubscription = Request.PopFixedString();
            Response.Initialize(7); // "@G"
            Response.AppendString(sSubscription);
            Response.AppendInt32(1337);
            Response.AppendInt32(0);
            Response.AppendInt32(0);
            Response.AppendBoolean(true);
            SendResponse();

            //Response.Initialize(22); // "@V"
            //SendResponse();

            Response.Initialize(23); // "@W"
            SendResponse();
        }
        /// <summary>
        /// 44 - "@l"
        /// </summary>
        private void UPDATE()
        {
            UserPropertiesDecoder props = new UserPropertiesDecoder(Request);
            if (props[4] != null)
                mSession.GetHabbo().Figure = props[4];
            if (props[5] != null)
                mSession.GetHabbo().Gender = char.Parse(props[5]);
            if (props[6] != null)
                mSession.GetHabbo().Motto = props[6];

            GET_INFO(); // Re-send user object
            IonEnvironment.GetDatabase().UPDATE(mSession.GetHabbo());
        }
        /// <summary>
        /// 149 - "BU"
        /// </summary>
        private void UPDATE_ACCOUNT()
        {
            UserPropertiesDecoder props = new UserPropertiesDecoder(Request);

            int errorID = 0;
            if (props[13] != mSession.GetHabbo().Password)
                errorID = 1; // Bad password
            else if (props[8] != mSession.GetHabbo().DateOfBirth)
                errorID = 2; // Bad date of birth

            if (errorID == 0) // Correct password & date of birth
            {
                if (props[3] != null) // Change password
                    mSession.GetHabbo().Password = props[3];
                else if (props[7] != null) // Change email
                    mSession.GetHabbo().Email = props[7];

                // Update user
                IonEnvironment.GetDatabase().UPDATE(mSession.GetHabbo());
            }

            Response.Initialize(169); // "Bi"
            Response.Append(errorID);
            SendResponse();
        }
        /// <summary>
        /// 157 - "B]"
        /// </summary>
        public void GETAVAILABLEBADGES()
        {

        }

        public void RegisterUser()
        {
            mRequestHandlers[7] = new RequestHandler(GET_INFO);
            mRequestHandlers[8] = new RequestHandler(GET_CREDITS);
            mRequestHandlers[12] = new RequestHandler(MESSENGER_INIT);
            mRequestHandlers[26] = new RequestHandler(SCR_GINFO);
            mRequestHandlers[44] = new RequestHandler(UPDATE);
            mRequestHandlers[149] = new RequestHandler(UPDATE_ACCOUNT);
            mRequestHandlers[157] = new RequestHandler(GETAVAILABLEBADGES);
        }
    }
}
