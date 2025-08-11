using Microsoft.Maui.Graphics;
using ChronoVoid2500.Mobile.Models;
using System.Collections.ObjectModel;

namespace ChronoVoid2500.Mobile.Controls;

public class GalaxyMapView : GraphicsView, IDrawable
{
    private readonly List<StarNode> _starNodes = new();
    private readonly List<HyperTunnel> _hyperTunnels = new();
    private StarNode? _selectedNode;
    private float _scale = 1.0f;
    private PointF _offset = new(0, 0);
    private PointF _lastPanPoint;
    private bool _isPanning = false;

    // Bindable Properties
    public static readonly BindableProperty AvailableNodesProperty = 
        BindableProperty.Create(nameof(AvailableNodes), typeof(ObservableCollection<ConnectedNodeDto>), typeof(GalaxyMapView), 
            propertyChanged: OnAvailableNodesChanged);

    public static readonly BindableProperty CurrentNodeProperty = 
        BindableProperty.Create(nameof(CurrentNode), typeof(ConnectedNodeDto), typeof(GalaxyMapView), 
            propertyChanged: OnCurrentNodeChanged);

    public ObservableCollection<ConnectedNodeDto>? AvailableNodes
    {
        get => (ObservableCollection<ConnectedNodeDto>?)GetValue(AvailableNodesProperty);
        set => SetValue(AvailableNodesProperty, value);
    }

    public ConnectedNodeDto? CurrentNode
    {
        get => (ConnectedNodeDto?)GetValue(CurrentNodeProperty);
        set => SetValue(CurrentNodeProperty, value);
    }

    // Events
    public event EventHandler<ConnectedNodeDto>? NodeSelected;

    public GalaxyMapView()
    {
        Drawable = this;
        BackgroundColor = Colors.Black;
        
        // Add gesture recognizers
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnTapped;
        GestureRecognizers.Add(tapGesture);

        var pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += OnPinchUpdated;
        GestureRecognizers.Add(pinchGesture);
    }

