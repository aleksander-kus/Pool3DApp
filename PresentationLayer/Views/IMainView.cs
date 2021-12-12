using PresentationLayer.Presenters;
using System.Drawing;

namespace PresentationLayer.Views
{
    public interface IMainView
    {
        MainPresenter Presenter { set; }
        int CanvasWidth { get; }
        int CanvasHeight { get; }
        Image CanvasImage { set; }
        void RedrawCanvas();
    }
}
