using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Data.NH
{
    public interface IConnectionProvider : IDisposable
    {
        IDbConnection GetConnection();
    }

    public class ConnectionProvider : IConnectionProvider
    {
        private readonly Lazy<IDbConnection> conn;


        public ConnectionProvider(Func<IDbConnection> connFunc)
        {
            this.conn = new Lazy<IDbConnection>(connFunc );
        }
        public IDbConnection GetConnection()
        {
            return conn.Value;
        }

        public void Dispose()
        {
            if (conn.IsValueCreated)
            {
                conn.Value.Close();
                conn.Value.Dispose();
            }
        }
    }

}
