using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Drawing;
using Point = System.Windows.Point;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;

namespace _3D_scene
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        double angle = 0.0;
        const int N = 256;
        ModelVisual3D terrain = new ModelVisual3D();
        PerspectiveCamera camera = new PerspectiveCamera();

        public MainWindow()
        {
            InitializeComponent();
            scene.Focusable = true;
            grid.Background = Brushes.DeepSkyBlue;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
            System.Drawing.Bitmap hMap;
            hMap = new System.Drawing.Bitmap(dlg.FileName);

            //размер ландшафта (256х256 пикселей, как у карты высот)
            
            //модель для отображения ландшафта
           
            MeshGeometry3D geometry = new MeshGeometry3D();
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    //расстановка точек ландшафта
                    double y = hMap.GetPixel(i, j).R / 10.0;
                    geometry.Positions.Add(new Point3D(i, y, j));
                    //вычисление текстурных координат для точек ландшафта
                    double tu = i / Convert.ToDouble(N);
                    double tv = j / Convert.ToDouble(N);
                    geometry.TextureCoordinates.Add(new Point(tu, tv));
                }
            for (int i = 0; i < N - 1; i++)
                for (int j = 0; j < N - 1; j++)
                {
                    //вычисление индексов 4х точек, находящихся рядом
                    int ind0 = i + j * N;
                    int ind1 = (i + 1) + j * N;
                    int ind2 = i + (j + 1) * N;
                    int ind3 = (i + 1) + (j + 1) * N;
                    //описание первого треугольника
                    geometry.TriangleIndices.Add(ind0);
                    geometry.TriangleIndices.Add(ind1);
                    geometry.TriangleIndices.Add(ind3);
                    //описание второго треугольника
                    geometry.TriangleIndices.Add(ind0);
                    geometry.TriangleIndices.Add(ind3);
                    geometry.TriangleIndices.Add(ind2);
                }

            ImageBrush ib = new ImageBrush();
            //загрузка изображения и назначение кисти
            ib.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/grasstile.jpg",
            UriKind.Absolute));
            //масштабирование кисти, текстура будет повторена 4 раза по поверхности ландшафта
            ib.Transform = new ScaleTransform(0.5, 0.5);
            ib.TileMode = TileMode.Tile;
            ib.Stretch = Stretch.Fill;
            //создание материала
            DiffuseMaterial mat = new DiffuseMaterial(ib);
            //создание модели
            GeometryModel3D model = new GeometryModel3D(geometry, mat);
            //установка модели в визуальный объект
            terrain.Content = model;
            //добавление визуального объекта в сцену
            scene.Children.Add(terrain);

            
            //расположение камеры
            camera.Position = new Point3D(N , N , N * 1.5);
            //точка, на которую направлена камера (центр ландшафта)            
            camera.Position = new Point3D(N / 2, N / 2, N * 1.5);
            //точка, на которую направлена камера (центр ландшафта)
            Vector3D lookAt = new Vector3D(N / 2, 0, N / 2);
            camera.LookDirection = Vector3D.Subtract(lookAt, new Vector3D(N / 2, N / 2, N * 2));

            camera.FarPlaneDistance = 1000;
            camera.NearPlaneDistance = 1;
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 75;
            scene.Camera = camera;
            PointLight pl = new PointLight();
            pl.Color = Colors.LightYellow;
            pl.Position = new Point3D(N/2, N/2, N/2);
            ModelVisual3D light = new ModelVisual3D();
            light.Content = pl;
            scene.Children.Add(light);
            


        }

        private void scene_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                angle--;
            }
            //если нажата стрелка вправо
            if (e.Key == Key.Right)
            {
                angle++;
            }
            //создание поворота вокруг оси Y на угол angle
            //AxisAngleRotation3D ax3d = new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle);

            //Math.Sin(5);

            camera.Position = new Point3D((N / 2) + (N) * Math.Cos((angle* Math.PI)/180), N/2, (N / 2) + (N) * Math.Sin((angle * Math.PI) / 180));
            //точка, на которую направлена камера (центр ландшафта)
            Vector3D lookAt = new Vector3D((N / 2), 0, (N / 2) );
            
            camera.LookDirection = Vector3D.Subtract(lookAt, new Vector3D((N / 2) + (N) * Math.Cos((angle * Math.PI) / 180), N / 2, (N / 2) + (N) * Math.Sin((angle * Math.PI) / 180)));
            

        }

        
    
    }

    }
