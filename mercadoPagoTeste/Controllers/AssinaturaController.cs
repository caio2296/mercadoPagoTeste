using MercadoPago.Config;
using mercadoPagoTeste.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace mercadoPagoTeste.Controllers
{
    public class AssinaturaController : Controller
    {

        [HttpPost("/api/criar-assinatura")]
        public async Task<IActionResult> CriarAssinatura([FromBody] AssinaturaRequisicao requisicao, [FromHeader(Name = "X-meli-session-id")] string deviceSessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceSessionId))
                {
                    return BadRequest("Device Session ID não foi fornecido.");
                }

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");
                    httpClient.DefaultRequestHeaders.Add("X-meli-session-id", deviceSessionId);

                    MercadoPagoConfig.AccessToken = "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172";

                    var subscriptionRequest = new
                    {
                        preapproval_plan_id = requisicao.preapproval_plan_id, // ID do plano criado previamente no Mercado Pago
                        reason = "Auto registro",
                        external_reference = "Auto",
                        payer_email = requisicao.Email,
                        card_token_id = requisicao.Token,
                        additional_info = new
                        {
                            ip_address = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            payer = new
                            {
                                first_name = "Débora",
                                last_name = "Oliveira",
                                email = requisicao.Email,
                                phone = new
                                {
                                    area_code = "21",
                                    number = "972076680"
                                },
                                address = new
                                {
                                    street_name = "Rua Jose Felipe da Silva",
                                    street_number = "10",
                                    zip_code = "23560239"
                                }
                            }
                        },
                        auto_recurring = new
                        {
                            frequency = 1,
                            frequency_type = "months",
                            //start_date = "2020-06-02T13:07:14.260Z",
                            //"end_date": "2022-07-20T15:59:52.581Z",
                            transaction_amount = 10,
                            currency_id = "BRL"
                        },
                        back_url = "https://seusite.com/sucesso-assinatura",
                        status = "authorized"
                    };

                    var conteudo = new StringContent(JsonSerializer.Serialize(subscriptionRequest), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("https://api.mercadopago.com/preapproval", conteudo);

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

        // PUT api/<MercadoPagoController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PausarAssinatura(string id, [FromBody] string value)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");


                    MercadoPagoConfig.AccessToken = "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172";

                    var AtualizarRequest = new
                    {
                        status = "paused"
                    };

                    var conteudo = new StringContent(JsonSerializer.Serialize(AtualizarRequest), Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync($"https://api.mercadopago.com/preapproval/{id}", conteudo);

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
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterAssinatura(string id)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/preapproval/{id}");

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

        [HttpGet("BuscarAssinaturaPorEmail")]
        public async Task<IActionResult> BuscarAssinaturaPorEmail(string email)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/preapproval/search?payer_email={Uri.EscapeDataString(email)}");

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

        [HttpGet("BuscarCliente")]
        public async Task<IActionResult> BuscarCliente(string id)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/v1/customers/{id}");

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

        [HttpGet("BuscarPagamentosEmail")]
        public async Task<IActionResult> BuscarPagamentosExternall(string externalReference, string payerId)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/v1/payments/search?external_reference={Uri.EscapeDataString(externalReference)}&payer.id={Uri.EscapeDataString(payerId)}");

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


        [HttpPost("/api/CriarAssinaturaCartãoSalvo")]
        public async Task<IActionResult> CriarAssinaturaCartãoSalvo([FromBody] string token, [FromHeader(Name = "X-meli-session-id")] string deviceSessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceSessionId))
                {
                    return BadRequest("Device Session ID não foi fornecido.");
                }

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");
                    httpClient.DefaultRequestHeaders.Add("X-meli-session-id", deviceSessionId);

                    MercadoPagoConfig.AccessToken = "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172";

                    var subscriptionRequest = new
                    {
                        preapproval_plan_id = "2c93808493dee0900193fe07b5d40d33", // ID do plano criado previamente no Mercado Pago
                        reason = "Auto registro",
                        external_reference = "Auto",
                        payer_email = "",
                        card_token_id = token,
                        additional_info = new
                        {
                            ip_address = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            payer = new
                            {
                                first_name = "",
                                last_name = "",
                                email = "",
                                phone = new
                                {
                                    area_code = "",
                                    number = ""
                                },
                                address = new
                                {
                                    street_name = "  ",
                                    street_number = "",
                                    zip_code = ""
                                }
                            }
                        },
                        auto_recurring = new
                        {
                            frequency = 1,
                            frequency_type = "months",
                            //start_date = "2020-06-02T13:07:14.260Z",
                            //"end_date": "2022-07-20T15:59:52.581Z",
                            transaction_amount = 10,
                            currency_id = "BRL"
                        },
                        back_url = "https://seusite.com/sucesso-assinatura",
                        status = "authorized"
                    };

                    var conteudo = new StringContent(JsonSerializer.Serialize(subscriptionRequest), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("https://api.mercadopago.com/preapproval", conteudo);

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

        [HttpGet("/api/ObterFatura")]
        public async Task<IActionResult> ObterFatura(string id)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/authorized_payments/{id}");

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

        [HttpGet("/api/BuscarFatura")]
        public async Task<IActionResult> BuscarFatura(string preapproval_id)
        {
            try
            {

                //token de teste:
                //TEST-8247032065591220-101311-f9f1d87ef9fb1c5b8f302eead9d8b2f9-208999172
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "APP_USR-8247032065591220-101311-9e0427ade185518b9e4ee3702b8ec7bc-208999172");

                    var response = await httpClient.GetAsync($"https://api.mercadopago.com/authorized_payments/search?preapproval_id={Uri.EscapeDataString(preapproval_id)}");

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
