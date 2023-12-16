/// <summary>
/// Clase implementada para englobar todas las variables y funciones.
/// </summary>
public class SpecialTokensClass
{
    protected string? nombre;

    public SpecialTokensClass(string? nombre)
    {
        this.nombre = nombre;
    }

    public string? Nombre    // the Name property
    {
        get => nombre;
        set => nombre = value;
    }
}