using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Xml.Linq;
public abstract class Element
{
    private static readonly List<string> AcceptableElementTypes = new List<string> {"shape", "text", "tflText", "instance", "shapeObj"};
    public static class DefaultValues
    {
        public const double Width = double.NaN;
        public const double Height = double.NaN;
        public const bool Selected = false;

    }
    private void SetOrRemoveAttribute<T>(in string attributeName, T value, T defaultValue)
    {
        if (EqualityComparer<T>.Default.Equals(value, defaultValue))
        {
            root?.Attribute(attributeName)?.Remove();
        }
        else
        {
            root?.SetAttributeValue(attributeName, value);
        }
    }
    protected XElement? root;
    protected XNamespace ns;
    protected string elementType;
    protected double width, height;
    protected bool selected;
    protected Matrix matrix;
    protected Point transformationPoint;
    public XElement? Root { get { return root; } }
    public string? ElementType { get { return elementType; } }
    public virtual double Width { get { return width; } set { width = value; root?.SetAttributeValue("width", value); } }
    public virtual double Height { get { return height; } set { height = value; root?.SetAttributeValue("height", value); } }
    public bool Selected { get { return selected; } set { selected = value; SetOrRemoveAttribute("isSelected", value, DefaultValues.Selected); } }
    public Matrix Matrix { get { return matrix; } }
    public Point TransformationPoint { get { return transformationPoint; } }
    public Element()
    {
        root = null;
        ns = string.Empty;
        elementType = string.Empty;
        width = double.NaN;
        height = double.NaN;
        selected = false;
        matrix = new Matrix();
        transformationPoint = new Point();
    }
    public Element(in XElement elementNode, string elementType)
    {
        if (!AcceptableElementTypes.Contains(elementType))
        {
            throw new ArgumentException("Invalid element type: " + elementType);
        }
        root = elementNode;
        ns = root.Name.Namespace;
        this.elementType = elementType;
        width = double.NaN;
        height = double.NaN;
        selected = (bool?)elementNode.Attribute("isSelected") ?? DefaultValues.Selected;
        matrix = elementNode.Element(ns + "matrix")?.Element(ns + "Matrix") is not null ? new Matrix(elementNode.Element(ns + "matrix")!.Element(ns + "Matrix"), root) : new Matrix();
        transformationPoint = elementNode.Element(ns + "transformationPoint")?.Element(ns + "Point") is not null ? new Point(elementNode.Element(ns + "transformationPoint")!.Element(ns + "Point")!) : new Point();
    }
    public Element(in Element other)
    {
        root = other.Root is null ? null : new XElement(other.Root);
        ns = other.ns;
        elementType = other.elementType;
        width = other.width;
        height = other.height;
        selected = other.selected;
        matrix = new Matrix(root?.Element(ns + "matrix")!.Element(ns + "Matrix")!, root);
        transformationPoint = new Point(root?.Element(ns + "transformationPoint")!.Element(ns + "Point")!);
    }
}