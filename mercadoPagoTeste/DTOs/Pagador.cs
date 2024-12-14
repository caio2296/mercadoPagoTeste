using MercadoPago.Client.Common;
using System.ComponentModel.DataAnnotations;

namespace mercadoPagoTeste.DTOs
{
    public class Pagador
    {
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do Email é inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo FirstName é obrigatório.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "O campo LastName é obrigatório.")]
        public string LastName { get; set; }

        public IdentificationRequest Identification { get; set; }
    }
}
