using System;
using System.IO;
using System.Text;
using System.Configuration;

using Ion.Core;
using Ion.Storage;
using Ion.Configuration;
using Ion.Net.Connections;

namespace Ion
{
    /// <summary>
    /// A static class that holds references to all managers and registered instances.
    /// </summary>
    public static class IonEnvironment
    {
        #region Fields
        private static Logging mLog = new Logging();
        private static ConfigurationModule mConfig;
        private static Encoding mTextEncoding = Encoding.GetEncoding(28591); // Latin1

        private static DatabaseManager mDatabaseManager;
        private static IonTcpConnectionManager mTcpConnectionManager;

        private static HabboHotel.HabboHotel mHabboHotel;
        #endregion

        #region Properties
        public static Logging Log
        {
            get { return mLog; }
        }
        public static ConfigurationModule Configuration
        {
            get { return mConfig; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the Ion server environment.
        /// </summary>
        public static void Initialize()
        {
            Log.minimumLogImportancy = LogType.Debug;
            Log.WriteLine("Initializing Ion environment.");

            try
            {
                // Try to initialize configuration
                try
                {
                    mConfig = ConfigurationModule.LoadFromFile("settings");
                }
                catch (FileNotFoundException ex)
                {
                    Log.WriteError("Failed to load configuration file, exception message was: " + ex.Message);
                    IonEnvironment.Destroy();
                    return;
                }

                // Initialize database and test a connection by getting & releasing it
                DatabaseServer pDatabaseServer = new DatabaseServer(
                    IonEnvironment.Configuration["db1.server.host"],
                    IonEnvironment.Configuration.TryParseUInt32("db1.server.port"),
                    IonEnvironment.Configuration["db1.server.uid"],
                    IonEnvironment.Configuration["db1.server.pwd"]);

                Database pDatabase = new Database(
                    IonEnvironment.Configuration["db1.name"],
                    IonEnvironment.Configuration.TryParseUInt32("db1.minpoolsize"),
                    IonEnvironment.Configuration.TryParseUInt32("db1.maxpoolsize"));

                mDatabaseManager = new DatabaseManager(pDatabaseServer, pDatabase);
                mDatabaseManager.SetClientAmount(2);
                mDatabaseManager.ReleaseClient(mDatabaseManager.GetClient().Handle);
                mDatabaseManager.StartMonitor();

                // Initialize TCP listener
                mTcpConnectionManager = new IonTcpConnectionManager(
                    IonEnvironment.Configuration["net.tcp.localip"],
                    IonEnvironment.Configuration.TryParseInt32("net.tcp.port"),
                    IonEnvironment.Configuration.TryParseInt32("net.tcp.maxcon"));
                mTcpConnectionManager.GetListener().Start();

                
                // Try to initialize Habbo Hotel
                mHabboHotel = new Ion.HabboHotel.HabboHotel();

                Log.WriteLine("Initialized Ion environment.");
            }
            catch (Exception ex) // Catch all other exceptions
            {
                mLog.WriteError("Unhandled exception occurred during initialization of Ion environment. Exception message: " + ex.Message);
            }
        }
        /// <summary>
        /// Destroys the Ion server environment and exits the application.
        /// </summary>
        public static void Destroy()
        {
            Log.WriteLine("Destroying Ion environment.");

            // Destroy Habbo Hotel 8-) (and all inner modules etc)
            if (GetHabboHotel() != null)
            {
                GetHabboHotel().Destroy(); 
            }

            // Clear connections
            if (GetTcpConnections() != null)
            {
                IonEnvironment.Log.WriteLine("Destroying TCP connection manager.");
                GetTcpConnections().GetListener().Stop();
                GetTcpConnections().GetListener().Destroy();
                GetTcpConnections().DestroyManager();
            }

            // Stop database
            if (GetDatabase() != null)
            {
                IonEnvironment.Log.WriteLine("Destroying database " + GetDatabase().ToString());
                GetDatabase().StopMonitor();
                GetDatabase().DestroyClients();
                GetDatabase().DestroyManager();
            }
           
            Log.WriteLine("Press a key to exit.");

            Console.ReadKey();
            System.Environment.Exit(2);
        }

        /// <summary>
        /// Returns the configuration module.
        /// </summary>
        /// <returns>ConfigurationModule holding configuration values for Ion.</returns>
        public static ConfigurationModule GetConfiguration()
        {
            return mConfig;
        }
        /// <summary>
        /// Returns the default System.Text.Encoding for encoding and decoding text.
        /// </summary>
        /// <returns>System.Text.Encoding</returns>
        public static Encoding GetDefaultTextEncoding()
        {
            return mTextEncoding;
        }

        public static DatabaseManager GetDatabase()
        {
            return mDatabaseManager;
        }
        public static IonTcpConnectionManager GetTcpConnections()
        {
            return mTcpConnectionManager;
        }
        public static HabboHotel.HabboHotel GetHabboHotel()
        {
            return mHabboHotel;
        }
        #endregion
    }
}
