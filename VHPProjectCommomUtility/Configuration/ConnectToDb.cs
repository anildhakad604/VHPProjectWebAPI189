namespace VHPProjectCommonUtility.Configuration
{
    public class ConnectToDb
    {
        private string _connectionString;

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public int ConnectionTimeOutSec { get; set; } = 300;
        public int CommandTimeOutSec { get; set; } = 600;
    }
}
