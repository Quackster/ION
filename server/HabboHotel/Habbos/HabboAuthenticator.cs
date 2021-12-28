using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ion.HabboHotel.Habbos
{
    public class HabboAuthenticator
    {
        #region Constructor
        
        #endregion

        #region Methods
        public Habbo Login(string sUsername, string sPassword)
        {
            // Do not use HabboManager.GetHabbo(string) here, as caching is planned to be implemented there
            Habbo pHabbo = new Habbo();
            if (pHabbo.LoadByUsername(IonEnvironment.GetDatabase(), sUsername) == false)
                throw new IncorrectLoginException("login incorrect: Wrong username");

            if (pHabbo.Password != sPassword)
                throw new IncorrectLoginException("login incorrect: Wrong password");

            // Drop old client (if logged in via other connection)
            IonEnvironment.GetHabboHotel().GetClients().DropClientOfHabbo(pHabbo.ID);

            return pHabbo;
        }
        #endregion
    }
}
