using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SkiaSharp;
using Flexbox;

namespace Eden
{
    public class Panel
    {
        public Panel? Parent { get; protected set; }

        private Node _node;

        private bool _isInvalid;

        public string[] ClassNames { get; set; }
        public Style NodeStyle => _node.nodeStyle;

        //public PanelStyle Style; //TODO: Implement PanelStyle (will hold parsed CSS styles)

        public List<Panel> Children { get; protected set; }

        public float Width => _node.layout.width;
        public float Height => _node.layout.height;

        public float X => _node.layout.x;
        public float Y => _node.layout.y;

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
            child.Parent = this;
        }

        public void InvalidateLayout()
        {

        }

        public void RecalculateLayout()
        {
            _node.CalculateLayout(Parent?.Width ?? 500f , Parent?.Height ?? 500f, Direction.Inherit);
            Children.ForEach(x => x.RecalculateLayout());

        }
    }
}
