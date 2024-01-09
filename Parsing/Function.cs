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

    public Function(string name, List<Variable> parametro, List<string> cuerpo) : base(name)
    {
        this.parametro = parametro;
        this.cuerpo = cuerpo;
    }

    public Function() : base(null)
    {
        this.parametro = new List<Variable>();
        this.cuerpo = new List<string>();
    }
}
