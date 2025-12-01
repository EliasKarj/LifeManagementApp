namespace LifeManagementApp.Models
{

    public abstract class Joke
    {
        public string? Category { get; set; }
        public abstract override string ToString();
    }

    public class SingleJoke : Joke
    {
        public string? JokeText { get; set; }

        public override string ToString() => $"{Category}: {JokeText}";
    }

    public class TwoPartJoke : Joke
    {
        public string? Setup { get; set; }
        public string? Delivery { get; set; }

        public override string ToString() => $"{Category}: {Setup} ... {Delivery} 😂";
    }

    public class JokeDto
    {
        public string? type { get; set; }
        public string? category { get; set; }
        public string? joke { get; set; }
        public string? setup { get; set; }
        public string? delivery { get; set; }
        public List<JokeDto>? jokes { get; set; }
    }
}