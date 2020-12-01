namespace NeoServer.Data
{
    public class DbContextFactory
    {
        private static DbContextFactory _instance;

        public static DbContextFactory GetInstance()
        {
            if (_instance == null)
                _instance = new DbContextFactory();

            return _instance;
        }
    }
}
