using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Commands;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Services;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using CustomerResult = DestilatechPlatform.API.Shared.Application.Patterns.Result<
    DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Aggregates.Customer,
    DestilatechPlatform.API.OrdersReplenishment.Domain.Model.OrdersReplenishmentError>;

namespace DestilatechPlatform.API.OrdersReplenishment.Application.Internal.CommandServices;

/// <summary>
/// Registers and updates the customers of an account (US17).
/// </summary>
public class CustomerCommandService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    : ICustomerCommandService
{
    public async Task<CustomerResult> HandleAsync(RegisterCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        Customer customer;
        try
        {
            customer = new Customer(command);
        }
        catch (OrdersReplenishmentException exception)
        {
            return new CustomerResult.Failure(exception.Error);
        }

        await customerRepository.AddAsync(customer, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new CustomerResult.Success(customer);
    }

    public async Task<CustomerResult> HandleAsync(UpdateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.FindByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null || !customer.BelongsTo(command.UserId))
        {
            return new CustomerResult.Failure(OrdersReplenishmentError.CustomerNotFound(command.CustomerId));
        }

        try
        {
            customer.Update(command);
        }
        catch (OrdersReplenishmentException exception)
        {
            return new CustomerResult.Failure(exception.Error);
        }

        customerRepository.Update(customer);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new CustomerResult.Success(customer);
    }
}
