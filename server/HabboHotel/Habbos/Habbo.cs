using System;
using System.Text;
using System.Data;

using Ion.Storage;

namespace Ion.HabboHotel.Habbos
{
    /// <summary>
    /// Represents a service user's account and avatar in the account and holds the information about the account.
    /// </summary>
    public class Habbo : IDataObject
    {
        #region Fields
        // Account
        private uint mID;
        private string mUsername;
        private string mPassword;
        private DateTime mSignedUp;

        // Personal
        private string mEmail;
        private string mDateOfBirth;

        // Avatar
        private string mMotto;
        private string mFigure;
        private char mGender;

        // Valueables
        private uint mCoins;
        private uint mFilms;
        private uint mGameTickets;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ID of this Habbo as a 32 bit unsigned integer.
        /// </summary>
        public uint ID
        {
            get { return mID; }
        }
        /// <summary>
        /// Gets or sets the username of this Habbo as a string. The username is shown to other Habbo's in the service, and is also used to login to the service.
        /// </summary>
        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }
        /// <summary>
        /// Gets or sets the password of this Habbo as a string. (plaintext) The password is used in combination with the username when logging in to the service.
        /// </summary>
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
        public DateTime signedUp
        {
            get { return mSignedUp; }
            set { mSignedUp = value; }
        }

        public string Email
        {
            get { return mEmail; }
            set { mEmail = value; }
        }
        public string DateOfBirth
        {
            get { return mDateOfBirth; }
            set { mDateOfBirth = value; }
        }

        public string Motto
        {
            get { return mMotto; }
            set
            {
                // TODO: swear word filtering?
                mMotto = value;
            }
        }
        public string Figure
        {
            get { return mFigure; }
            set { mFigure = value; }
        }
        public char Gender
        {
            get { return mGender; }
            set { mGender = (value == 'M' || value == 'F') ? value : 'M'; }
        }

        public uint Coins
        {
            get { return mCoins; }
            set { mCoins = value; }
        }
        public uint Films
        {
            get { return mFilms; }
            set { mFilms = value; }
        }
        public uint gameTickets
        {
            get { return mGameTickets; }
            set { mGameTickets = value; }
        }
        #endregion

        #region Methods
        public string ToProtocolString()
        {
            return "name=" + mUsername + Convert.ToChar(13) +
                      "figure=" + mFigure + Convert.ToChar(13) +
                      "sex=" + mGender.ToString() + Convert.ToChar(13) +
                      "customData=" + mMotto + Convert.ToChar(13) +
                      "ph_tickets=" + mGameTickets + Convert.ToChar(13) +
                      "photo_film=" + mFilms + Convert.ToChar(13) +
                      "ph_figure=" + "" + Convert.ToChar(13) +
                      "directMail=0" + Convert.ToChar(13);
        }

        #region Storage
        private void AddUserParams(ref DatabaseClient dbClient)
        {
            dbClient.AddParamWithValue("@id", mID);
            dbClient.AddParamWithValue("@username", mUsername);
            dbClient.AddParamWithValue("@password", mPassword);
            dbClient.AddParamWithValue("@signedup", mSignedUp);

            dbClient.AddParamWithValue("@email", mEmail);
            dbClient.AddParamWithValue("@dob", mDateOfBirth);

            dbClient.AddParamWithValue("@motto", mMotto);
            dbClient.AddParamWithValue("@figure", mFigure);
            dbClient.AddParamWithValue("@gender", mGender);

            dbClient.AddParamWithValue("@coins", mCoins);
            dbClient.AddParamWithValue("@films", mFilms);
            dbClient.AddParamWithValue("@gametickets", mGameTickets);
        }
        private bool SetUserParams(ref DataRow dRow)
        {
            if (dRow == null)
                return false;

            mID = (uint)dRow["id"];
            mUsername = (string)dRow["username"];
            mPassword = (string)dRow["password"];
            mSignedUp = (DateTime)dRow["signedup"];

            mEmail = (string)dRow["email"];
            mDateOfBirth = (string)dRow["dob"];

            mMotto = (string)dRow["motto"];
            mFigure = (string)dRow["figure"];
            Gender = char.Parse(dRow["gender"].ToString());

            mCoins = (uint)dRow["coins"];
            mFilms = (uint)dRow["films"];
            mGameTickets = (uint)dRow["gametickets"];

            return true;
        }
        
        public bool LoadByID(DatabaseManager database, uint ID)
        {
            DataRow result = null;
            using (DatabaseClient dbClient = database.GetClient())
            {
                dbClient.AddParamWithValue("@id", ID);
                result = dbClient.ReadDataRow("SELECT * FROM users WHERE id = @id LIMIT 1;");
            }

            return SetUserParams(ref result);
        }
        public bool LoadByUsername(DatabaseManager database, string sUsername)
        {
            DataRow result = null;
            using (DatabaseClient dbClient = database.GetClient())
            {
                dbClient.AddParamWithValue("@username", sUsername);
                result = dbClient.ReadDataRow("SELECT * FROM users WHERE username = @username LIMIT 1;");
            }

            return SetUserParams(ref result);
        }

        public void INSERT(DatabaseClient dbClient)
        {
            AddUserParams(ref dbClient);
            dbClient.ExecuteQuery("INSERT INTO users" +
                "(username,password,signedup,email,dob,motto,figure,gender,coins,films,gametickets) " +
                "VALUES(@username,@password,@signedup,@email,@dob,@motto,@figure,@gender,@coins,@films,@gametickets);");
        }
        public void DELETE(DatabaseClient dbClient)
        {
            dbClient.AddParamWithValue("@id", mID);
            dbClient.ExecuteQuery("DELETE FROM users WHERE id = @id LIMIT 1;");
        }
        public void UPDATE(DatabaseClient dbClient)
        {
            AddUserParams(ref dbClient);
            dbClient.ExecuteQuery("UPDATE users " +
                "SET username=@username,password=@password,signedup=@signedup,email=@email,dob=@dob,motto=@motto,figure=@figure,gender=@gender,coins=@coins,films=@films,gametickets=@gametickets " +
                "WHERE id = @id " +
                "LIMIT 1;");
        }
        #endregion
        #endregion
    }
}
