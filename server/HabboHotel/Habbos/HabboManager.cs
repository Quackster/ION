using System;

namespace Ion.HabboHotel.Habbos
{
    /// <summary>
    /// Manages service users ('Habbo's') and provides methods for updating and retrieving accounts.
    /// </summary>
    public class HabboManager
    {
        #region Constructors

        #endregion

        #region Methods
        public Habbo GetHabbo(uint ID)
        {
            Habbo pHabbo = new Habbo();
            if (pHabbo.LoadByID(IonEnvironment.GetDatabase(), ID))
                return pHabbo;

            return null;
        }
        public Habbo GetHabbo(string sUsername)
        {
            Habbo pHabbo = new Habbo();
            if (pHabbo.LoadByUsername(IonEnvironment.GetDatabase(), sUsername))
                return pHabbo;

            return null;
        }
        #endregion
    }
}
