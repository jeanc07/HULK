/// <summary>
/// Clase utilizada para guardar los valores de las variables.
/// </summary>
public class TokenValue : SpecialTokensClass
{
    private string valor;

    public TokenValue(string? nombre, string valor) : base(nombre)
    {
        this.valor = valor;
    }

    public string Valor    // the Name property
    {
        get => valor;
        set => valor = value;
    }
}