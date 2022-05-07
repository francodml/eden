// See https://aka.ms/new-console-template for more information
using Flexbox;
using Silk.NET.Windowing;
using SkiaSharp;
using System.Collections.Generic;

Node root = new();

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

// create window and listen for events.
window = Window.Create(WindowOptions.Default);
window.WindowBorder = WindowBorder.Resizable;

window.Load += () =>
{
    var child = new Node();
    var child2 = new Node();

    root.nodeStyle.Apply("width: 100%; height: 100%; flex-direction: column; justify-content: space-around; align-items: center;");
    child.nodeStyle.Apply("width: 50px; height: 50px;");
    child2.nodeStyle.Apply("width: 50px; height: 50px;");
    root.AddChild(child);
    root.AddChild(child2);

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
};

window.Render += deltaTime =>
{
    // draw a basic rectangle
    root.MarkAsDirty();
    root.CalculateLayout(skiaBackendRenderTarget.Width, skiaBackendRenderTarget.Height, Direction.LTR);

    canvas.Clear(new SKColor(255, 0, 0));

    canvas.DrawRect(root.layout.x, root.layout.y, root.layout.width, root.layout.height, paint);

    for (int i = 0; i < root.ChildrenCount; i++)
    {
        Node child = root.GetChild(i);
        canvas.DrawRect(child.layout.x, child.layout.y, child.layout.width, child.layout.height, paint2);
    }

    var textPaint = new SKPaint();
    textPaint.Color = new SKColor(255, 255, 255);
    textPaint.Typeface = SKTypeface.FromFamilyName("Segoe UI");
    canvas.DrawText($"{window.Size}", new SKPoint(0,20), textPaint);
    canvas.DrawText($"{root.layout}", new SKPoint(0,35), textPaint);


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
    root.nodeStyle.Apply($"width: {newSize.X}px; height: {newSize.Y}px;");
    root.MarkAsDirty();

    Flex.CalculateLayout(root, newSize.X, newSize.Y, Direction.LTR);
    Console.WriteLine(newSize);
    Console.WriteLine(skiaBackendRenderTarget.Size);
    Console.WriteLine(root.layout.ToString());

};

window.Run();

// clean up resources
canvas.Dispose();
surface.Dispose();
skiaBackendRenderTarget.Dispose();
skiaBackendContext.Dispose();
skiaGlInterface.Dispose();
window.Dispose();