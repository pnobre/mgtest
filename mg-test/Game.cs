using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mg_test;

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    private Vector3 _cameraTarget;
    private Vector3 _cameraPosition;
    private Matrix _projectionMatrix;
    private Matrix _viewMatrix;
    private Matrix _worldMatrix;

    private BasicEffect _basicEffect;
    private VertexPositionColor[] _triangleVertices;
    private VertexBuffer _vertexBuffer;
    private bool _orbit = false;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true; 
        graphics.GraphicsProfile = GraphicsProfile.HiDef;
        graphics.IsFullScreen = false;
        graphics.ApplyChanges(); 
    }

    protected override void Initialize()
    {
        base.Initialize();
        _cameraTarget = new Vector3(0f, 0f, 0f);
        _cameraPosition = new Vector3(0f, 0f, -100f);
        _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45f),
            GraphicsDevice.DisplayMode.AspectRatio,
            1f,
            1000f);

        _viewMatrix = Matrix.CreateLookAt(
            _cameraPosition, 
            _cameraTarget, 
            new Vector3(0f, 1f, 0f)); // Y up
        
        _worldMatrix = Matrix.CreateWorld(
            _cameraTarget, 
            Vector3.Forward, 
            Vector3.Up);

        _basicEffect = new BasicEffect(GraphicsDevice);
        _basicEffect.Alpha = 1f;
        _basicEffect.VertexColorEnabled = true;
        _basicEffect.LightingEnabled = false; 
        
        _triangleVertices = new VertexPositionColor[3];
        _triangleVertices[0] = new VertexPositionColor(new Vector3(
            0, 20, 0), Color.Red);
        _triangleVertices[1] = new VertexPositionColor(new Vector3(-
            20, -20, 0), Color.Green);
        _triangleVertices[2] = new VertexPositionColor(new Vector3(
            20, -20, 0), Color.Blue);
        
        _vertexBuffer = new VertexBuffer(
            GraphicsDevice, 
            typeof(VertexPositionColor), 
            3, 
            BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_triangleVertices);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            _orbit = !_orbit;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        if (_orbit)
        {
            var rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
            _cameraPosition = Vector3.Transform(_cameraPosition, rotationMatrix);
        }

        _viewMatrix = Matrix.CreateLookAt(_cameraPosition, _cameraTarget, Vector3.Up); 
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _basicEffect.Projection = _projectionMatrix;
        _basicEffect.View = _viewMatrix;
        _basicEffect.World = _worldMatrix; 
        
        GraphicsDevice.Clear (Color.CornflowerBlue);
        GraphicsDevice.SetVertexBuffer(_vertexBuffer);

        var rasterizerState = new RasterizerState();
        rasterizerState.CullMode = CullMode.None;

        GraphicsDevice.RasterizerState = rasterizerState;

        foreach (var pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawPrimitives(
                PrimitiveType.TriangleList, 
                0, 
                3);
        }
        base.Draw(gameTime);
    }
}