namespace ApiEspeciales.Modules.Especiales2.Models
{
    public class EstadoTraslado
    {
        public const int ANULADO = 0;
        public const int GENERADO = 1;
        public const int POR_RECIBIR = 2;
        public const int RECIBIDO = 3;
        public const int DEPURADO = 4;
        public const int COMPLETADO = 5;
    }

    public static class EstadoDetalleTraslado
    {
        public const int NO_DEPURADO = 1;
        public const int NO_EXISTE_CLIENTE = 2;
        public const int EXISTE_CLIENTE = 3;
        public const int EXISTE_COINCIDENCIA = 4;
        public const int NO_EXISTE_COINCIDENCIA = 5;
    }

}
