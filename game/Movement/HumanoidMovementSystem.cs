using TinyDungeon.Graphics;
using TinyEngine.Ecs;

namespace TinyDungeon.Movement;

public class HumanoidMovementState
{
    public HumanoidState MovementState {get; set;} = HumanoidState.Idle;
    public SpriteDirection Direction {get; set;} = SpriteDirection.Down;
    public double WalkSpeed {get; set;} = 100.0;
}

public class HumanoidMovementSystem(
    RefTableJoin<HumanoidMovementState, Velocity> velocities,
    RefTableJoin<HumanoidMovementState, HumanoidAnimation> animations
) : GameSystem
{
    public override void Execute()
    {
        UpdateVelocities();
        UpdateAnimations();
    }

    private void UpdateVelocities()
    {
        foreach(var (state,velocity) in velocities)
        {
            if (state.MovementState != HumanoidState.Walk)
            {
                velocity.Value = new(0,0);
                continue;
            }

            if (state.MovementState == HumanoidState.Walk)
            {
                velocity.Value = state.Direction switch
                {
                    SpriteDirection.Up => new(0, -state.WalkSpeed),
                    SpriteDirection.Down => new(0, state.WalkSpeed),
                    SpriteDirection.Left => new(-state.WalkSpeed, 0),
                    SpriteDirection.Right => new(state.WalkSpeed, 0),
                    _ => new()
                };
            }
        }
    }

    private void UpdateAnimations()
    {
        foreach(var (state, animation) in animations)
        {
            if (state.MovementState is 
                HumanoidState.Idle or
                HumanoidState.Walk)
            {
                animation.SwapAnimation(animation.SpriteSheet.Animations.Get(state.MovementState, state.Direction));
            }
        }
    }
}