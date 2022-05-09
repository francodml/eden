using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SkiaSharp;
using Facebook.Yoga;

namespace Eden
{
    public class Panel
    {
        bool Active = false;
        bool Hovered = false;
        bool Focused = false;

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

        //public PanelStyle Style; //TODO: Implement PanelStyle (will hold parsed CSS styles)

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

        }

        public void RecalculateLayout()
        {
            _node.CalculateLayout(Parent?.Width.Value ?? 500f , Parent?.Height.Value ?? 500f);
        }

        public static implicit operator YogaNode(Panel p) => p._node;
    }
}
