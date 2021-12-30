using System;

using Ion.Security.RC4;

using Ion.HabboHotel.Habbos;
using Ion.HabboHotel.Client.Utilities;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 4 - "@D"
        /// </summary>
        private void TRY_LOGIN()
        {
            string sUsername = Request.PopFixedString();
            string sPassword = Request.PopFixedString();

            mSession.Login(sUsername, sPassword);
        }
        /// <summary>
        /// 5 - "@E"
        /// </summary>
        private void CHK_VERSION()
        {
            // No longer static! :D
            string sPublicKey = HabboHexRC4.GeneratePublicKeyString(); //"55wfe030o2b17933arq9512j5u111105ckp230c81rp3m61ew9er3y0d523";
            mSession.GetConnection().SetEncryption(sPublicKey);

            Response.Initialize(1); // "@A"
            Response.Append(sPublicKey);
            SendResponse();

            Response.Initialize(50); // "@r"
            SendResponse();
        }
        /// <summary>
        /// 6 - "@F"
        /// </summary>
        private void SET_UID()
        {
            string sMachineID = Request.PopFixedString();

            Response.Initialize(139); // "BK"
            Response.Append("Your machine ID is: ");
            Response.Append(sMachineID);
            Response.Append("<br>Don't worry, this is not logged yet! :-D");
            SendResponse();
        }
        /// <summary>
        /// 43 - "@k"
        /// </summary>
        private void REGISTER()
        {
            // Prepare user object
            Habbo NEWUSER = new Habbo();

            // Gather properties
            UserPropertiesDecoder props = new UserPropertiesDecoder(Request);
            NEWUSER.Username = props[2];
            NEWUSER.Password = props[3];
            NEWUSER.Figure = props[4];
            NEWUSER.Gender = char.Parse(props[5]);
            NEWUSER.Motto = props[6];
            NEWUSER.Email = props[7];
            NEWUSER.DateOfBirth = props[8];
            NEWUSER.signedUp = DateTime.Today;

            // Validate properties

            // Store user
            IonEnvironment.GetDatabase().INSERT(NEWUSER);

            // Login user
            mSession.Login(NEWUSER.Username, NEWUSER.Password);
        }

        public void RegisterPreLogin()
        {
            mRequestHandlers[4] = new RequestHandler(TRY_LOGIN);
            mRequestHandlers[5] = new RequestHandler(CHK_VERSION);
            mRequestHandlers[6] = new RequestHandler(SET_UID);
            mRequestHandlers[43] = new RequestHandler(REGISTER);
        }
        public void UnRegisterPreLogin()
        {
            mRequestHandlers[4] = null;
            mRequestHandlers[5] = null;
            mRequestHandlers[6] = null;
            mRequestHandlers[43] = null;
        }
    }
}
