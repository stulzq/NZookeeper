namespace NZookeeper
{
    public class ZkConnectionOptions
    {
        /// <summary>
        /// Zookeeper address, multiple addresses use ',' separated
        /// </summary>
        public string ConnectionString { get; set; }

        public int SessionTimeout { get; set; }
    }
}