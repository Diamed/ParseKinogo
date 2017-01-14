using System;

namespace ParseKinogo
{
	class Program
	{
		static void Main(string[] args)
		{
			// Все сразу страницы запускать нельзя, задудосим сайт - плохо
			for (int i=1;i<=554;i++)
			{
				Films films = new Films(i);
				films.Fill();
			}

			Console.ReadKey();
		}
	}
}
