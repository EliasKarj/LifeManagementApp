using JokeConsole;
using LifeManagementApp.Models;

namespace LifeManagementApp.Interfaces
{
    public interface IJokeService
    {
        Task<List<Joke>> GetJokesAsync();
    }
}