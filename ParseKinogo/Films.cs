using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Linq;
using System.Data;

namespace ParseKinogo
{
	/// <summary>
	/// Класс для загрузки фильмов с сайта
	/// </summary>
	public class Films
	{
		int _page = 1;
		public Films(int page)
		{
			_page = page;

		}
		public async void Fill()
		{
			Parser parser = new Parser("<h2 class=\"zagolovki\">.+</h2>", $"http://kinogo.club/page/{_page}", false);

			var matches = await parser.Get();

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

		public static IEnumerable<Film> GetAll()
		{
			var dt = new Database().GetRowsUsingProcedure("Films.GetAll");
			List<Film> result = new List<Film>();

			foreach(DataRow row in dt.Rows)
			{
				result.Add
				(
					new Film((int)row["Id"], row["Name"].ToString(), row["URL"].ToString(), row["Year"].ToString())
				);
			}

			return result;
		}

		private static Film GetFilmFromString(string text)
		{
			Regex reg = new Regex("http.+html");

			string uri = reg.Match(text).ToString();

			reg = new Regex("html\">.+</a>");
			string nameAndYear = reg.Match(text).ToString().Replace("html\">", "").Replace("</a>", "");
			string year = nameAndYear.Substring(nameAndYear.Length - 6).Replace("(", "").Replace(")", "");
			string name = nameAndYear.Remove(nameAndYear.Length - 6).Trim().Normalize();

			return new Film(name, uri, year);
		}

		private static void AddToDatabase(Film film)
		{
			List<SqlParameter> parameters = new List<SqlParameter>()
			{
				new SqlParameter("Name", film.Name),
				new SqlParameter("Url", film.Uri),
				new SqlParameter("Year", film.Year)
			};

			Database database = new Database();
			database.ExecProcedure("Films.InsertFilm", parameters);
		}
	}
}
