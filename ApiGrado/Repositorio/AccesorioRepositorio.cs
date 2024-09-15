using ApiGrado.Data;
using ApiGrado.Modelos;
using ApiGrado.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiGrado.Repositorio
{
    public class AccesorioRepositorio:IAccesorioRepositorio
    {
        private readonly ApplicationDbContext _bd;

        public AccesorioRepositorio(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        public bool ActualizarAccesorio(Accesorio accesorio)
        {
            accesorio.FechaActualizacion = DateTime.Now;
            var imagenDesdeBd = _bd.Accesorios.AsNoTracking().FirstOrDefault(c => c.Id == accesorio.Id);

            if (accesorio.RutaImagen == null)
            {
                accesorio.RutaImagen = imagenDesdeBd.RutaImagen;
            }

            _bd.Accesorios.Update(accesorio);
            return Guardar();
        }

        public bool BorrarAccesorio(Accesorio accesorio)
        {
            _bd.Accesorios.Remove(accesorio);
            return Guardar();
        }

        public bool CrearAccesorio(Accesorio accesorio)
        {
            accesorio.FechaCreacion = DateTime.Now;
            _bd.Accesorios.Add(accesorio);
            return Guardar();
        }

        public bool ExisteAccesorio(string nombre)
        {
            bool valor = _bd.Accesorios.Any(c => c.Descripcion.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool ExisteAccesorio(int id)
        {
            return _bd.Accesorios.Any(c => c.Id == id);
        }

        public Accesorio GetAccesorio(int accesorioId)
        {
            return _bd.Accesorios.FirstOrDefault(c => c.Id == accesorioId);
        }

        public ICollection<Accesorio> GetAccesorios()
        {
            return _bd.Accesorios.OrderBy(c => c.Id).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
