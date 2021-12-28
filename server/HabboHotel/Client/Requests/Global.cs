using System;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 9 - "@I"
        /// </summary>
        public void GETAVAILABLESETS()
        {
            Response.Initialize(8); // "@H"
            Response.Append("[100,105,110,115,120,125,130,135,140,145,150,155,160,165,170,175,176,177,178,180,185,190,195,200,205,206,207,210,215,220,225,230,235,240,245,250,255,260,265,266,267,270,275,280,281,285,290,295,300,305,500,505,510,515,520,525,530,535,540,545,550,555,565,570,575,580,585,590,595,596,600,605,610,615,620,625,626,627,630,635,640,645,650,655,660,665,667,669,670,675,680,685,690,695,696,700,705,710,715,720,725,730,735,740]");
            
            SendResponse();
        }
        /// <summary>
        /// 41 - "@i"
        /// </summary>
        private void FINDUSER()
        {
            string[] szContent = Response.GetContentString().Split((char)9);
            //string sUsername = szContent[0];
            //string sSystem = szContent[1];

            Response.Initialize(147); // "BS"
            // TODO: messenger user information etc

            SendResponse();
        }
        /// <summary>
        /// 42 - "@j"
        /// </summary>
        private void APPROVENAME()
        {
            string sUsername = Request.PopFixedString();

            Response.Initialize(36); // "@d"
            Response.AppendInt32(0); // TODO: error code

            SendResponse();
        }
        /// <summary>
        /// 49 - "@q"
        /// </summary>
        private void GDATE()
        {
            Response.Initialize(163); // "Bc"
            Response.Append(DateTime.Today.ToShortDateString());
            
            SendResponse();
        }
        /// <summary>
        /// 196 - "CD"
        /// </summary>
        private void PONG()
        {

        }
        public void RegisterGlobal()
        {
            mRequestHandlers[9] = new RequestHandler(GETAVAILABLESETS);
            mRequestHandlers[41] = new RequestHandler(FINDUSER);
            mRequestHandlers[42] = new RequestHandler(APPROVENAME);
            mRequestHandlers[49] = new RequestHandler(GDATE);
        }
    }
}
