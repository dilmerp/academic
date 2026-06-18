namespace Academic.Domain.Entities;

public class GrupoPromocionItem
{
    public int Id { get; set; }
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public int IdTipo { get; set; }
    public int IdSubTipo { get; set; }
    public int IdProducto { get; set; }
    public int IdModulo { get; set; }
}