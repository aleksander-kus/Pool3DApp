using InfrastructureLayer;
using InfrastructureLayer.Services;
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
        private readonly IDrawingService drawingService;
        public MainPresenter(IMainView view, IViewLoader viewLoader)
        {
            this.view = view;
            this.viewLoader = viewLoader;
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            ByteBitmap byteBitmap = new(bitmap);
            drawingService = new DrawingService();
            drawingService.DrawLineBresenham(byteBitmap, Color.Black, new Point(100, 100), new Point(200, 200));
            this.view.CanvasImage = bitmap = byteBitmap.Bitmap;
            this.view.RedrawCanvas();
        }
    }
}
