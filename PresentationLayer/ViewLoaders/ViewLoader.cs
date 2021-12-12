using PresentationLayer.Presenters;
using PresentationLayer.Views;
using System.Windows.Forms;

namespace PresentationLayer.ViewLoaders
{
    public class ViewLoader : IViewLoader
    {
        private Form lastLoadedView;
        public Form LastLoadedForm => lastLoadedView;

        public void LoadMainView()
        {
            MainView view = new();
            MainPresenter presenter = new(view, this);

            view.Presenter = presenter;

            LoadView(view);
        }

        private void LoadView(Form view)
        {
            view.Show();
            lastLoadedView = view;
        }
    }
}
