using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RisePayTest.Data;
using RisePayTest.Models;
using RisePayTest.Models.AuxiliaryModels;
using RisePayTest.Extensions;
namespace RisePayTest.Controllers
{
    public class ColaboradorController : Controller
    {
        private readonly AppDbContext _context;
        public ColaboradorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var cargos = await _context.Cargos.ToListAsync();
            if (!cargos.Any())
            {
                await GenerateCargos();
            }
            return View();
        }

        public async Task<IActionResult> LoadTable(DtParameters dtParameters)
        {
            var searchBy = dtParameters.Search?.Value;

            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = (await _context.Colaboradores.Include(c => c.Cargo).ToListAsync()).AsQueryable();

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.Nome != null && r.Nome.ToUpper().Contains(searchBy.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            var filteredResultsCount = result.Count();
            var totalResultsCount = await _context.Colaboradores.CountAsync();

            if(dtParameters.Length == -1)
            {
                dtParameters.Length = filteredResultsCount;
            }

            return Json(new DtResult<Colaborador>
            {
                Draw = dtParameters.Draw,
                RecordsTotal = totalResultsCount,
                RecordsFiltered = filteredResultsCount,
                Data = result
                    .Skip(dtParameters.Start)
                    .Take(dtParameters.Length)
                    .ToList()
            });
        }
        public async Task<IActionResult> Create()
        {
            ViewData["Cargos"] = await _context.Cargos.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Nome,Email,Telefone,IdCargo")] Colaborador colaborador)
        {
            ViewData["Cargos"] = await _context.Cargos.ToListAsync();

            ModelState.Remove("Cargo");
            if (!ModelState.IsValid)
            {
                return View(colaborador);
            }

            _context.Add(colaborador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Cargos"] = await _context.Cargos.ToListAsync();
            var colaborador = await _context.Colaboradores.FindAsync(id);
            if(colaborador == null)
            {
                return NotFound();
            } 
            return View(colaborador);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id,Nome,Email,Telefone,IdCargo")] Colaborador colaborador)
        {
            ModelState.Remove("Cargo");
            if (!ModelState.IsValid)
            {
                
                return View(colaborador);
            }

            _context.Update(colaborador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }

        private async Task GenerateCargos()
        {
            List<Cargo> novosCargos = 
                [
                    new Cargo() { Nome = "Analista"},
                    new Cargo() { Nome = "Suporte"},
                    new Cargo() { Nome = "DBA"},
                    new Cargo() { Nome = "Administrador de Rede"}
                ];
            foreach(var cargo in novosCargos)
            {
                _context.Add(cargo);
                await _context.SaveChangesAsync();
            }
        }
    }


}
