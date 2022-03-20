module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Game1 () as this =
    inherit Game ()

    [<return: Struct>]
    let (|KeyDown|_|) key (state: KeyboardState) =
        if state.IsKeyDown key
        then ValueSome()
        else ValueNone 

    let graphics =
        new GraphicsDeviceManager
            ( this
              , GraphicsProfile = GraphicsProfile.HiDef              
              , IsFullScreen = true )
    
    let mutable cameraTarget = Unchecked.defaultof<_>
    let mutable cameraPosition = Unchecked.defaultof<_>
    let mutable projectionMatrix = Unchecked.defaultof<_>
    let mutable viewMatrix = Unchecked.defaultof<_> 
    let mutable worldMatrix = Unchecked.defaultof<_>
    let mutable basicEffect = Unchecked.defaultof<_>
    let mutable vertexBuffer = Unchecked.defaultof<_>
    let mutable orbit = false
        
    do this.Content.RootDirectory <- "Content"
    do this.IsMouseVisible <- true 
    do graphics.ApplyChanges ()
    
    let (triangleVertices: VertexPositionColor[]) =
        [| VertexPositionColor (Vector3(0.f, 20.f, 0.f), Color.Red)
           VertexPositionColor (Vector3(20.f, -20.f, 0.f), Color.Green)
           VertexPositionColor (Vector3(20.f, -20.f, 0.f), Color.Blue) |]        
    
    override x.Initialize () =
        base.Initialize ()
        
        let graphicsDevice = this.GraphicsDevice
        
        cameraTarget <- Vector3 (0.f, 0.f, 0.f)
        cameraPosition <- Vector3 (0.f, 0.f, -100.f)
        
        projectionMatrix <-     
            Matrix.CreatePerspectiveFieldOfView
                ( MathHelper.ToRadians 45.f
                , graphicsDevice.DisplayMode.AspectRatio
                , 1.f
                , 1000.f )
                
        viewMatrix <-
            Matrix.CreateLookAt
                ( cameraPosition
                  , cameraTarget
                  , Vector3(0.f, 1.f, 0.f) )
                
        worldMatrix <- 
            Matrix.CreateWorld
                ( cameraTarget
                  , Vector3.Forward
                  , Vector3.Up )
            
        basicEffect <- 
            new BasicEffect
                ( graphicsDevice
                  , Alpha = 1.0f
                  , VertexColorEnabled = true
                  , LightingEnabled = false )
                
        vertexBuffer <- 
            new VertexBuffer
                ( graphicsDevice
                  , typeof<VertexPositionColor>
                  , 3
                  , BufferUsage.WriteOnly )
                
        vertexBuffer.SetData triangleVertices
        
    override this.Update gameTime =
        match Keyboard.GetState () with
        | KeyDown Keys.Space -> 
            orbit <- not orbit 
        | KeyDown Keys.Escape ->
            this.Exit ()
        | _ -> ()
        
        if orbit
        then
            let rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians 1.f)
            cameraPosition <- Vector3.Transform(cameraPosition, rotationMatrix)
        
        viewMatrix <-
            Matrix.CreateLookAt
                ( cameraPosition
                  , cameraTarget
                  , Vector3.Up )
        
        base.Update gameTime
        
    override this.Draw gameTime =
        let graphicsDevice = this.GraphicsDevice
        basicEffect.Projection <- projectionMatrix
        basicEffect.View <- viewMatrix
        basicEffect.World <- worldMatrix
        
        graphicsDevice.Clear Color.CornflowerBlue
        graphicsDevice.SetVertexBuffer vertexBuffer
        
        // Turn off backface culling
        graphicsDevice.RasterizerState <-
            new RasterizerState(CullMode = CullMode.None)
        
        basicEffect.CurrentTechnique.Passes
        |> Seq.iter (fun pass ->
            pass.Apply ()
            graphicsDevice.DrawPrimitives
                ( PrimitiveType.TriangleList
                  , 0
                  , 3 )
            )
        
        base.Draw gameTime
