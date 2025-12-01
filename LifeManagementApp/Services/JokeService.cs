using System.Text.Json;
using LifeManagementApp.Interfaces;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services
{
    public class JokeService : IJokeService
    {
        private readonly HttpClient _httpClient;

        private const string Url = "https://v2.jokeapi.dev/joke/Programming?blacklistFlags=nsfw,religious,political,racist,sexist,explicit&amount=1";

        public JokeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Joke>> GetJokesAsync()
        {
            try
            {
                var json = await _httpClient.GetStringAsync(Url);

                var singleDto = JsonSerializer.Deserialize<JokeDto>(json);
                var jokes = new List<Joke>();

                if (singleDto != null)
                {
                    if (singleDto.type == "single")
                    {
                        jokes.Add(new SingleJoke { JokeText = singleDto.joke, Category = singleDto.category });
                    }
                    else if (singleDto.type == "twopart")
                    {
                        jokes.Add(new TwoPartJoke { Setup = singleDto.setup, Delivery = singleDto.delivery, Category = singleDto.category });
                    }
                }

                return jokes;
            }
            catch (Exception ex)
            {
                return new List<Joke> { new SingleJoke { JokeText = "Vitsien haku epäonnistui: " + ex.Message } };
            }
        }
    }
}