using Box2DX.Collision;
using Box2DX.Dynamics;

namespace SharpNeat.DomainsExtra.WalkerBox2d
{
    public static class WalkerWorldUtils
    {
        public static void CreateMound(World world, float x, float y, float friction, float restitution)
        {
            // ==== Mound ====
            BodyDef moundBodyDef = new BodyDef();
            moundBodyDef.Position.Set(x, y);

            // Mound body.
            Body moundBody = world.CreateBody(moundBodyDef);

            // Define shape.
            PolygonDef moundShapeDef = new PolygonDef();
            moundShapeDef.VertexCount = 3;
            moundShapeDef.Vertices[0].Set(0.20f, 0f);
            moundShapeDef.Vertices[1].Set(0f, 0.01f);
            moundShapeDef.Vertices[2].Set(-0.20f, 0f);
            moundShapeDef.Friction = friction;
            moundShapeDef.Restitution = restitution;
            moundShapeDef.Filter.CategoryBits = 0x3;

            // Add shape to body.
            moundBody.CreateShape(moundShapeDef);
        }
    }
}
