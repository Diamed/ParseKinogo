using System.Collections.Generic;
using System.Data.SqlClient;

namespace ParseKinogo
{
	public class Database
	{
		private const string _connectionString = "Data Source=DIAMED;Initial Catalog=Private;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True";
		private SqlConnection _connection = new SqlConnection(_connectionString);

		public void ExecProcedure(string procedureName, List<SqlParameter> parameters = null)
		{
			SqlCommand cmd = new SqlCommand(procedureName, _connection)
			{
				CommandType = System.Data.CommandType.StoredProcedure,
			};

			parameters?.ForEach(p => cmd.Parameters.Add(p));

			OpenConnection();
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch
			{

			}
			CloseConnection();
		}

		private void OpenConnection()
		{
			try
			{
				_connection.Open();
			}
			catch
			{

			}
		}

		private void CloseConnection()
		{
			try
			{
				_connection.Close();
			}
			catch
			{

			}
		}
	}
}
