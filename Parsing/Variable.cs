/// <summary>
/// CLase utilizada para almacenar las variables.
/// </summary>
public class Variable : TokenValue
{
    private string tipo;

    public string Tipo    // the Type property
    {
        get => tipo;
        set => tipo = value;
    }    

    public Variable(string nombre,string tipo, string valor) : base(nombre, valor)
    {
        this.tipo = tipo;
    }


}