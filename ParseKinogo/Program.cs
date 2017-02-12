using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace ParseKinogo
{
	class Program
	{
		static void Main(string[] args)
		{
			//FillFilmsFromSite();

			FillActorsFromSite();
			
			Console.ReadKey();
		}

		private static void FillFilmsFromSite()
		{
			// Все сразу страницы запускать нельзя, задудосим сайт - плохо
			// Либо включаем ожидание
			for (int i = 1; i <= 1; i++)
			{
				Films films = new Films(i);
				films.Fill();

				Thread.Sleep(2000);
			}
		}

		private static void FillActorsFromSite()
		{
			List<Film> films = Films.GetAll() as List<Film>;

			foreach (Film film in films)
			{
				Actors actors = new Actors(film);

				actors.AddActorsFromUriToDb();

				Thread.Sleep(2000);
				Console.WriteLine($"В фильме {film.Name} все актеры добавлены в БД");
			}

			Console.WriteLine("Все актеры добавлены в базу данных");
		}

		private static void FillActorsFromSiteTest()
		{
			Film film = Films.GetAll().First();

			Actors actors = new Actors(film);

			actors.AddActorsFromUriToDb();

			Thread.Sleep(2000);

			Console.WriteLine("Все актеры добавлены в базу данных");
		}
	}
}
