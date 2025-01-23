namespace mercadoPagoTeste.DTOs
{
    public class AssinaturaRequisicao
    {
        public string preapproval_plan_id { get; set; } // ID do plano de assinatura
        public string Email { get; set; }
        //public string IdentificationType { get; set; }
        //public string IdentificationNumber { get; set; }
        public string BackUrl { get;set; }
        public string Token { get; set; }

        public AssinaturaRequisicao()
        {
                this.BackUrl = "https://seusite.com/sucesso-assinatura";
        }
    }
}
