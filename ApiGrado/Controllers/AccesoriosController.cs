
using ApiGrado.Modelos;
using ApiGrado.Modelos.Dtos;
using ApiGrado.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiGrado.Controllers
{
    [Route("api/accesorios")]
    [ApiController]
    public class AccesoriosController : ControllerBase
    {
        private readonly IAccesorioRepositorio _postRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AccesoriosController> _logger;
        public AccesoriosController(IAccesorioRepositorio postRepo, IMapper mapper, ILogger<AccesoriosController> logger)
        {
            _postRepo = postRepo;
            _mapper = mapper;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAccesorios()
        {
            var listaPosts = _postRepo.GetAccesorios();

            var listaPostsDto = new List<AccesorioDto>();

            foreach (var lista in listaPosts)
            {
                listaPostsDto.Add(_mapper.Map<AccesorioDto>(lista));
            }

            return Ok(listaPostsDto);
        }

        [AllowAnonymous]
        [HttpGet("{postId:int}", Name = "GetAccesorio")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAccesorio(int postId)
        {
            var itemPost = _postRepo.GetAccesorio(postId);

            if (itemPost == null)
            {
                return NotFound();
            }

            var itemPostDto = _mapper.Map<AccesorioDto>(itemPost);
            return Ok(itemPostDto);
        }

        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(AccesorioCrearDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearAccesorio([FromBody] AccesorioCrearDto crearPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (crearPostDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_postRepo.ExisteAccesorio(crearPostDto.Descripcion))
            {
                ModelState.AddModelError("", "El accesorio ya existe");
                return StatusCode(404, ModelState);
            }

            var accesorio = _mapper.Map<Accesorio>(crearPostDto);
            if (!_postRepo.CrearAccesorio(accesorio))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro{accesorio.Descripcion}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetAccesorio", new { postId = accesorio.Id }, accesorio);
        }


        [HttpPatch("{accesorioId:int}", Name = "ActualizarPatchAccesorio")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ActualizarPatchAccesorio(int accesorioId, [FromBody] AccesorioActualizarDto actualizarAccesorioDto)
        {
            if (actualizarAccesorioDto == null || accesorioId != actualizarAccesorioDto.Id)
            {
                return BadRequest(ModelState);
            }

            var accesorio = _mapper.Map<Accesorio>(actualizarAccesorioDto);

            if (!_postRepo.ActualizarAccesorio(accesorio))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {accesorio.Descripcion}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{accesorioId:int}", Name = "BorrarAccesorio")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarAccesorio(int accesorioId)
        {
            var accesorio = _postRepo.GetAccesorio(accesorioId);
            if (accesorio == null)
            {
                return NotFound();
            }

            if (!_postRepo.BorrarAccesorio(accesorio))
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el accesorio {accesorio.Descripcion}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
