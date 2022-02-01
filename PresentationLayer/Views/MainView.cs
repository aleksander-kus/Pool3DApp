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

        private void MainView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    presenter.MoveCube(x: -0.02f);
                    break;
                case Keys.D:
                    presenter.MoveCube(y: 0.02f);
                    break;
                case Keys.S:
                    presenter.MoveCube(x: 0.02f);
                    break;
                case Keys.A:
                    presenter.MoveCube(y: -0.02f);
                    break;
                case Keys.E:
                    presenter.MoveCube(angle: -5);
                    break;
                case Keys.Q:
                    presenter.MoveCube(angle: 5);
                    break;
                case Keys.C:
                    presenter.SwitchCamera();
                    break;
                default:
                    return;
            }
            presenter.Update();
        }

        private void MainView_KeyPress(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show($"Pressed key {e.KeyChar}");
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
    }
}
