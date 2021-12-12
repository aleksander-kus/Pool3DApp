using PresentationLayer.ViewLoaders;
using PresentationLayer.Views;
using System.Drawing;

namespace PresentationLayer.Presenters
{
    public class MainPresenter
    {
        private Bitmap bitmap;
        private readonly IMainView view;
        private readonly IViewLoader viewLoader;
        public MainPresenter(IMainView view, IViewLoader viewLoader)
        {
            this.view = view;
            this.viewLoader = viewLoader;
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            this.view.CanvasImage = bitmap;
            this.view.RedrawCanvas();
        }
    }
}
