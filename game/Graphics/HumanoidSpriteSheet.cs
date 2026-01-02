using System.Diagnostics.CodeAnalysis;
using TinyEngine.Drawing;
using TinyEngine.General;
using TinyEngine.SdlAbstractions;

namespace TinyDungeon.Graphics;

public class HumanoidAnimation(HumanoidSpriteSheet spriteSheet)
{
    public HumanoidSpriteSheet SpriteSheet {get; set;} = spriteSheet;
    public Animation<HumanoidAnimationKey> Animation {get; private set;} = spriteSheet.Animations.GetDefault();
    public TimerIndex FrameIndex {get; set;}

    public void Update(TimeSpan delta)
    {
        if(Animation.NextIndex(FrameIndex).TimeInState == TimeSpan.MaxValue)
        {
            return;
        }

        var advance = FrameIndex;
        advance.TimeInState += delta;
        
        FrameIndex = Animation.NextIndex(advance);
    }

    public void SwapAnimation(Animation<HumanoidAnimationKey> newAnimation)
    {
        Animation = newAnimation;
        if (FrameIndex.Index > Animation.Sequence.Length)
        {
            FrameIndex = new();
        }
    }

    public bool IsAtEnd()
    {
        return Animation.IsComplete(FrameIndex);
    }
}

public class HumanoidSpriteSheet
{
    public HumanoidSpriteSheet(Texture spriteSheet)
    {
        SpriteAtlas = new(spriteSheet);

        AddSet(HumanoidState.Idle, 0*32, 4);
        AddSet(HumanoidState.Walk, 4*32, 4);
        AddSet(HumanoidState.Attack, 8*32, 4);

        AddSet(HumanoidState.FlashYellow, 12*32, 1);
        AddSet(HumanoidState.FlashRed, 13*32, 1);

    }

    public SpriteAtlas<HumanoidAnimationKey> SpriteAtlas {get;}
    public HumanoidAnimations Animations {get;} = new();

    void AddSet(HumanoidState state, int startX, int nSet)
    {
        for (var i = 0; i < nSet; i++)
        {
            var x = startX + 32 * i;
            SpriteAtlas.AddSprite(new(state, SpriteDirection.Up, i), x, 0, 32, 32);
            SpriteAtlas.AddSprite(new(state, SpriteDirection.Right, i), x, 32, 32, 32);
            SpriteAtlas.AddSprite(new(state, SpriteDirection.Down, i), x, 64, 32, 32);
            SpriteAtlas.AddSprite(new(state, SpriteDirection.Left, i), x, 96, 32, 32);
        }
    }
}

public struct HumanoidAnimationKey(HumanoidState state, SpriteDirection direction, int index)
{
    public (HumanoidState, SpriteDirection, int) Key {get;} = (state,direction,index);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is HumanoidAnimationKey key)
        {
            return key.Key == Key;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}

public class HumanoidAnimations
{

    public HumanoidAnimations()
    {
        MakeFrames(HumanoidState.Walk, SpriteDirection.Up, TimeSpan.FromMilliseconds(100));
        MakeFrames(HumanoidState.Walk, SpriteDirection.Down, TimeSpan.FromMilliseconds(100));
        MakeFrames(HumanoidState.Walk, SpriteDirection.Right, TimeSpan.FromMilliseconds(100));
        MakeFrames(HumanoidState.Walk, SpriteDirection.Left, TimeSpan.FromMilliseconds(100));

        MakeFrames(HumanoidState.Attack, SpriteDirection.Up, TimeSpan.FromMilliseconds(100));
        MakeFrames(HumanoidState.Attack, SpriteDirection.Down, TimeSpan.FromMilliseconds(100));
        MakeFrames(HumanoidState.Attack, SpriteDirection.Right, TimeSpan.FromMilliseconds(100));
        MakeFrames(HumanoidState.Attack, SpriteDirection.Left, TimeSpan.FromMilliseconds(100));

        MakeFrames(HumanoidState.Idle, SpriteDirection.Up, TimeSpan.MaxValue);
        MakeFrames(HumanoidState.Idle, SpriteDirection.Down, TimeSpan.MaxValue);
        MakeFrames(HumanoidState.Idle, SpriteDirection.Right, TimeSpan.MaxValue);
        MakeFrames(HumanoidState.Idle, SpriteDirection.Left, TimeSpan.MaxValue);

    }

    void MakeFrames(HumanoidState state, SpriteDirection direction, TimeSpan duration)
    {
        Animations.Add( new(state,direction), new([
            new(new(state, direction, 0), duration),
            new(new(state, direction, 1), duration),
            new(new(state, direction, 2), duration),
            new(new(state, direction, 3), duration)
        ], true));
    }

    Dictionary<(HumanoidState,SpriteDirection), Animation<HumanoidAnimationKey>> Animations {get;} = new();

    public Animation<HumanoidAnimationKey> Get(HumanoidState state, SpriteDirection direction)
    {
        return Animations[(state,direction)];
    }

    public Animation<HumanoidAnimationKey> GetDefault()
    {
        return Animations.Values.First();
    }
}

public enum HumanoidState
{
    Idle,
    Walk,
    Attack,
    FlashYellow,
    FlashRed,

}

public enum SpriteDirection
{
    Up,
    Down,
    Left,
    Right
}