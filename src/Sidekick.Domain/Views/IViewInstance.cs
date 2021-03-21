using System.Threading.Tasks;

namespace Sidekick.Domain.Views
{
    /// <summary>
    /// Interface to manage a view instance
    /// </summary>
    public interface IViewInstance
    {
        /// <summary>
        /// Minimizes the view
        /// </summary>
        Task Minimize();

        /// <summary>
        /// Maximizes the view
        /// </summary>
        Task Maximize();

        /// <summary>
        /// Closes the view
        /// </summary>
        Task Close();
    }
}
