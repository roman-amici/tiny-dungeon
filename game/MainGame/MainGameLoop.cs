using SDL2;
using TinyDungeon.Graphics;
using TinyDungeon.MainGame;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.Engine;
using TinyEngine.SdlAbstractions;

namespace TinyDungeon;

public class MainGameLoop : GameLoop
{
    public List<GameSystem> Schedule {get;} = new();
    public TimeDelta TimeDelta {get;} = new();
    public Screen Screen {get;  }
    public World World {get; }

    public MainGameLoop() : base(new())
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            throw new Exception($"Failed to start SDL2: {SDL.SDL_GetError()}");
        }

        Screen = new Screen(1280, 720, "TinyDungeon");

        var galTexture = Texture.LoadTexture(Screen.Renderer.RendererPtr, "Assets/Gal.png" );
        var overlays = new SpritesOverlay()
        {
            PlayerCharacter = new HumanoidSpriteSheet(galTexture)
        };

        World = MainGameWorldBuilder.Build(Screen, InputState, TimeDelta, overlays);
        Schedule.Clear();
        Schedule.AddRange(MainGameWorldBuilder.BuildSchedule(World));
    }

    public override bool Tick(TimeSpan delta)
    {
        TimeDelta.Delta = delta;
        Screen.Renderer.Clear();
        foreach(var system in Schedule)
        {
            system.Execute();
        }
        Screen.Renderer.Present();

        return true;
    }
}

public class TimeDelta
{
    public TimeSpan Delta {get; set;}
}