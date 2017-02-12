using System.Collections.Generic;
using System.Data;
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
				CommandType = CommandType.StoredProcedure,
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

		public DataTable GetRowsUsingProcedure(string procedureName, List<SqlParameter> parameters = null)
		{
			using (SqlDataAdapter dataAdapter = new SqlDataAdapter(procedureName, _connection))
			{
				dataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

				parameters?.ForEach(p => dataAdapter.SelectCommand.Parameters.Add(p));

				using (DataTable dt = new DataTable())
				{
					OpenConnection();
					try
					{
						dataAdapter.Fill(dt);

						return dt;
					}
					catch
					{

					}
					CloseConnection();
				}
			}

			return new DataTable();
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
