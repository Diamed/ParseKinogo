namespace ParseKinogo
{
	public class Film
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public string Uri { get; private set; }
		public string Year { get; private set; }

		public Film(string name, string uri, string year)
		{
			Name = name;
			Uri = uri;
			Year = year;
		}

		public Film(int id, string name, string uri, string year)
		{
			Id = id;
			Name = name;
			Uri = uri;
			Year = year;
		}
	}
}
