namespace OrderCase.Processing.Application.Abstractions.Payments;

public interface IPaymentGateway
{
    /// <summary>
    /// Ödemeyi gerçekleştirir.
    /// İş kuralı kaynaklı reddedilmeler (yetersiz bakiye, limit aşımı vb.) PaymentResult.Failure olarak döner.
    /// Geçici altyapı hataları (timeout, sağlayıcıya ulaşılamaması vb.) exception olarak fırlatılır.
    /// </summary>
    Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken);
}