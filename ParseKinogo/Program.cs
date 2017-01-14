using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Net;

namespace ParseKinogo
{
	class Program
	{
		static void Main(string[] args)
		{
			Test test = new Test();
			test.Fill();
		}

		
	}

	public class Test
	{
		HttpClientHandler handler;
		public Test()
		{
			//NetworkCredential proxyCreds = new NetworkCredential();

			//WebProxy proxy = new WebProxy("178.151.69.119:3128", false);
			//proxy.UseDefaultCredentials = true;
			//handler = new HttpClientHandler();
			//handler.Proxy = proxy;
			//handler.UseDefaultCredentials = true;
			handler = new HttpClientHandler();
			handler.UseCookies = true;
			handler.Proxy = WebRequest.DefaultWebProxy;
			handler.UseDefaultCredentials = true;
		}

		public async void Fill()
		{
			HttpClient httpClient = new HttpClient(handler);
			try
			{
				HttpResponseMessage response = null;
				while (response == null)
				{
					response = await httpClient.GetAsync("http://kinogo.club/");
				}
				response.EnsureSuccessStatusCode();

				string responseBodyAsText = await response.Content.ReadAsStringAsync();
				Regex reg = new Regex("<h2 class=\"zagolovki\">.+</h2>");
				MatchCollection matches = reg.Matches(responseBodyAsText);

				List<Film> films = new List<Film>();

				foreach (var m in matches)
				{
					films.Add(GetFilmFromString(m.ToString()));
				}

				films.ForEach(f => AddToDatabase(f));
			}
			catch (Exception ex)
			{
				int some = 0;
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
			film.Name = nameAndYear.Remove(nameAndYear.Length - 6).Trim();

			return film;
		}

		private static void AddToDatabase(Film film)
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
	}

	struct Film
	{
		public string Name { get; set; }
		public string Uri { get; set; }
		public string Year { get; set; }
	}
}
