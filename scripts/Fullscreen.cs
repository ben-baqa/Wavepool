using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class FullScreen
{
    RenderTarget2D renderTarget;
    Rectangle renderTargetDestination;
    Point resolution;
    bool canToggleFullscreen = true;
    bool fullscreenToggled;

    GraphicsDeviceManager graphics;
    GraphicsDevice graphicsDevice;
    SpriteBatch spriteBatch;
    Color letterboxingColour;

    Vector2 letterBoxOffset;

    public FullScreen(Vector2 res, Color backgroundColour, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
    {
        resolution = new Point((int)res.X, (int)res.Y);

        this.graphics = graphics;
        graphics.PreferredBackBufferWidth = resolution.X;
        graphics.PreferredBackBufferHeight = resolution.Y;
        graphics.ApplyChanges();

        this.graphicsDevice = graphicsDevice;
        renderTarget = new RenderTarget2D(graphicsDevice, resolution.X, resolution.Y);
        renderTargetDestination = GetRenderTargetDestination(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

        this.spriteBatch = new SpriteBatch(graphicsDevice);
        letterboxingColour = backgroundColour;
    }

    public void Update()
    {
        var keyboard = Keyboard.GetState();
        if (canToggleFullscreen && keyboard.IsKeyDown(Keys.F11))
        {
            canToggleFullscreen = false;
            ToggleFullScreen();
        }
        else
            canToggleFullscreen = true;
    }

    public void Push()
    {
        graphicsDevice.SetRenderTarget(renderTarget);
        graphicsDevice.Clear(letterboxingColour);
    }

    public void Pop()
    {
        graphicsDevice.SetRenderTarget(null);
        graphicsDevice.Clear(letterboxingColour);

        spriteBatch.Begin();
        spriteBatch.Draw(renderTarget, renderTargetDestination, Color.White);
        spriteBatch.End();
    }

    public void ToggleFullScreen()
    {
        fullscreenToggled = true;
        if (!graphics.IsFullScreen)
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
        else
        {
            graphics.PreferredBackBufferWidth = resolution.X;
            graphics.PreferredBackBufferHeight = resolution.Y;
        }
        graphics.IsFullScreen = !graphics.IsFullScreen;
        graphics.ApplyChanges();

        renderTargetDestination = GetRenderTargetDestination(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
    }

    Rectangle GetRenderTargetDestination(int preferredBackBufferWidth, int preferredBackBufferHeight)
    {
        float resolutionRatio = (float)resolution.X / resolution.Y;
        float screenRatio;
        Point bounds = new Point(preferredBackBufferWidth, preferredBackBufferHeight);
        screenRatio = (float)bounds.X / bounds.Y;
        float scale;
        Rectangle rectangle = new Rectangle();

        letterBoxOffset = Vector2.Zero;
        if (resolutionRatio < screenRatio)
        {
            scale = (float)bounds.Y / resolution.Y;
            letterBoxOffset = new Vector2(bounds.X / scale - this.resolution.X, 0) / 2;
        }
        else if (resolutionRatio > screenRatio)
        {
            scale = (float)bounds.X / resolution.X;
            letterBoxOffset = new Vector2(bounds.Y / scale - this.resolution.Y, 0) / 2;
        }
        else
        {
            // Resolution and window/screen share aspect ratio
            rectangle.Size = bounds;
            return rectangle;
        }
        rectangle.Width = (int)(resolution.X * scale);
        rectangle.Height = (int)(resolution.Y * scale);

        return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
    }

    static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
    {
        Point delta = outerRectangle.Center - innerRectangle.Center;
        innerRectangle.Offset(delta);
        return innerRectangle;
    }

    public void OnWindowSizeChanged(Rectangle clientBounds)
    {
        if (fullscreenToggled)
        {
            fullscreenToggled = false;
            return;
        }

        graphics.PreferredBackBufferWidth = clientBounds.Width;
        graphics.PreferredBackBufferHeight = clientBounds.Height;
        graphics.ApplyChanges();

        renderTargetDestination = GetRenderTargetDestination(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
    }

    public Vector2 ScreenToGamePoint(Vector2 pos)
    {
        float screenWidth = graphics.PreferredBackBufferWidth;
        float screenHeight = graphics.PreferredBackBufferHeight;
        Vector2 offset = Vector2.Zero;
        float scale = 1;

        float resolutionRatio = (float)resolution.X / resolution.Y;
        float screenRatio = (float)screenWidth / screenHeight;

        if (resolutionRatio < screenRatio)
        {
            scale = (float)screenHeight / resolution.Y;
            offset = new Vector2(screenWidth / scale - resolution.X, 0) / 2;
        }
        else if (resolutionRatio > screenRatio)
        {
            scale = (float)screenWidth / resolution.X;
            offset = new Vector2(0, screenHeight / scale - resolution.Y) / 2;
        }

        pos = pos / scale;


        return pos - letterBoxOffset;
    }
    public Vector2 ScreenToGamePoint(Point p)
    {
        Vector2 pos = new Vector2(p.X, p.Y);
        return ScreenToGamePoint(pos);
    }
}