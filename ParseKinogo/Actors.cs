using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ParseKinogo
{
	/// <summary>
	/// Класс для загрузки актеров с сайта
	/// </summary>
	class Actors
	{
		Film _film;
		// В ролях.+</div>

		public Actors(Film film)
		{
			_film = film;
		}

		public async void AddActorsFromUriToDb()
		{
			Parser parser = new Parser("В ролях.+</div>", _film.Uri, false);
			List<Actor> actors = new List<Actor>();

			List<string> matches = await parser.Get() as List<string>;

			if (matches?.Count > 0)
			{
				actors = GetActorsFromString(matches?[0].ToString()) as List<Actor>;

				actors?.ForEach(a => AddToDatabase(a));
				//foreach (Actor actor in actors)
				//{
				//	AddToDatabase(actor);
				//}
			}
		}

		private static IEnumerable<Actor> GetActorsFromString(string stringToParse)
		{
			try
			{
				List<Actor> result = new List<Actor>();
				// var r ="В ролях:</b> Кейт Бекинсейл, Тео Джеймс, Брэдли Джеймс, Чарльз Дэнс, Алисия Вела-Бэйли, Тобайас Мензис, Лара Пулвер, Дэйзи Хэд, Трент Гарретт, Джеймс Фолкнер</div>";
				// Избавляемся от "В ролях:</b>"
				string resultList = stringToParse.Replace(stringToParse.Substring(0, stringToParse.IndexOf("</b>")), "");

				// Избавляемся от начального тега "</b>"
				resultList = resultList.Replace("</b>", "").Replace("</ b>","");

				// Избавляемся от последнего "</div>"
				resultList = resultList.Replace("</div>", "");

				// Заводим всю строку в массив, разбивая знаками "," (запятая)
				var actors = resultList.Split(',');

				// Перегоняем массив в список
				//result = actors.Select(a => new Actor(a.Trim())) as List<Actor>;

				actors.ToList().ForEach(a => result.Add(new Actor(a.Trim())));

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return null;
			}
		}

		private void AddToDatabase(Actor actor)
		{
			try
			{
				List<SqlParameter> parameters = new List<SqlParameter>()
				{
					new SqlParameter("Name", actor.Name),
					new SqlParameter("FilmId", _film.Id)
				};

				Database db = new Database();
				db.ExecProcedure("Films.AddActor", parameters);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}
	}
}
