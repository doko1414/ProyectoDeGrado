using Newtonsoft.Json;
using PersonalizacionProyectoGradoWASM.Helpers;
using PersonalizacionProyectoGradoWASM.Modelos;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;
using System.Text;

namespace PersonalizacionProyectoGradoWASM.Servicios
{
    public class AccesoriosServicio:IAccesoriosServicio
    {
        private readonly HttpClient _cliente;

        public AccesoriosServicio(HttpClient cliente)
        {
            _cliente = cliente;
        }

        public async Task<Accesorio> ActualizarAccesorio(int accesorioId, Accesorio accesorio)
        {
            var content = JsonConvert.SerializeObject(accesorio);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _cliente.PatchAsync($"{Inicializar.UrlBaseApi}api/accesorios/{accesorioId}", bodyContent);
            if (response.IsSuccessStatusCode)
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Accesorio>(contentTemp);
                return result;
            }
            else
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(contentTemp);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<Accesorio> CrearAccesorio(Accesorio accesorio)
        {
            var content = JsonConvert.SerializeObject(accesorio);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/accesorios", bodyContent);
            if (response.IsSuccessStatusCode)
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Accesorio>(contentTemp);
                return result;
            }
            else
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<bool> EliminarAccesorio(int accesorioId)
        {
            var response = await _cliente.DeleteAsync($"{Inicializar.UrlBaseApi}api/accesorios/{accesorioId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<Accesorio> GetAccesorio(int accesorioId)
        {
            var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/accesorios/{accesorioId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var accesorio = JsonConvert.DeserializeObject<Accesorio>(content);
                return accesorio;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<IEnumerable<Accesorio>> GetAccesorios()
        {
            var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/accesorios");
            var content = await response.Content.ReadAsStringAsync();
            var accesorios = JsonConvert.DeserializeObject<IEnumerable<Accesorio>>(content);
            return accesorios;
        }

        public Task<string> ObtenerRutaBicicleta()
        {
            // Aquí asumimos que la ruta es fija y conocida
            var rutaBicicleta = $"{Inicializar.UrlBaseApi}ImagenesPosts/ee47ad5a-0ebd-4cb7-9e0c-120870538ebb.glb";
            return Task.FromResult(rutaBicicleta);
        }

        public async Task<string> SubidaImagen(MultipartFormDataContent content)
        {
            var accesorioResult = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/upload", content);
            var accesorioContent = await accesorioResult.Content.ReadAsStringAsync();
            if (!accesorioResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(accesorioContent);
            }
            else
            {
                var modeloPath = $"{Inicializar.UrlBaseApi}{accesorioContent}";
                return modeloPath;
            }
        }
    }
}
