using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VendorMngSystem.DbData;
using VendorMngSystem.Dtos;
using VendorMngSystem.IRepositorys;
using VendorMngSystem.IServices;
using VendorMngSystem.Models;
using VendorMngSystem.Repositorys;
using VendorMngSystem.ViewModel;
namespace VendorMngSystem.Controllers
{
    public class VendorsController : Controller
    {
        //private readonly DataDbContext _context;
        //private readonly IVendorsRepository _vendorsRepository;
        //private readonly IMapper _mapper;
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IVendorService _vendorService;

        public VendorsController( IWebHostEnvironment webHostEnvironment, IVendorService vendorService)
        {
            _webHostEnvironment = webHostEnvironment;
            _vendorService = vendorService;
        }

        // GET: Vendors
        public async Task<IActionResult> Index()
        {
            var vendorList = await _vendorService.GetAllVendors();
            return View(vendorList);
        }
        [HttpGet]
        public async Task<IActionResult> CreateUpdate(int id)
        {
            try
            {
                var model = await _vendorService.GetVendorById(id);
                //old doc map in maaping class
                return PartialView("_Create", model);
            }
            catch (Exception ex)
            {
                var dropdown = new VendorsDto();
                
                dropdown.vendorCategoriesList = await _vendorService.GetVendorCategoriesDropDown();
                return PartialView("_Create", dropdown);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdate([FromForm] VendorsDto dto)
        {
            if (!ModelState.IsValid)
            {

                // Reload existing documents 
                dto.ExistingDocuments = await _vendorService.VendorDocumentsList(dto.VendorId);
                dto.vendorCategoriesList = await _vendorService.GetVendorCategoriesDropDown();
                return PartialView("_Create", dto);
            }

            try
            {
                var (rowChanged, newId) = await _vendorService.CreateUpdateVendor(dto, _webHostEnvironment.WebRootPath);

                return Json(new { rowchanged = rowChanged, vendorId = dto.VendorId });
            }
            catch (Exception ex)
            {
                return Json(new { rowchanged = 0, vendorId = dto.VendorId, message = ex.Message });
            }
        
        }
        
        

        [HttpGet]
        public async Task<IActionResult> GetVendorCategoryDropDown()
        {
            var list = await _vendorService.GetVendorCategoriesDropDown();
            return Json(list);
        
        }


        [HttpPost]
        public async Task<IActionResult> VendorDatatable(DatatableParameters parameters)
        {
            var result = await _vendorService.GetVendorsDataTable(parameters);
            return Json(result);

        }



        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
             {
                int rowChanged = await _vendorService.DeleteVendorRecord(id);
                return Json(new { rowChanged = rowChanged });
             }
             catch (Exception e)
             {

                return Json(new { rowChanged = 0 });
            }

        }



    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> xCreateUpdate([FromForm] VendorsDto dto) //remove
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            dto.vendorCategoriesList = await VendorCategoriesDropDown();
    //            return PartialView("_Create", dto);
    //        }
    //        try
    //        {
    //            await _unitOfWork.BeginTransactionAsync();
    //            var model = _mapper.Map<Vendors>(dto);
    //            if (dto.VendorId == 0)
    //            {
    //                await _unitOfWork._vendorsRepository.CreateVendor(model);
    //            }
    //            else
    //            {
    //                var VendorWithDoc = await _unitOfWork._vendorsRepository.FindVendorsWithVendorDocument(dto.VendorId);

    //                if (VendorWithDoc == null)
    //                {
    //                    return Json(new { rowchanged = 0, vendorId = dto.VendorId });
    //                }
    //                _mapper.Map(dto, VendorWithDoc);  // to Dto
    //            }

    //            //File Upload code..
    //            if (dto.VendorDocumentsPath != null && dto.VendorDocumentsPath.Count > 0)
    //            {
    //                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploadsDocumnets", "VendorDocuments");
    //                if (!Directory.Exists(uploadsFolder))
    //                {
    //                    Directory.CreateDirectory(uploadsFolder);
    //                }

    //                foreach (var file in dto.VendorDocumentsPath)
    //                {
    //                    if (file.Length > 0)
    //                    {
    //                        string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
    //                        string fullPath = Path.Combine(uploadsFolder, fileName);
    //                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
    //                        {
    //                            await file.CopyToAsync(fileStream);
    //                        }
    //                        var vDDto = new VendorDocumentsDto
    //                        {
    //                            VendorFileName = file.FileName,
    //                            VendorFilePath = $"/uploadsDocumnets/VendorDocuments/{fileName}"
    //                        };

    //                        var vendorDoc = _mapper.Map<VendorDocuments>(vDDto);
    //                        // vendor class automatic save doc table.
    //                        // await _unitOfWork._vendorDocumentRepository.AddDocument(vendorDoc);
    //                        //vendorModel.VendorDocuments.Add(vendorDoc);

    //                    }
    //                }
    //            }
    //            int rowchanged = await _unitOfWork.CommitTransaction();
    //            return Json(new { rowchanged, dto.VendorId });
    //        }
    //        catch (Exception e)
    //        {
    //            return Json(new { rowchanged = 0, dto.VendorId });

    //        }


    //    }
   }
}
  
