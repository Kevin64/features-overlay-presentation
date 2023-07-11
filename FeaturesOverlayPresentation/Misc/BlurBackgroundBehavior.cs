using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace FeaturesOverlayPresentation.Misc
{
    /// <summary>
    /// Class for implementing a blur effect on the image
    /// </summary>
    public class BlurBackgroundBehavior : Behavior<Shape>
    {
        public static readonly DependencyProperty BlurContainerProperty
            = DependencyProperty.Register(
                                          "BlurContainer",
                                          typeof(UIElement),
                                          typeof(BlurBackgroundBehavior),
                                          new PropertyMetadata(OnContainerChanged));

        private static readonly DependencyProperty BrushProperty
            = DependencyProperty.Register(
                                          "Brush",
                                          typeof(VisualBrush),
                                          typeof(BlurBackgroundBehavior),
                                          new PropertyMetadata());

        private VisualBrush Brush
        {
            get => (VisualBrush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public UIElement BlurContainer
        {
            get => (UIElement)GetValue(BlurContainerProperty);
            set => SetValue(BlurContainerProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.Effect = new BlurEffect
            {
                Radius = 10,
                KernelType = KernelType.Gaussian,
                RenderingBias = RenderingBias.Quality
            };

            _ = AssociatedObject.SetBinding(Shape.FillProperty,
                                             new Binding
                                             {
                                                 Source = this,
                                                 Path = new PropertyPath(BrushProperty)
                                             });

            AssociatedObject.LayoutUpdated += (sender, args) => UpdateBounds();
            UpdateBounds();
        }

        protected override void OnDetaching()
        {
            BindingOperations.ClearBinding(AssociatedObject, Border.BackgroundProperty);
        }

        private static void OnContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BlurBackgroundBehavior)d).OnContainerChanged((UIElement)e.OldValue, (UIElement)e.NewValue);
        }

        private void OnContainerChanged(UIElement oldValue, UIElement newValue)
        {
            if (oldValue != null)
            {
                oldValue.LayoutUpdated -= OnContainerLayoutUpdated;
            }

            if (newValue != null)
            {
                Brush = new VisualBrush(newValue)
                {
                    ViewboxUnits = BrushMappingMode.Absolute
                };

                newValue.LayoutUpdated += OnContainerLayoutUpdated;
                UpdateBounds();
            }
            else
            {
                Brush = null;
            }
        }

        private void OnContainerLayoutUpdated(object sender, EventArgs eventArgs)
        {
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            if (AssociatedObject != null && BlurContainer != null && Brush != null)
            {
                Point difference = AssociatedObject.TranslatePoint(new Point(), BlurContainer);
                Brush.Viewbox = new Rect(difference, AssociatedObject.RenderSize);
            }
        }
    }
}
