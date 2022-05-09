// See https://aka.ms/new-console-template for more information
using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Numerics;
using SkiaSharp;
using Eden;
using System.Collections.Generic;
using Facebook.Yoga;
using Topten.RichTextKit;

Panel root = new();

YogaNode node = new();

IWindow window;

SKColorType format = SKColorType.Rgba8888;

GRGlInterface skiaGlInterface = null;
GRContext skiaBackendContext = null;
GRBackendRenderTarget skiaBackendRenderTarget = null;
// basic skia stuff
SKSurface surface = null; // this surface is acually our window's buffer
SKCanvas canvas = null;
// for drawing
SKPaint paint = null;
SKPaint paint2 = null;

var textPaint = new SKPaint();
textPaint.Color = new SKColor(255, 255, 255);
textPaint.TextSize = 24;
textPaint.Typeface = SKTypeface.FromFamilyName("Arial");

// create window and listen for events.
var WindowConfig = WindowOptions.Default;
WindowConfig.Size = new Silk.NET.Maths.Vector2D<int>(1280, 700);
WindowConfig.Title = "Eden";
window = Window.Create(WindowConfig);

IKeyboard primaryKeyboard;
Vector2 MousePosition = default;

window.Load += () =>
{
    var child = new Panel();
    var child2 = new Panel();

    root.Width = window.Size.X;
    root.Height = window.Size.Y;
    root._node.Display = YogaDisplay.Flex;
    root._node.FlexDirection = YogaFlexDirection.Row;
    root._node.Wrap = YogaWrap.Wrap;
    root._node.JustifyContent = YogaJustify.SpaceBetween;
    root._node.AlignItems = YogaAlign.Center;
    child.Width = 20.Percent();
    child._node.Margin = 50.Pt();
    child.Height = 30.Pt();
    child2._node.CopyStyle(child);
    root.AddChild(child);
    root.AddChild(child2);

    for (int i = 30; i > 0; i--)
    {
        Panel p = new();
        p._node.CopyStyle(child);
        root.AddChild(p);
    }

    root.RecalculateLayout();

    skiaGlInterface = GRGlInterface.CreateOpenGl(name =>
    {
        if (window.GLContext.TryGetProcAddress(name, out nint fn))
            return fn;
        return (nint)0;
    });

    skiaBackendContext = GRContext.CreateGl(skiaGlInterface);

    format = SKColorType.Rgba8888;

    // create a skia backend render target for the window.
    skiaBackendRenderTarget = new GRBackendRenderTarget(
        window.Size.X, window.Size.Y, // window size
        window.Samples ?? 1, window.PreferredStencilBufferBits ?? 16, // 
        new GRGlFramebufferInfo(
            0, // use the window's framebuffer
            format.ToGlSizedFormat()
            )
        );


    // create a surface from the render target
    surface = SKSurface.Create(skiaBackendContext, skiaBackendRenderTarget, format);

    canvas = surface.Canvas;

    paint = new SKPaint();
    paint.Color = new SKColor(0, 0, 255);

    paint2 = new SKPaint();
    paint2.Color = new SKColor(0, 255, 0);

    IInputContext input = window.CreateInput();
    primaryKeyboard = input.Keyboards.FirstOrDefault();
    if (primaryKeyboard != null)
    {
        primaryKeyboard.KeyDown += KeyDown;
    }
    for (int i = 0; i < input.Mice.Count; i++)
    {
        input.Mice[i].Cursor.CursorMode = CursorMode.Normal;
        input.Mice[i].MouseMove += OnMouseMove;
        input.Mice[i].Scroll += OnMouseWheel;
    }

};

window.Render += deltaTime =>
{
    // draw a basic rectangle

    canvas.Clear(new SKColor(255, 0, 0));

    canvas.DrawRect(root.X, root.Y, root.Width.Value, root.Height.Value, paint);

    foreach (Panel child in root.Children)
    {
        canvas.DrawRect(child.X, child.Y, child.Width.Value, child.Height.Value, paint2);
        canvas.DrawText($"{child._node}", new SKPoint(child.X, child.Y+15), textPaint);
    }

    canvas.DrawText($"{window.Size}", new SKPoint(0,20), textPaint);
    canvas.DrawText($"{root._node}", new SKPoint(0,40), textPaint);

    canvas.DrawText($"{deltaTime*60*60}", new SKPoint(0,60), textPaint);

    canvas.DrawCircle(new SKPoint(MousePosition.X, MousePosition.Y), 2, paint2);

    canvas.Flush(); // wait for commands to finish
};

window.Resize += newSize =>
{

    
};

window.FramebufferResize += newSize =>
{
    skiaBackendRenderTarget.Dispose();
    surface.Dispose();
    canvas.Dispose();

    skiaBackendRenderTarget = new GRBackendRenderTarget(
    newSize.X, newSize.Y, // window size
    window.Samples ?? 1, window.PreferredStencilBufferBits ?? 16, // 
    new GRGlFramebufferInfo(
        0, // use the window's framebuffer
        format.ToGlSizedFormat()
        )
    );

    surface = SKSurface.Create(skiaBackendContext, skiaBackendRenderTarget, format);
    canvas = surface.Canvas;
    root.Width = newSize.X;
    root.Height = newSize.Y;
    root.RecalculateLayout();

    Console.WriteLine(newSize);
    Console.WriteLine(skiaBackendRenderTarget.Size);

};

void OnMouseMove(IMouse mouse, Vector2 position)
{
    if (MousePosition == default) { MousePosition = position; }
    else
    {
        MousePosition = position;
    }
}

void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
{

}

void KeyDown(IKeyboard keyboard, Key key, int arg3)
{
    if (key == Key.Escape)
    {
        window.Close();
    }
}

window.Run();

// clean up resources
canvas.Dispose();
surface.Dispose();
skiaBackendRenderTarget.Dispose();
skiaBackendContext.Dispose();
skiaGlInterface.Dispose();
window.Dispose();