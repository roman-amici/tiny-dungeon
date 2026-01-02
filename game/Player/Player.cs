using TinyDungeon.Graphics;
using TinyDungeon.MainGame;
using TinyDungeon.Movement;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.General;

namespace TinyDungeon.Player;

public class PlayerState
{
}

public struct SpawnPlayerMessage
{
    public Point2D SpawnPosition {get; set;}
}

public class PlayerSpawner(
    World world, 
    Queue<SpawnPlayerMessage> spawnPlayer,
    SpritesOverlay overlays,
    Singleton<PlayerState> player,
    RefTable<HumanoidMovementState> movementStates,
    RefTable<Velocity> velocity,
    RefTable<WorldPosition> positions,
    RefTable<HumanoidAnimation> animations,
    RefTable<RefSprite<HumanoidAnimationKey>> sprites) : SpawningSystem<SpawnPlayerMessage>(world)
{
    public override void Execute()
    {
        while(spawnPlayer.TryDequeue(out var message))
        {
            SpawnEntity(message);
        }
    }

    protected override void Spawn(EntityId entityId, SpawnPlayerMessage context)
    {
        var playerState = new PlayerState();
        player.Spawn(entityId, playerState);

        var movement = new HumanoidMovementState();
        movementStates.Add(entityId, movement);

        var bounds = new Rect2D(new(0,0), 32,32);
        bounds.WithCenter(context.SpawnPosition);
        positions.Add(entityId, new(bounds));

        var spriteSheet = overlays.PlayerCharacter;
        animations.Add(entityId, new(spriteSheet));
        sprites.Add(entityId, new(spriteSheet.SpriteAtlas, default));
        velocity.Add(entityId, new(new()));
    }
}