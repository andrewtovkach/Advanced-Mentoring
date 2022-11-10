using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Common.Constants;
using CatalogService.Application.Common.Correlation;
using CatalogService.Application.Common.Events;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Domain.Entities;
using CatalogService.MessageBroker.Producer;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Items.Commands.UpdateItem
{
    public class UpdateItemCommand : IRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public double Price { get; set; }

        public int Amount { get; set; }
    }

    public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IKafkaProducer<string, ItemChanged> _kafkaProducer;
        private readonly ICorrelationIdInitializer _correlationIdInitializer;
        private readonly ILogger<UpdateItemCommandHandler> _logger;

        public UpdateItemCommandHandler(IApplicationDbContext context,
            IKafkaProducer<string, ItemChanged> kafkaProducer,
            ICorrelationIdInitializer correlationIdInitializer,
            ILogger<UpdateItemCommandHandler> logger)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
            _correlationIdInitializer = correlationIdInitializer;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Items.FindAsync(request.Id);

            if (entity == null)
            {
                _logger.LogInformation($"The item with id {request.Id} was not found.");

                throw new NotFoundException(nameof(Item), request.Id);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Amount = request.Amount;
            entity.Image = request.Image;
            entity.Price = request.Price;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Raising ItemChanged event for the item with id {request.Id}.");

            await _kafkaProducer.ProduceAsync(KafkaTopics.ItemChanged, null, new ItemChanged
            {
                CorellationId = _correlationIdInitializer.CorrelationId,
                Id = request.Id,
                Name = request.Name,
                Price = request.Price
            });

            _logger.LogInformation($"ItemChanged event for the item with id {request.Id} was raised.");

            return Unit.Value;
        }
    }
}
