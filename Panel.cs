using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SkiaSharp;
using Facebook.Yoga;
using System.Numerics;

namespace Eden
{

    public struct Bounds
    {
        public Vector2 Min;
        public Vector2 Max;
        public Bounds(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }
        public bool IsInside(Vector2 p)
        {
            return p.X >= Min.X && p.X <= Max.X && p.Y >= Min.Y && p.Y <= Max.Y;
        }
    }
    public class Panel
    {
        public bool Active = false;
        public bool Hovered = false;
        public bool Focused = false;
        public bool Invalid = false;

        public SKColor Color { get; set; } = new(0, 255, 0, 255);
        public Bounds Bounds { get; private set; }
        public Panel? Parent
        {
            get => _parent;
            set => SetParent(value);
        }

        public void SetParent(Panel? value)
        {
            _parent = value;
        }

        private Panel? _parent;

        public YogaNode _node;

        public string[] ClassNames { get; set; }

        //public Style Style; //TODO: Implement PanelStyle (will hold parsed CSS styles)

        public List<Panel> Children { get; protected set; }


        public YogaValue Width
        {
            get => _width;
            set => _node.Width = value;
        }
        public YogaValue Height
        {
            get => _height;
            set => _node.Height = value;
        }

        private float _width => _node.LayoutWidth;
        private float _height => _node.LayoutHeight;

        public float X => _node.LayoutX;
        public float Y => _node.LayoutY;


        public SKPoint Position { get { return new SKPoint(X, Y); } }

        public Panel()
        {
            Children = new List<Panel>();
            _node = new();
            ClassNames = new string[] {};
        }

        public Panel(params string[] classNames) : this()
        {
            ClassNames = classNames;
        }

        public void AddChild(Panel child)
        {
            Children.Add(child);
            _node.AddChild(child._node);
            child.Parent = this;
        }

        public void InvalidateLayout()
        {
            Invalid = true;
        }

        public void RecalculateLayout()
        {
            _node.CalculateLayout(Parent?.Width.Value ?? 500f , Parent?.Height.Value ?? 500f);
            RecalculateBounds();
            Invalid = false;
        }

        private void RecalculateBounds()
        {
            Bounds = new(new Vector2(X, Y), new Vector2(X + _width, Y + _height));
            Children.ForEach(x => x.RecalculateBounds());
        }

        public static implicit operator YogaNode(Panel p) => p._node;

        public virtual void OnClick()
        {
            Console.WriteLine("Click click!");
        }
        
        public virtual void Update(Vector2 mp)
        {
            if (Invalid)
                RecalculateLayout();
            Children.ForEach(x => x.Update(mp));

            //check if mp is within element bounds
            if (Bounds.IsInside(mp))
            {
                if (!Hovered)
                {
                    Hovered = true;
                    Color = new SKColor(255, 255, 255, 255);
                }
            }
            else
            {
                if (Hovered)
                {
                    Hovered = false;
                    Color = new SKColor(0, 255, 0, 255);
                }
            }
        }
    }
}
