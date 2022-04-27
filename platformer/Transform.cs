using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using Vector2 = tainicom.Aether.Physics2D.Common.Vector2;

public struct Transform
{
    public Vector2 Position { get => Body.Position; set { Body.Position = value; } }
    public Body Body;
}