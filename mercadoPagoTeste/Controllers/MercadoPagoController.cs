using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Client;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;
using mercadoPagoTeste.DTOs;
using MercadoPago.Client.Preference;
using mercadoPagoTeste.Helpers;
using Microsoft.OpenApi.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace mercadoPagoTeste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MercadoPagoController : ControllerBase
    {

        // POST api/<MercadoPagoController>
        [HttpPost("/api/ProcessarPagamentoCartao")]
        public async Task<IActionResult> ProcessarPagamentoCartao([FromBody] SolicitacaoPagamentoCartao solicitacaopagamentoCartao)
        {
            try
            {
                MercadoPagoConfig.AccessToken = "TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172";

                var requestOptions = new RequestOptions();
                requestOptions.CustomHeaders.Add("x-idempotency-key", Guid.NewGuid().ToString());

                var paymentRequest = new PaymentCreateRequest
                {
                    TransactionAmount = solicitacaopagamentoCartao.TransactionAmount,
                    Token = solicitacaopagamentoCartao.Token,
                    Description = solicitacaopagamentoCartao.Description,
                    Installments = solicitacaopagamentoCartao.Installments,
                    PaymentMethodId = solicitacaopagamentoCartao.PaymentMethodId,
                    Payer = new PaymentPayerRequest
                    {
                        Email = solicitacaopagamentoCartao.CardholderEmail,
                        Identification = new IdentificationRequest
                        {
                            Type = solicitacaopagamentoCartao.IdentificationType,
                            Number = solicitacaopagamentoCartao.IdentificationNumber,
                        },
                        FirstName = solicitacaopagamentoCartao.CardholderName
                    },
                };

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(paymentRequest, requestOptions);

                return Ok(payment);
            }
            catch (Exception ex)
            {
                 return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("/api/ProcessarPagamentoPix")]
        public async Task<IActionResult> PagamentoPix([FromBody] SolicitacaoPix solicitacaoPix)
        {
            try
            {
                MercadoPagoConfig.AccessToken = "TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172";

                var requestOptions = new RequestOptions();
                requestOptions.AccessToken = MercadoPagoConfig.AccessToken;
                requestOptions.CustomHeaders.Add("x-idempotency-key", Guid.NewGuid().ToString());

                var request = new PaymentCreateRequest
                {
                    TransactionAmount = 10,
                    PaymentMethodId = "pix",
                    Payer = new PaymentPayerRequest
                    {
                        Email = solicitacaoPix.Email,
                    },
                };

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(request, requestOptions);
                return Ok(payment);
            }
            catch (MercadoPago.Error.MercadoPagoApiException ex)
            {
                var apiErrorMessage = ex.ApiError?.Message ?? "Mensagem da API indisponível";
                var apiCause = ex.ApiError?.Cause != null ? string.Join(", ", ex.ApiError.Cause) : "Causas desconhecidas";

                
                var apiErrorCause = ex.ApiError?.Cause != null
                                    ? string.Join(", ", ex.ApiError.Cause.Select(c => c.Description))
                                    : "Causas desconhecidas";

                // Obter detalhes da resposta
                var apiResponseContent = ex.ApiResponse?.ToString() ?? "Resposta indisponível";
                var apiResponseStatusCode = ex.ApiResponse?.StatusCode.ToString() ?? "Código de status desconhecido";


                Console.WriteLine($"Erro da API do Mercado Pago:");
                Console.WriteLine($"Mensagem: {ex.Message}");
                Console.WriteLine($"Mensagem da API: {apiErrorMessage}");
                Console.WriteLine($"Causas: {apiCause}");

                return StatusCode(500, $"Erro ao processar pagamento: {ex.Message}, {ex.ApiResponse}, {apiErrorMessage},{apiCause},{apiErrorCause},{apiResponseContent},{apiResponseStatusCode}");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

           

        }

        [HttpPost("/api/ObterPreferencia")]
        public IActionResult ObterPreferencia([FromBody] Pagador pagador)
        {
            MercadoPagoConfig.AccessToken = "TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172";

            var preferenceRequest = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
        {
            new PreferenceItemRequest
            {
                Title = "Produto Exemplo",
                Quantity = 1,
                UnitPrice = 10000
            }
        },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://seusite.com/sucesso",
                    Failure = "https://seusite.com/falha",
                    Pending = "https://seusite.com/pendente"
                },
                AutoReturn = "approved",
                Payer = new PreferencePayerRequest { Email = pagador.Email }
            };

            var preferenceClient = new PreferenceClient();
            var createdPreference = preferenceClient.Create(preferenceRequest);

            return Ok(new { preferenceId = createdPreference.Id });
        }
        // PUT api/<MercadoPagoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MercadoPagoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
