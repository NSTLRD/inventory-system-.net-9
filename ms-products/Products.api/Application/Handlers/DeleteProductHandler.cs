// Products.api/Application/Handlers/DeleteProductHandler.cs
using MediatR;
using Products.Api.Application.Commands;
using Products.Api.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Products.Api.Domain.Entities;

namespace Products.Api.Application.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProduct, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
            
            if (product != null)
            {
                _unitOfWork.Repository<Product>().DeleteAsync(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
