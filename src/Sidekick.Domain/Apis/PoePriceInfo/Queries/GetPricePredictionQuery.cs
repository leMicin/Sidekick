using MediatR;
using Sidekick.Domain.Apis.PoePriceInfo.Models;
using Sidekick.Domain.Game.Items.Models;

namespace Sidekick.Domain.Apis.PoePriceInfo.Queries
{
    /// <summary>
    /// Predict the price of an item
    /// </summary>
    public class GetPricePredictionQuery : IQuery<PricePrediction>
    {
        /// <summary>
        /// Predict the price of an item
        /// </summary>
        /// <param name="item">The item to price predict</param>
        public GetPricePredictionQuery(Item item)
        {
            Item = item;
        }

        /// <summary>
        /// The item to price predict
        /// </summary>
        public Item Item { get; set; }
    }
}
