using PresentationLayer.Presenters;
using System.Drawing;
using System.Windows.Forms;

namespace PresentationLayer.Views
{
    public partial class MainView : Form, IMainView
    {
        private MainPresenter presenter;
        private readonly Timer timer = new();
        public MainView()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
            timer.Interval = 50;
        }

        private void Timer_Tick(object sender, System.EventArgs e) => presenter.Update();

        public MainPresenter Presenter { set => presenter = value; }

        public int CanvasWidth => canvasBox.Width;

        public int CanvasHeight => canvasBox.Height;

        public Image CanvasImage { set => canvasBox.Image = value; }

        public void RedrawCanvas() => canvasBox.Invalidate();



        private void MainView_Load(object sender, System.EventArgs e)
        {
            fovBar_ValueChanged(null, null);
            kdTrackBar_ValueChanged(null, null);
            ksTrackBar_ValueChanged(null, null);
            kaTrackbar_ValueChanged(null, null);
            hTrackbar_ValueChanged(null, null);
            yTrackbar_ValueChanged(null, null);
            xTrackbar_ValueChanged(null, null);
            timer.Start();
        }

        private void MainView_Resize(object sender, System.EventArgs e)
        {
            presenter.LoadCanvasDimensions();
        }


        private void fovBar_ValueChanged(object sender, System.EventArgs e)
        {
            presenter.Fov = fovBar.Value;
            fovLabel.Text = $"Fov: {fovBar.Value}";
        }
        private void kdTrackBar_ValueChanged(object sender, System.EventArgs e)
        {
            presenter.Kd = kdTrackBar.Value;
            kdLabel.Text = $"Kd: {kdTrackBar.Value/100f}";
        }

        private void ksTrackBar_ValueChanged(object sender, System.EventArgs e)
        {
            presenter.Ks = ksTrackBar.Value;
            ksLabel.Text = $"Ks: {ksTrackBar.Value/100f}";
        }

        private void kaTrackbar_ValueChanged(object sender, System.EventArgs e)
        {
            presenter.Ka = kaTrackbar.Value;
            kaLabel.Text = $"Ka: {kaTrackbar.Value / 100f}";
        }
        private void hTrackbar_ValueChanged(object sender, System.EventArgs e)
        {
            presenter.MainLigthZ = hTrackbar.Value;
            hLabel.Text = $"z: {hTrackbar.Value / 10f}";
        }

        private void yTrackbar_ValueChanged(object sender, System.EventArgs e)
        {
            presenter.MainLigthY = yTrackbar.Value;
            yLabel.Text = $"y: {yTrackbar.Value / 10f}";
        }
        private void xTrackbar_ValueChanged(object sender, System.EventArgs e)
        {
            presenter.MainLigthX = xTrackbar.Value;
            xLabel.Text = $"x: {xTrackbar.Value / 10f}";
        }

        private void gouraudButton_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.ShadingMode = DomainLayer.Enum.ShadingMode.Gouraud;
        }

        private void constantButton_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.ShadingMode = DomainLayer.Enum.ShadingMode.Constant;
        }

        private void phongButton_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.ShadingMode = DomainLayer.Enum.ShadingMode.Phong;
        }

        private void staticCameraButton_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.SwitchCamera(0);
        }

        private void followingCameraButton_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.SwitchCamera(1);
        }

        private void topCameraButton_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.SwitchCamera(2);
        }

        private void cubeMovementBox_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.CubeMovement = cubeMovementBox.Checked;
        }

        private void cubeRotationBox_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.CubeRotiation = cubeRotationBox.Checked;
        }

        private void mainLightButton_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.MainLight = mainLightButton.Checked;
        }

        private void reflectorButton_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.Reflector = reflectorButton.Checked;
        }

        private void fogBox_CheckedChanged(object sender, System.EventArgs e)
        {
            presenter.Fog = fogBox.Checked;
        }
    }
}
