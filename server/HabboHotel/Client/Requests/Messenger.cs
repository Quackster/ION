using System;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 15 - "@O"
        /// </summary>
        private void MESSENGER_SENDUPDATE()
        {

        }
        /// <summary>
        /// 30 - "@^"
        /// </summary>
        private void MESSENGER_C_CLICK()
        {

        }
        /// <summary>
        /// 31 - "@_"
        /// </summary>
        private void MESSENGER_C_READ()
        {

        }
        /// <summary>
        /// 32 - "@`"
        /// </summary>
        private void MESSENGER_MARKREAD()
        {

        }
        /// <summary>
        /// 33 - "@a"
        /// </summary>
        private void MESSENGER_SENDMSG()
        {

        }
        /// <summary>
        /// 34 - "@b"
        /// </summary>
        private void MESSENGER_SENDEMAILMSG()
        {

        }
        /// <summary>
        /// 35 - "@c"
        /// </summary>
        private void MESSENGER_ASSIGNPERSMSG()
        {

        }
        /// <summary>
        /// 36 - "@d"
        /// </summary>
        private void MESSENGER_ACCEPTBUDDY()
        {

        }
        /// <summary>
        /// 37 - "@e"
        /// </summary>
        private void MESSENGER_DECLINEBUDDY()
        {

        }
        /// <summary>
        /// 38 - "@f"
        /// </summary>
        private void MESSENGER_REQUESTBUDDY()
        {

        }
        /// <summary>
        /// 39 - "@g"
        /// </summary>
        private void MESSENGER_REMOVEBUDDY()
        {

        }

        /// <summary>
        /// Registers the request handlers for the in-game messenger. ('Console')
        /// </summary>
        public void RegisterMessenger()
        {
            mRequestHandlers[15] = new RequestHandler(MESSENGER_SENDUPDATE);
            mRequestHandlers[30] = new RequestHandler(MESSENGER_C_CLICK);
            mRequestHandlers[31] = new RequestHandler(MESSENGER_C_READ);
            mRequestHandlers[32] = new RequestHandler(MESSENGER_MARKREAD);
            mRequestHandlers[33] = new RequestHandler(MESSENGER_SENDMSG);
            mRequestHandlers[34] = new RequestHandler(MESSENGER_SENDEMAILMSG);
            mRequestHandlers[35] = new RequestHandler(MESSENGER_ASSIGNPERSMSG);
            mRequestHandlers[36] = new RequestHandler(MESSENGER_ACCEPTBUDDY);
            mRequestHandlers[37] = new RequestHandler(MESSENGER_DECLINEBUDDY);
            mRequestHandlers[38] = new RequestHandler(MESSENGER_REQUESTBUDDY);
            mRequestHandlers[39] = new RequestHandler(MESSENGER_REMOVEBUDDY);
        }
    }
}
