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

            // if we have an empty search then just order the results by Id ascending
            var orderCriteria = "Id";
            var orderAscendingDirection = true;

            if (dtParameters.Order != null)
            {
                // in this example we just default sort on the 1st column
                orderCriteria = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderAscendingDirection = dtParameters.Order[0].Dir.ToString().ToLower() == "asc";
            }

            var result = (await _context.Colaboradores.ToListAsync()).AsQueryable();

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(r => r.Nome != null && r.Nome.ToUpper().Contains(searchBy.ToUpper()));
            }

            result = orderAscendingDirection ? result.OrderByDynamic(orderCriteria, DtOrderDir.Asc) : result.OrderByDynamic(orderCriteria, DtOrderDir.Desc);

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var filteredResultsCount = result.Count();
            var totalResultsCount = await _context.Colaboradores.CountAsync();

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
