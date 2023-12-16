/// <summary>
/// Clase usada para guardar las funciones definidas por el usuario.
/// </summary>
public class Function : SpecialTokensClass
{
    private List<Variable> parametro;

    private List<string> cuerpo;

    public List<Variable> Parametro {
        get => parametro;
        set => parametro = value;
    }

    public List<string> Cuerpo {
        get => cuerpo;
        set => cuerpo = value;
    }

    public Function(string nombre, List<Variable> parametro, List<string> cuerpo) : base(nombre)
    {
        this.parametro = parametro;
        this.cuerpo = cuerpo;
    }
}