    private static void OnAvailableNodesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GalaxyMapView mapView && newValue is ObservableCollection<ConnectedNodeDto> nodes)
        {
            mapView.UpdateNodes(nodes);
        }
    }

    private static void OnCurrentNodeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GalaxyMapView mapView && newValue is ConnectedNodeDto currentNode)
        {
            mapView.UpdateCurrentNode(currentNode);
        }
    }

    private void UpdateNodes(ObservableCollection<ConnectedNodeDto> nodes)
    {
        _starNodes.Clear();
        _hyperTunnels.Clear();

        // Add current node if available
        if (CurrentNode != null)
        {
            var currentStarNode = new StarNode
            {
                Id = CurrentNode.NodeId,
                NodeNumber = CurrentNode.NodeNumber,
                Position = new PointF(400, 300), // Center position
                StarName = CurrentNode.StarName,
                PlanetCount = CurrentNode.PlanetCount,
                HasQuantumStation = CurrentNode.HasQuantumStation,
                IsCurrent = true
            };
            _starNodes.Add(currentStarNode);
        }

        // Add connected nodes in a circular pattern around current node
        var angleStep = 360f / Math.Max(nodes.Count, 1);
        var radius = 150f;

        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var angle = i * angleStep * (Math.PI / 180);
            var x = 400 + (float)(Math.Cos(angle) * radius);
            var y = 300 + (float)(Math.Sin(angle) * radius);

            var starNode = new StarNode
            {
                Id = node.NodeId,
                NodeNumber = node.NodeNumber,
                Position = new PointF(x, y),
                StarName = node.StarName,
                PlanetCount = node.PlanetCount,
                HasQuantumStation = node.HasQuantumStation,
                IsCurrent = false
            };
            _starNodes.Add(starNode);

            // Create hyper tunnel connection to current node
            if (CurrentNode != null)
            {
                _hyperTunnels.Add(new HyperTunnel
                {
                    From = new PointF(400, 300),
                    To = new PointF(x, y)
                });
            }
        }

        Invalidate();
    }

    private void UpdateCurrentNode(ConnectedNodeDto currentNode)
    {
        foreach (var node in _starNodes)
        {
            node.IsCurrent = node.Id == currentNode.NodeId;
        }
        Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Clear background with star field
        DrawStarField(canvas, dirtyRect);

        // Apply transformations
        canvas.SaveState();
        canvas.Translate(_offset.X, _offset.Y);
        canvas.Scale(_scale, _scale);

        // Draw hyper tunnels first (behind nodes)
        DrawHyperTunnels(canvas);

        // Draw star nodes
        DrawStarNodes(canvas);

        canvas.RestoreState();
    }

    private void DrawStarField(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(dirtyRect);

        // Draw animated twinkling stars
        var random = new Random(42); // Fixed seed for consistent star positions
        canvas.FillColor = Colors.White;

        for (int i = 0; i < 100; i++)
        {
            var x = random.NextSingle() * dirtyRect.Width;
            var y = random.NextSingle() * dirtyRect.Height;
            var size = random.NextSingle() * 2 + 0.5f;
            
            // Add some twinkling effect
            var alpha = 0.3f + (float)(Math.Sin(DateTime.Now.Millisecond * 0.01 + i) * 0.3 + 0.3);
            canvas.Alpha = alpha;
            canvas.FillCircle(x, y, size);
        }
        canvas.Alpha = 1.0f;
    }

    private void DrawHyperTunnels(ICanvas canvas)
    {
        foreach (var tunnel in _hyperTunnels)
        {
            // Draw animated energy flow
            canvas.StrokeColor = Color.FromArgb("#00ff88");
            canvas.StrokeSize = 2;
            canvas.StrokeDashPattern = new float[] { 10, 5 };
            
            // Add pulsing effect
            var pulseAlpha = 0.5f + (float)(Math.Sin(DateTime.Now.Millisecond * 0.005) * 0.3);
            canvas.Alpha = pulseAlpha;
            
            canvas.DrawLine(tunnel.From, tunnel.To);
            
            // Draw energy particles
            var direction = new PointF(tunnel.To.X - tunnel.From.X, tunnel.To.Y - tunnel.From.Y);
            var length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            direction = new PointF(direction.X / length, direction.Y / length);
            
            var particlePos = (DateTime.Now.Millisecond * 0.001f) % 1.0f;
            var particleX = tunnel.From.X + direction.X * length * particlePos;
            var particleY = tunnel.From.Y + direction.Y * length * particlePos;
            
            canvas.FillColor = Color.FromArgb("#00ff88");
            canvas.FillCircle(particleX, particleY, 3);
        }
        
        canvas.Alpha = 1.0f;
        canvas.StrokeDashPattern = null;
    }

    private void DrawStarNodes(ICanvas canvas)
    {
        foreach (var node in _starNodes)
        {
            var position = node.Position;
            var radius = node.IsCurrent ? 25f : 20f;

            // Node background circle
            if (node.IsCurrent)
            {
                // Pulsing effect for current node
                var pulseRadius = radius + (float)(Math.Sin(DateTime.Now.Millisecond * 0.01) * 3);
                canvas.FillColor = Color.FromArgb("#0066ff");
                canvas.Alpha = 0.3f;
                canvas.FillCircle(position.X, position.Y, pulseRadius);
                canvas.Alpha = 1.0f;
            }

            // Main node circle
            canvas.FillColor = GetNodeColor(node);
            canvas.FillCircle(position.X, position.Y, radius);

            // Node border
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = node.IsCurrent ? 3 : 2;
            canvas.DrawCircle(position.X, position.Y, radius);

            // Quantum station indicator
            if (node.HasQuantumStation)
            {
                canvas.FillColor = Colors.Yellow;
                canvas.FillCircle(position.X + radius - 5, position.Y - radius + 5, 4);
            }

            // Node number
            canvas.FontColor = Colors.White;
            canvas.FontSize = 12;
            var nodeText = node.NodeNumber.ToString();
            var textSize = canvas.GetStringSize(nodeText, Microsoft.Maui.Graphics.Font.Default, 12);
            canvas.DrawString(nodeText, position.X - textSize.Width / 2, position.Y + textSize.Height / 2, 
                HorizontalAlignment.Center);

            // Star name (if selected or current)
            if (node.IsCurrent || node == _selectedNode)
            {
                canvas.FontSize = 10;
                canvas.FontColor = Color.FromArgb("#00ff88");
                var nameSize = canvas.GetStringSize(node.StarName, Microsoft.Maui.Graphics.Font.Default, 10);
                canvas.DrawString(node.StarName, position.X - nameSize.Width / 2, position.Y + radius + 15,
                    HorizontalAlignment.Center);

                // Planet count
                var planetText = $"{node.PlanetCount} Planets";
                var planetSize = canvas.GetStringSize(planetText, Microsoft.Maui.Graphics.Font.Default, 8);
                canvas.FontSize = 8;
                canvas.FontColor = Colors.Gray;
                canvas.DrawString(planetText, position.X - planetSize.Width / 2, position.Y + radius + 28,
                    HorizontalAlignment.Center);
            }
        }
    }

    private Color GetNodeColor(StarNode node)
    {
        if (node.IsCurrent)
            return Color.FromArgb("#0066ff"); // Blue for current
        if (node.HasQuantumStation)
            return Color.FromArgb("#00ff88"); // Green for quantum station
        return Color.FromArgb("#666666"); // Gray for normal nodes
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (e.GetPosition(this) is Point tapPoint)
        {
            var adjustedPoint = new PointF(
                (float)(tapPoint.X - _offset.X) / _scale,
                (float)(tapPoint.Y - _offset.Y) / _scale
            );

            var tappedNode = GetNodeAtPoint(adjustedPoint);
            if (tappedNode != null && !tappedNode.IsCurrent)
            {
                _selectedNode = tappedNode;
                
                // Find the corresponding ConnectedNodeDto
                var connectedNode = AvailableNodes?.FirstOrDefault(n => n.NodeId == tappedNode.Id);
                if (connectedNode != null)
                {
                    NodeSelected?.Invoke(this, connectedNode);
                }
                
                Invalidate();
            }
        }
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _isPanning = true;
                _lastPanPoint = new PointF((float)e.TotalX, (float)e.TotalY);
                break;

            case GestureStatus.Running:
                if (_isPanning)
                {
                    var deltaX = (float)e.TotalX - _lastPanPoint.X;
                    var deltaY = (float)e.TotalY - _lastPanPoint.Y;
                    
                    _offset = new PointF(_offset.X + deltaX, _offset.Y + deltaY);
                    _lastPanPoint = new PointF((float)e.TotalX, (float)e.TotalY);
                    
                    Invalidate();
                }
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _isPanning = false;
                break;
        }
    }

    private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Running:
                _scale = Math.Max(0.5f, Math.Min(3.0f, _scale * (float)e.Scale));
                Invalidate();
                break;
        }
    }

    private StarNode? GetNodeAtPoint(PointF point)
    {
        foreach (var node in _starNodes)
        {
            var distance = (float)Math.Sqrt(
                Math.Pow(point.X - node.Position.X, 2) + 
                Math.Pow(point.Y - node.Position.Y, 2)
            );
            
            var radius = node.IsCurrent ? 25f : 20f;
            if (distance <= radius)
                return node;
        }
        return null;
    }
}

// Helper classes
public class StarNode
{
    public int Id { get; set; }
    public int NodeNumber { get; set; }
    public PointF Position { get; set; }
    public string StarName { get; set; } = string.Empty;
    public int PlanetCount { get; set; }
    public bool HasQuantumStation { get; set; }
    public bool IsCurrent { get; set; }
}

public class HyperTunnel
{
    public PointF From { get; set; }
    public PointF To { get; set; }
}