using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace ParseKinogo
{
	class Films
	{
		int _page = 1;
		HttpClientHandler handler;
		public Films(int page)
		{
			_page = page;

			string address = ProxyServer.List[new Random().Next(1, 395)].ToString();
			WebProxy proxy = new WebProxy(address);
			handler = new HttpClientHandler();
			//handler.Proxy = proxy;
		}
		public async void Fill()
		{
			try
			{
				HttpResponseMessage response = await new HttpClient(handler).GetAsync($"http://kinogo.club/page/{_page}");
				response.EnsureSuccessStatusCode();

				byte[] bytes = await response.Content.ReadAsByteArrayAsync();
				string responseBodyAsText = Encoding.Default.GetString(bytes);

				Regex reg = new Regex("<h2 class=\"zagolovki\">.+</h2>");
				MatchCollection matches = reg.Matches(responseBodyAsText);

				List<Film> films = new List<Film>();

				foreach (var m in matches)
				{
					try
					{
						films.Add(GetFilmFromString(m.ToString()));
					}
					catch
					{

					}
				}

				films.ForEach(f => AddToDatabase(f));
				Console.WriteLine($"В базу занесена страница {_page}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Страница {_page} не была обработана. {ex.Message}");
			}
		}

		private static Film GetFilmFromString(string text)
		{
			Regex reg = new Regex("http.+html");
			Film film = new Film();
			film.Uri = reg.Match(text).ToString();

			reg = new Regex("html\">.+</a>");
			string nameAndYear = reg.Match(text).ToString().Replace("html\">", "").Replace("</a>", "");
			film.Year = nameAndYear.Substring(nameAndYear.Length - 6).Replace("(", "").Replace(")", "");
			film.Name = nameAndYear.Remove(nameAndYear.Length - 6).Trim().Normalize();

			return film;
		}

		private static void AddToDatabase(Film film)
		{
			try
			{
				List<SqlParameter> parameters = new List<SqlParameter>()
			{
				new SqlParameter("Name", film.Name),
				new SqlParameter("Url", film.Uri),
				new SqlParameter("Year", film.Year)
			};

				SqlConnection connection = new SqlConnection("Data Source=DIAMED;Initial Catalog=Private;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True");
				SqlCommand cmd = new SqlCommand("Films.InsertFilm", connection);
				cmd.CommandType = System.Data.CommandType.StoredProcedure;

				parameters.ForEach(p => cmd.Parameters.Add(p));

				connection.Open();
				cmd.ExecuteNonQuery();
				connection.Close();
			}
			catch
			{

			}
		}
	}

	struct Film
	{
		public string Name { get; set; }
		public string Uri { get; set; }
		public string Year { get; set; }
	}
}
