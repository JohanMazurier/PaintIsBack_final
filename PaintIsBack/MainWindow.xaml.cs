using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintIsBack
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<UIElement> HistoryElement { get; set; } = new List<UIElement>();
        public List<List<UIElement>> History { get; set; } = new List<List<UIElement>>();
        public List<List<UIElement>> UndoList { get; set; } = new List<List<UIElement>>();

        public int maxdo = 20;

        public bool? fill = false;

        public Line _line = new Line();

        public Rectangle _rectangle = new Rectangle();

        public Ellipse _ellipse = new Ellipse();

        public Polygon _triangle = new Polygon();
        public int Thickness { get; set; } = 1;
        public System.Windows.Media.Brush Color { get; set; } = Brushes.Black;
        public Point currentPosition;

        public bool line = true;

        public bool rectangle = false;

        public bool ellipse = false;

        public bool triangle = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UndoList.Count > 0)
            {
                UndoList.Clear();
                RedoButton.IsEnabled = false;
            }

            if (line)
            {
                _line = new Line();
                currentPosition = e.GetPosition(Canvas);
                currentPosition.Y += 80;
            }

            if (rectangle)
            {
                _rectangle = new Rectangle();
                currentPosition = e.GetPosition(Canvas);
                Canvas.SetLeft(_rectangle, currentPosition.X);
                Canvas.SetTop(_rectangle, currentPosition.Y);
                Canvas.Children.Add(_rectangle);
            }

            if (ellipse)
            {
                _ellipse = new Ellipse();
                currentPosition = e.GetPosition(Canvas);
                Canvas.SetLeft(_ellipse, currentPosition.X);
                Canvas.SetTop(_ellipse, currentPosition.Y);
                Canvas.Children.Add(_ellipse);
            }

            if (triangle)
            {
                _triangle = new Polygon();
                currentPosition = e.GetPosition(Canvas);
                Canvas.Children.Add(_triangle);

                _triangle.Points.Add(new Point(0, 0));
                _triangle.Points.Add(new Point(0, 0));
                _triangle.Points.Add(new Point(0, 0));


            }

        }
 

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(line && e.LeftButton == MouseButtonState.Pressed)
            {
                _line = new Line();
                _line.Stroke = Color;
                _line.StrokeStartLineCap = PenLineCap.Round;
                _line.StrokeEndLineCap = PenLineCap.Round;
                _line.StrokeThickness = Thickness;
                _line.X1 = currentPosition.X;
                _line.Y1 = currentPosition.Y-80;
                _line.X2 = e.GetPosition(this).X;
                _line.Y2 = e.GetPosition(this).Y-80;
                currentPosition = e.GetPosition(this);
                Canvas.Children.Add(_line);
                HistoryElement.Add(_line);
            }
            if (rectangle && e.LeftButton == MouseButtonState.Pressed)
            {
                _rectangle.Stroke = Color;
                _rectangle.StrokeThickness = Thickness;
                if((bool) fill)
                {
                    _rectangle.Fill = Color;
                }
                Canvas.SetLeft(_rectangle, currentPosition.X);
                Canvas.SetTop(_rectangle, currentPosition.Y);
                var pos = e.GetPosition(Canvas);

                var x = Math.Min(pos.X, currentPosition.X);
                var y = Math.Min(pos.Y, currentPosition.Y);

                var w = Math.Max(pos.X, currentPosition.X) - x;
                var h = Math.Max(pos.Y, currentPosition.Y) - y;

                _rectangle.Width = w;
                _rectangle.Height = h;

                Canvas.SetLeft(_rectangle, x);
                Canvas.SetTop(_rectangle, y);

            }
            if (ellipse && e.LeftButton == MouseButtonState.Pressed)
            {
                _ellipse.Stroke = Color;
                _ellipse.StrokeThickness = Thickness;
                if ((bool)fill)
                {
                    _ellipse.Fill = Color;
                }
                Canvas.SetLeft(_ellipse, currentPosition.X);
                Canvas.SetTop(_ellipse, currentPosition.Y);
                var pos = e.GetPosition(Canvas);

                var x = Math.Min(pos.X, currentPosition.X);
                var y = Math.Min(pos.Y, currentPosition.Y);

                var w = Math.Max(pos.X, currentPosition.X) - x;
                var h = Math.Max(pos.Y, currentPosition.Y) - y;

                _ellipse.Width = w;
                _ellipse.Height = h;

                Canvas.SetLeft(_ellipse, x);
                Canvas.SetTop(_ellipse, y);

            }

            if (triangle && e.LeftButton == MouseButtonState.Pressed)
            {
                _triangle.Stroke = Color;
                _triangle.StrokeThickness = Thickness;
                if ((bool)fill)
                {
                    _triangle.Fill = Color;
                }
                Canvas.SetLeft(_triangle, currentPosition.X);
                Canvas.SetTop(_triangle, currentPosition.Y);
                var pos = e.GetPosition(Canvas);

                Point p1 = new Point(0, pos.Y - currentPosition.Y);
                Point p2 = new Point(pos.X - currentPosition.X, pos.Y - currentPosition.Y);
                Point p3 = new Point((pos.X - currentPosition.X) / 2, 0);
                _triangle.Points[0] = p1;
                _triangle.Points[1] = p2;
                _triangle.Points[2] = p3;

                Canvas.SetLeft(_triangle, currentPosition.X);
                Canvas.SetTop(_triangle, currentPosition.Y);

            }
        }

        private void ThicknessSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int thickness = ((System.Windows.Controls.ComboBox)e.Source).SelectedIndex +1;
            Thickness = thickness;
        }
        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            History.Clear();
            Canvas.Children.Clear();
        }
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (History.Count > 0 && UndoList.Count < 20)
            {
                RedoButton.IsEnabled = true;
                List<UIElement> copy = new List<UIElement>();
                copy.AddRange(History[History.Count - 1]);
                UndoList.Add(copy);
                History.RemoveAt(History.Count - 1);
                Canvas.Children.Clear();
                for (int i = 0; i < History.Count; i++)
                {
                    for (int j = 0; j < History[i].Count; j++)
                    {
                        Canvas.Children.Add(History[i][j]);
                    }
                }
            }
            if (History.Count <= 0 || UndoList.Count >= 20)
            {
                UndoButton.IsEnabled = false;
            }

        }
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (UndoList.Count > 0)
            {
                UndoButton.IsEnabled = true;
                List<UIElement> copy = new List<UIElement>();
                copy.AddRange(UndoList[UndoList.Count - 1]);
                History.Add(copy);
                UndoList.RemoveAt(UndoList.Count - 1);
                Canvas.Children.Clear();
                for (int i = 0; i < History.Count; i++)
                {
                    for (int j = 0; j < History[i].Count; j++)
                    {
                         if(!Canvas.Children.Contains(History[i][j]))
                        {
                            Canvas.Children.Add(History[i][j]);

                        }
                        
                        
                    }
                }
            }
            if (UndoList.Count <= 0)
            {
                RedoButton.IsEnabled = false;
            }

        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (line && e.LeftButton == MouseButtonState.Released)
            {

                currentPosition = new Point(0, 0);
                List<UIElement> copy = new List<UIElement>();
                copy.AddRange(HistoryElement);
                History.Add(copy);
                HistoryElement.Clear();
                UndoButton.IsEnabled = (UndoList.Count <= 0);
            }

            if (rectangle && e.LeftButton == MouseButtonState.Released)
            {
                HistoryElement.Add(_rectangle);
                _rectangle = new Rectangle();
                List<UIElement> copy = new List<UIElement>();
                copy.AddRange(HistoryElement);
                History.Add(copy);
                HistoryElement.Clear();
                UndoButton.IsEnabled = (UndoList.Count <= 0);
            }

            if (ellipse && e.LeftButton == MouseButtonState.Released)
            {

                HistoryElement.Add(_ellipse);
                _ellipse = new Ellipse();
                List<UIElement> copy = new List<UIElement>();
                copy.AddRange(HistoryElement);
                History.Add(copy);
                HistoryElement.Clear();
                UndoButton.IsEnabled = (UndoList.Count <= 0);
            }

            if (triangle && e.LeftButton == MouseButtonState.Released)
            {

                HistoryElement.Add(_triangle);
                _triangle = new Polygon();
                List<UIElement> copy = new List<UIElement>();
                copy.AddRange(HistoryElement);
                History.Add(copy);
                HistoryElement.Clear();
                UndoButton.IsEnabled = (UndoList.Count <= 0);
            }
        }





        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(Canvas);
            double dpi = 96d;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);
            rtb.Render(Canvas);
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "jpeg (*.jpeg)|*.jpeg|png (*.png)|*.png";
            if(sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    pngEncoder.Save(ms);
                    ms.Close();
                    File.WriteAllBytes(sfd.FileName, ms.ToArray());
                }
                catch (Exception err)
                {
                    System.Windows.MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }
        private void Canvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas_MouseLeftButtonUp(sender, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
            }
        }

        private void Canvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas_MouseLeftButtonDown(sender, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
            }
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas_MouseLeftButtonDown(sender, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
            }
        }

        private void Line_Click(object sender, RoutedEventArgs e)
        {
            line = true;
            rectangle = false;
            ellipse = false;
            triangle = false;
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            line = false;
            rectangle = true;
            ellipse = false;
            triangle = false;
        }

        private void Ellipse_Click(object sender, RoutedEventArgs e)
        {
            line = false;
            rectangle = false;
            ellipse = true;
            triangle = false;
        }

        private void Triangle_Click(object sender, RoutedEventArgs e)
        {
            line = false;
            rectangle = false;
            ellipse = false;
            triangle = true;
        }


        private void NewButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void colorBlack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Black;

            chosenColor.Fill = Brushes.Black;
        }

        private void colorWhite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.White;

            chosenColor.Fill = Brushes.White;
        }

        private void colorGray_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Gray;

            chosenColor.Fill = Brushes.Gray;
        }

        private void colorRed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Red;

            chosenColor.Fill = Brushes.Red;
        }

        private void colorGreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Green;

            chosenColor.Fill = Brushes.Green;
        }

        private void colorYellow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Yellow;

            chosenColor.Fill = Brushes.Yellow;
        }

        private void colorBrown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Brown;

            chosenColor.Fill = Brushes.Brown;
        }

        private void colorBlue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Blue;

            chosenColor.Fill = Brushes.Blue;
        }

        private void colorPurple_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Purple;

            chosenColor.Fill = Brushes.Purple;
        }

        private void colorPink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Pink;

            chosenColor.Fill = Brushes.Pink;
        }

        private void colorGold_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Gold;

            chosenColor.Fill = Brushes.Gold;
        }

        private void colorLightBlue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.LightBlue;

            chosenColor.Fill = Brushes.LightBlue;
        }

        private void colorLightGreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.LightGreen;

            chosenColor.Fill = Brushes.LightGreen;
        }

        private void colorCoral_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Coral;

            chosenColor.Fill = Brushes.Coral;
        }

        private void colorBeige_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.Beige;

            chosenColor.Fill = Brushes.Beige;
        }

        private void colorSlateGray_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Color = Brushes.SlateGray;

            chosenColor.Fill = Brushes.SlateGray;
        }

        private void fillForm_Checked(object sender, RoutedEventArgs e)
        {
            fill = fillForm.IsChecked;
        }
    }
}
