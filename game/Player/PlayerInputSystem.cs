using TinyDungeon.Movement;
using TinyEngine.Ecs;
using TinyEngine.Input;

namespace TinyDungeon.Player;

public struct StartAttackMessage(EntityId entityId)
{
    public EntityId EntityId {get;} = entityId;
}

public class PlayerInputSystem(
    InputState inputState,
    RefSingletonJoin<PlayerState, HumanoidMovementState> playerState,
    Queue<StartAttackMessage> startAttack
) : GameSystem
{
    public override void Execute()
    {
        var player = playerState.Join;
        if (player == null)
        {
            return;
        }

        var (_, movement) = player.Value;

        if (movement.MovementState == Graphics.HumanoidState.Attack)
        {
            return;
        }

        if (inputState.KeysDown.Contains(Key.Space))
        {
            movement.MovementState = Graphics.HumanoidState.Attack;
            startAttack.Enqueue(new(playerState.S.EntityId!.Value));
        }
        else if (inputState.KeysDown.Contains(Key.Up))
        {
            movement.MovementState = Graphics.HumanoidState.Walk;
            movement.Direction = Graphics.SpriteDirection.Up;
        }
        else if (inputState.KeysDown.Contains(Key.Down))
        {
            movement.MovementState = Graphics.HumanoidState.Walk;
            movement.Direction = Graphics.SpriteDirection.Down;
        }
        else if (inputState.KeysDown.Contains(Key.Left))
        {
            movement.MovementState = Graphics.HumanoidState.Walk;
            movement.Direction = Graphics.SpriteDirection.Left;
        }
        else if (inputState.KeysDown.Contains(Key.Right))
        {
            movement.MovementState = Graphics.HumanoidState.Walk;
            movement.Direction = Graphics.SpriteDirection.Right;
        }
        else
        {
            movement.MovementState = Graphics.HumanoidState.Idle;
        }
    }
}