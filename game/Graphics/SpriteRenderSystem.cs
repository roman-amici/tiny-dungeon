using System.Globalization;
using TinyDungeon.MainGame;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace TinyDungeon.Graphics;

public class TransformSpriteSystem(RefTableJoin<RefSprite<HumanoidAnimationKey>, WorldPosition> sprites) : GameSystem
{
    public override void Execute()
    {
        foreach(var (sprite,position) in sprites)
        {
            sprite.Transform = position.Bounds;
        }
    }
}

public class RenderSpriteSystem(
    RefTable<RefSprite<HumanoidAnimationKey>> sprites,
    Screen screen,
    Camera camera) : GameSystem
{
    public override void Execute()
    {
        // TODO: Layers
       foreach(var sprite in sprites)
        {
            sprite.Atlas.DrawSprite(sprite, screen, camera);
        }
    }
}

public class AnimationAdvanceSystem(
    RefTableJoin<HumanoidAnimation,RefSprite<HumanoidAnimationKey>> animations,
    TimeDelta delta
) : GameSystem
{
    public override void Execute()
    {
        foreach(var (state,sprite) in animations)
        {
            state.Update(delta.Delta);

            var spriteKey = state.Animation.Sequence[state.FrameIndex.Index].Element;
            sprite.SpriteKey = spriteKey;
        }
    }
}