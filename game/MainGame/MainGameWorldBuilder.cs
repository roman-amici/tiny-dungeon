using TinyDungeon.Graphics;
using TinyDungeon.Movement;
using TinyDungeon.Player;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.Input;

namespace TinyDungeon.MainGame;

public static class MainGameWorldBuilder
{
    public static World Build(Screen screen, InputState inputState, TimeDelta delta, SpritesOverlay sprites)
    {
        var world = new World();

        world.AddResource(inputState);
        world.AddResource(screen);
        world.AddResource(sprites);
        world.AddResource(delta);
        var q = new Queue<SpawnPlayerMessage>();
        q.Enqueue(new()
        {
            SpawnPosition = new(250,250)
        });
        world.AddResource(q);
        world.AddResource(new Queue<StartAttackMessage>());

        world.AddResource(new Camera(screen.Window.Width, screen.Window.Height, 1.0));

        world.AddComponent(new RefTable<HumanoidAnimation>());
        world.AddComponent(new RefTable<WorldPosition>());
        world.AddComponent(new RefTable<RefSprite<HumanoidAnimationKey>>());
        world.AddComponent(new RefTable<Velocity>());
        world.AddComponent(new RefTable<HumanoidMovementState>());
        world.AddComponent(new Singleton<PlayerState>());

        return world;
    }

    public static IEnumerable<GameSystem> BuildSchedule(World world)
    {
        var schedule = new List<GameSystem>();

        var playerSpawner = world.CreateInstance<PlayerSpawner>();
        playerSpawner.Execute();

        schedule.Add(world.CreateInstance<PlayerInputSystem>());

        schedule.Add(world.CreateInstance<HumanoidMovementSystem>());
        schedule.Add(world.CreateInstance<MovementSystem>());

        schedule.AddRange(BuildRenderSystems(world));

        return schedule;
    }

    public static IEnumerable<GameSystem> BuildRenderSystems(World world)
    {
        return [
          world.CreateInstance<AnimationAdvanceSystem>(),
          world.CreateInstance<TransformSpriteSystem>(),
          world.CreateInstance<RenderSpriteSystem>()  
        ];
    }
}