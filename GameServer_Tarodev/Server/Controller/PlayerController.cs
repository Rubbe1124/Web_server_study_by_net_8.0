using Microsoft.AspNetCore.Mvc;
using Server.Services;
using SharedLibrary;

namespace Server.Controller;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly GameDbContext _context;

    public PlayerController(IPlayerService playerService, GameDbContext context)
    {
        _playerService = playerService;
        _context = context;

        var user = new User()
        {
            UserName = "taro",
            PasswordHash = "password58",
            Salt = "skdmnkdieikdk"
        };

        _context.Add(user);
        
        _context.SaveChanges();
    }
    
    [HttpGet("{id}")]
    public Player Get([FromRoute]int id)
    {
        var player = new Player(){Id = id};
        
        _playerService.DoSomething();

        return player;
    }

    [HttpPost]
    public Player Post(Player player)
    {
        Console.WriteLine("Player has been added to the DB");
        return player;
    }
}