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
using System.Text;
using System.Text.Json;
using MercadoPago.Client.Customer;
using MercadoPago.Resource.Customer;
using MercadoPago.Resource.Common;




// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace mercadoPagoTeste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MercadoPagoController : ControllerBase
    {

        // POST api/<MercadoPagoController>
        //[HttpPost("/api/ProcessarPagamentoCartao")]
        //public async Task<IActionResult> ProcessarPagamentoCartao([FromBody] SolicitacaoPagamentoCartao solicitacaopagamentoCartao)
        //{
        //    try
        //    {
        //        MercadoPagoConfig.AccessToken = "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172";

        //        var requestOptions = new RequestOptions();
        //        requestOptions.CustomHeaders.Add("x-idempotency-key", Guid.NewGuid().ToString());

        //        var paymentRequest = new PaymentCreateRequest
        //        {
        //            TransactionAmount = solicitacaopagamentoCartao.TransactionAmount,
        //            Token = solicitacaopagamentoCartao.Token,
        //            Description = solicitacaopagamentoCartao.Description,
        //            Installments = solicitacaopagamentoCartao.Installments,
        //            PaymentMethodId = solicitacaopagamentoCartao.PaymentMethodId,
        //            Payer = new PaymentPayerRequest
        //            {
        //                Email = solicitacaopagamentoCartao.CardholderEmail,
        //                Identification = new IdentificationRequest
        //                {
        //                    Type = solicitacaopagamentoCartao.IdentificationType,
        //                    Number = solicitacaopagamentoCartao.IdentificationNumber,
        //                },
        //                FirstName = solicitacaopagamentoCartao.CardholderName
        //            },
        //        };

        //        var client = new PaymentClient();
        //        Payment payment = await client.CreateAsync(paymentRequest, requestOptions);

        //        return Ok(payment);
        //    }
        //    catch (Exception ex)
        //    {
        //         return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpPost("/api/ProcessarPagamentoCartao")]
        public async Task<IActionResult> ProcessarPagamentoCartao([FromBody] SolicitacaoPagamentoCartao solicitacaoPagamentoCartao)
        {
            try
            {
                MercadoPagoConfig.AccessToken = "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172";

                var requestOptions = new RequestOptions();
                requestOptions.CustomHeaders.Add("x-idempotency-key", Guid.NewGuid().ToString());

                var paymentRequest = new PaymentCreateRequest
                {
                    TransactionAmount = solicitacaoPagamentoCartao.TransactionAmount,
                    Token = solicitacaoPagamentoCartao.Token,
                    Description = solicitacaoPagamentoCartao.Description,
                    Installments = solicitacaoPagamentoCartao.Installments,
                    PaymentMethodId = solicitacaoPagamentoCartao.PaymentMethodId,
                    Payer = new PaymentPayerRequest
                    {
                        Email = solicitacaoPagamentoCartao.CardholderEmail,
                        Identification = new IdentificationRequest
                        {
                            Type = solicitacaoPagamentoCartao.IdentificationType,
                            Number = solicitacaoPagamentoCartao.IdentificationNumber,
                        },
                        FirstName = solicitacaoPagamentoCartao.CardholderName
                    },
                    AdditionalInfo = new PaymentAdditionalInfoRequest
                    {
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(), // Obtém o IP do usuário
                        Payer = new PaymentAdditionalInfoPayerRequest
                        {
                            FirstName = "Débora",
                            LastName = "Oliveira",
                            Phone = new PhoneRequest
                            {
                                AreaCode ="21",
                                Number = "972076680"
                            },
                            Address = new AddressRequest
                            {
                                StreetName = "Rua Vieira Ravasco",
                                StreetNumber = "51",
                                ZipCode = "23520185"
                            }
                        },
                    }
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
            MercadoPagoConfig.AccessToken = "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172";

            var preferenceRequest = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
        {
            new PreferenceItemRequest
            {
                Title = "Auto Registro Assinatura",
                Quantity = 1,
                UnitPrice = 10
            }
        },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://seusite.com/sucesso",
                    Failure = "https://seusite.com/falha",
                    Pending = "https://seusite.com/pendente"
                },
                AutoReturn = "approved",
                Payer = new PreferencePayerRequest { Email = pagador.Email,
                    Identification = new IdentificationRequest() {
                                   Type = pagador.Identification.Type, Number = pagador.Identification.Number
                     } 
                }
            };
            
            var preferenceClient = new PreferenceClient();
            var createdPreference = preferenceClient.Create(preferenceRequest);

            return Ok(new { preferenceId = createdPreference.Id });
        }

        [HttpGet]
        [Route("api/CapturarPagamento")]
        public async Task <IActionResult> CapturarPagamento(string id)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/v1/payments/{id}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, $"Erro na requisição: {errorContent}");
                    }

                    return Ok(await response.Content.ReadAsStringAsync());

                }
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/api/CriarCliente")]

        public async Task<IActionResult> CriarCliente([FromBody] AssinaturaRequisicao solicitacaoPagamentoCartao, [FromHeader(Name = "X-meli-session-id")] string deviceSessionId)
        {

            MercadoPagoConfig.AccessToken = "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172";

            var customerRequest = new CustomerRequest
            {
                Email = "deboranascy18@gmail.com",
                FirstName = "Débora",
                LastName = "Oliveira",
                Phone = new PhoneRequest{
                    AreaCode ="51",
                    Number = "972076680"
                },
                Identification = new IdentificationRequest
                {
                    Type="CPF",
                    Number="13712066783"
                },
                DefaultAddress= "Casa",
                Address = new CustomerDefaultAddressRequest {
                        Id = "123123",
                        ZipCode = "23520185",
                        StreetName = "Rua Vieira Ravasco",
                        StreetNumber = 51,
                }

            };
            var customerClient = new CustomerClient();
            Customer customer = await customerClient.CreateAsync(customerRequest);

            var cardRequest = new CustomerCardCreateRequest
            {
                Token = solicitacaoPagamentoCartao.Token
            };
            CustomerCard card = await customerClient.CreateCardAsync(customer.Id, cardRequest);
            return Ok(card);

        }

        [HttpGet]
        [Route("api/ObterCliente")]
        public async Task<IActionResult> ObterClienteEmail(string email)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/v1/customers/search?email={email}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, $"Erro na requisição: {errorContent}");
                    }

                    return Ok(await response.Content.ReadAsStringAsync());

                }
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("api/ObterListaCartoesCliente/{id}")]
        //[Route("api/ObterListaCartoesCliente/{id}")]
        public async Task<IActionResult> ObterListaCartoesCliente(string id)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/v1/customers/{id}/cards");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, $"Erro na requisição: {errorContent}");
                    }

                    return Ok(await response.Content.ReadAsStringAsync());

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
