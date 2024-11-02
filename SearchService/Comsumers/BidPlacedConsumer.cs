using Contracts;
using MassTransit;

namespace SearchService.Comsumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        public Task Consume(ConsumeContext<BidPlaced> context)
        {
            throw new NotImplementedException();
        }
    }
}
