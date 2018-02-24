using LinqToDB.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Das alles hier wird nur gemacht, damit man den ConnectionString vom Code aus erzeugen kann und
// nicht in der app.config definieren muss (siehe https://github.com/linq2db/linq2db (.Net Core))

namespace Mrv.Regatta.PreisDesMinisterpraesidenten
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class DbConfigurations : ILinqToDBSettings
    {
        List<ConnectionStringSettings> _connectionStringSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConfiguration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public DbConfigurations()
        {
            _connectionStringSettings = new List<ConnectionStringSettings>();
        }

        /// <summary>
        /// Adds the specified db connection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public void Add(string name, string providerName, string connectionString)
        {
            _connectionStringSettings.Add(new ConnectionStringSettings()
            {
                Name = name,
                ProviderName = providerName,
                ConnectionString = connectionString
            });
        }

        public IEnumerable<IDataProviderSettings> DataProviders
        {
            get { yield break; }
        }

        public string DefaultConfiguration => "not set";
        public string DefaultDataProvider => "not set";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                foreach(var connectionStringSetting in _connectionStringSettings)
                {
                    yield return connectionStringSetting;
                }
            }
        }
    }

}
