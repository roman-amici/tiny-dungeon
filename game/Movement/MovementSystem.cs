using TinyDungeon.MainGame;
using TinyEngine.Ecs;
using TinyEngine.General;

namespace TinyDungeon.Movement;

public class Velocity(Vector2D value)
{
    public Vector2D Value {get; set;} = value;
}

public class MovementSystem(
    TimeDelta delta,
    RefTableJoin<Velocity,WorldPosition> velocities
) : GameSystem
{
    public override void Execute()
    {
        foreach(var (velocity,position) in velocities)
        {
            position.Bounds = position.Bounds.Translated( delta.Delta.TotalSeconds * velocity.Value);
        }
    }
}