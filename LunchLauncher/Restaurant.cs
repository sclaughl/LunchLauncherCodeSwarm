namespace LunchLauncher
{
    public class Restaurant
    {
        public Restaurant()
        { }

        public Restaurant(string name)
            : this()
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
