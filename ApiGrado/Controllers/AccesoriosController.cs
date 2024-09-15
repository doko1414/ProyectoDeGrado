
using ApiGrado.Modelos;
using ApiGrado.Modelos.Dtos;
using ApiGrado.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ApiGrado.Controllers
{
    [Route("api/accesorios")]
    [ApiController]
    public class AccesoriosController : ControllerBase
    {
        private readonly IAccesorioRepositorio _postRepo;
        private readonly IMapper _mapper;

        public AccesoriosController(IAccesorioRepositorio postRepo, IMapper mapper)
        {
            _postRepo = postRepo;
            _mapper = mapper;
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

        
        [HttpPatch("{postId:int}", Name = "ActualizarPatchAccesorio")]
        [ProducesResponseType(201, Type = typeof(AccesorioCrearDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchAccesorio(int accesorioId, [FromBody] AccesorioActualizarDto actualizarAccesorioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (actualizarAccesorioDto == null || accesorioId != actualizarAccesorioDto.Id)
            {
                return BadRequest(ModelState);
            }

            var accesorio = _mapper.Map<Accesorio>(actualizarAccesorioDto);

            if (!_postRepo.ActualizarAccesorio(accesorio))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro{accesorio.Descripcion}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        
        [HttpDelete("{postId:int}", Name = "BorrarAccesorio")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult BorrarAccesorio(int accesorioId)
        {
            if (!_postRepo.ExisteAccesorio(accesorioId))
            {
                return NotFound();
            }

            var accesorio = _postRepo.GetAccesorio(accesorioId);

            if (!_postRepo.BorrarAccesorio(accesorio))
            {
                ModelState.AddModelError("", $"Error, Algo salió mal borrando el registro{accesorio.Descripcion}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
